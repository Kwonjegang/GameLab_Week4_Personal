using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

namespace Incheon.Artillery
{
    [Serializable] public class ShellImpactEvent : UnityEvent<Vector3> { }

    /// <summary>Place the prefab, move LaunchPoint and ImpactTarget, then trigger Fire or FireVolley.</summary>
    public sealed class ArtilleryBarrage : MonoBehaviour
    {
        [Header("1. 발사 위치와 목표")]
        public ArtilleryShell shellPrefab;
        public Transform launchPoint;
        public Transform impactTarget;
        [Tooltip("목표 아래의 지면을 찾아 높이를 맞춥니다. Ground 레이어를 사용합니다.")]
        public bool snapTargetToGround = true;
        public LayerMask groundMask = 1 << 3;
        [Tooltip("비행 중 충돌할 레이어. Trigger는 무시합니다.")]
        public LayerMask collisionMask = ~( (1 << 2) | (1 << 4) | (1 << 5) );

        [Header("2. 카메라에서 보이는 비행")]
        [Min(.05f), Tooltip("출발에서 목표까지 걸리는 시간(초)")] public float flightTime = 2.2f;
        [Min(0), Tooltip("출발-목표 직선보다 위로 솟는 높이")] public float arcHeight = 10f;
        [Min(0), Tooltip("목표 주위로 흩어지는 반경. 0이면 정확한 한 지점")] public float scatterRadius = 4f;

        [Header("3. 자동 포격 / 반복 재사용")]
        [Tooltip("켜면 Play 시 자동 포격. 타임라인으로 호출하려면 끕니다.")] public bool automaticFire = true;
        [Min(0)] public float startDelay = 1f;
        [Min(.05f)] public float fireInterval = 1.2f;
        [Range(1, 16)] public int shellsPerVolley = 1;
        [Range(1, 96), Tooltip("씬 시작 시 준비할 포탄 수. 사용 중인 한도를 넘으면 추가 발사는 생략합니다.")] public int poolSize = 16;
        [Tooltip("같은 값이면 Play할 때 같은 산포 순서로 재현됩니다.")] public int randomSeed = 1950;

        [Header("4. 바다 착탄")]
        [Tooltip("수면 Collider 없이 지정한 수면 높이에서 물보라를 재생합니다.")] public bool useWaterPlane = true;
        public float waterLevel = 0f;

        [Header("5. 착탄 연결 — 카메라/피격은 필요할 때 직접 연결")]
        public ShellImpactEvent onImpact = new ShellImpactEvent();

        public int FiredCount { get; private set; }
        public int ImpactCount { get; private set; }
        public int WaterImpactCount { get; private set; }
        public int DroppedCount { get; private set; }
        public int PoolCount => shells == null ? 0 : shells.Length;
        public int ActiveCount
        {
            get { int n = 0; if (shells != null) foreach (var shell in shells) if (shell != null && shell.IsBusy) n++; return n; }
        }
        ArtilleryShell[] shells;
        System.Random random;
        float nextFire;

        void Awake() => EnsurePool();
        void OnEnable() => nextFire = Time.time + Mathf.Max(0, startDelay);
        void Update()
        {
            if (automaticFire && Time.time >= nextFire)
            {
                FireVolley();
                // Do not create a catch-up burst after a long frame or pause.
                nextFire = Time.time + Mathf.Max(.05f, fireInterval);
            }
        }
        void OnDisable() => StopAllShells();

        bool EnsurePool()
        {
            if (shells != null) return true;
            if (shellPrefab == null) return false;
            random = new System.Random(randomSeed);
            shells = new ArtilleryShell[Mathf.Clamp(poolSize, 1, 96)];
            var pool = new GameObject("Shell_Pool_Runtime");
            pool.transform.SetParent(transform, false);
            pool.SetActive(false);
            for (int i = 0; i < shells.Length; i++)
            {
                shells[i] = Instantiate(shellPrefab, pool.transform);
                shells[i].name = "Pooled_Shell_" + (i + 1).ToString("00");
                shells[i].ResetForPool();
            }
            pool.SetActive(true);
            return true;
        }

        [ContextMenu("Play 중 / 포탄 한 발 발사")]
        public void Fire()
        {
            if (!Application.isPlaying || launchPoint == null || impactTarget == null) return;
            FireAt(impactTarget.position);
        }

        [ContextMenu("Play 중 / 설정한 수만큼 일제 사격")]
        public void FireVolley()
        {
            if (!Application.isPlaying || launchPoint == null || impactTarget == null) return;
            for (int i = 0; i < Mathf.Clamp(shellsPerVolley, 1, 16); i++) FireAt(impactTarget.position);
        }

        // Also callable by another gameplay script. A false result means no free pooled shell.
        public bool FireAt(Vector3 targetPosition)
        {
            if (!isActiveAndEnabled || launchPoint == null || !EnsurePool()) return false;
            ArtilleryShell available = null;
            foreach (var shell in shells) if (shell != null && !shell.IsBusy) { available = shell; break; }
            if (available == null) { DroppedCount++; return false; }
            float angle = (float)random.NextDouble() * Mathf.PI * 2;
            float radius = Mathf.Sqrt((float)random.NextDouble()) * Mathf.Max(0, scatterRadius);
            targetPosition += new Vector3(Mathf.Cos(angle) * radius, 0, Mathf.Sin(angle) * radius);
            bool waterTarget = false;
            var physics = gameObject.scene.GetPhysicsScene();
            if (snapTargetToGround && physics.Raycast(targetPosition + Vector3.up * 100, Vector3.down,
                out var hit, 300f, groundMask, QueryTriggerInteraction.Ignore)) targetPosition.y = hit.point.y;
            if (useWaterPlane && targetPosition.y <= waterLevel)
            { targetPosition.y = waterLevel; waterTarget = true; }
            available.Launch(this, launchPoint.position, targetPosition, flightTime, arcHeight,
                collisionMask, useWaterPlane, waterLevel, waterTarget);
            FiredCount++;
            return true;
        }

        public void StartBarrage() { automaticFire = true; nextFire = Time.time; }
        public void StopBarrage() => automaticFire = false;
        public void StopAllShells()
        {
            if (shells == null) return;
            foreach (var shell in shells) if (shell != null) shell.ResetForPool();
        }
        public void NotifyImpact(Vector3 point, bool water)
        {
            ImpactCount++;
            if (water) WaterImpactCount++;
            onImpact.Invoke(point);
        }

        void OnDrawGizmosSelected()
        {
            if (launchPoint == null || impactTarget == null) return;
            Gizmos.color = new Color(1, .65f, .12f);
            Vector3 previous = launchPoint.position;
            for (int i = 1; i <= 32; i++)
            {
                Vector3 next = ArtilleryShell.ArcPosition(launchPoint.position, impactTarget.position, arcHeight, i / 32f);
                Gizmos.DrawLine(previous, next); previous = next;
            }
            Gizmos.color = Color.cyan; Gizmos.DrawWireSphere(launchPoint.position, .5f);
            Gizmos.color = Color.red; Gizmos.DrawWireSphere(impactTarget.position, Mathf.Max(.3f, scatterRadius));
        }
    }
}
