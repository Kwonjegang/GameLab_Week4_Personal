using UnityEngine;
using UnityEngine.SceneManagement;

namespace Incheon.Artillery
{
    /// <summary>Game-only, directed arc with swept collision. Owned and reused by ArtilleryBarrage.</summary>
    public sealed class ArtilleryShell : MonoBehaviour
    {
        [Header("Prefab references — 제작된 프리팹 연결")]
        public GameObject model;
        public TrailRenderer flightTrail;
        public Transform impactRoot;
        public ParticleSystem[] groundEffects;
        public ParticleSystem[] waterEffects;
        [Min(.1f)] public float impactLifetime = 3.6f;
        [Min(.01f)] public float collisionRadius = .12f;

        public bool IsFlying { get; private set; }
        public bool IsBusy { get; private set; }
        ArtilleryBarrage owner;
        Vector3 start, destination;
        float age, duration, arcHeight, waterY;
        int collisionMask;
        bool hasWater, destinationIsWater;
        PhysicsScene physicsScene;

        public static Vector3 ArcPosition(Vector3 from, Vector3 to, float height, float time01)
        {
            float t = Mathf.Clamp01(time01);
            return Vector3.LerpUnclamped(from, to, t) + Vector3.up * (4f * height * t * (1f - t));
        }

        public void Launch(ArtilleryBarrage source, Vector3 from, Vector3 to, float seconds, float height,
            int mask, bool useWater, float waterLevel, bool waterTarget)
        {
            ResetForPool();
            owner = source; start = from; destination = to;
            duration = Mathf.Max(.05f, seconds); arcHeight = Mathf.Max(0, height);
            collisionMask = mask; hasWater = useWater; waterY = waterLevel; destinationIsWater = waterTarget;
            physicsScene = gameObject.scene.GetPhysicsScene();
            age = 0; IsFlying = IsBusy = true;
            transform.position = from;
            FaceFlight(0);
            gameObject.SetActive(true);
            if (model != null) model.SetActive(true);
            if (flightTrail != null) { flightTrail.Clear(); flightTrail.emitting = true; }
        }

        void Update() => Step(Time.deltaTime);

        // Small time steps keep fast projectiles from cutting through a curved path on slow frames.
        void Step(float deltaTime)
        {
            if (!IsBusy || deltaTime <= 0) return;
            if (!IsFlying)
            {
                age += deltaTime;
                if (age >= impactLifetime) ResetForPool();
                return;
            }
            float remaining = Mathf.Min(deltaTime, duration - age);
            while (remaining > .000001f && IsFlying)
            {
                float dt = Mathf.Min(remaining, 1f / 60f);
                Vector3 previous = transform.position;
                age = Mathf.Min(age + dt, duration);
                Vector3 next = ArcPosition(start, destination, arcHeight, age / duration);
                Vector3 segment = next - previous;
                float length = segment.magnitude;
                bool groundHit = false;
                RaycastHit hit = default;
                if (length > .000001f)
                    groundHit = physicsScene.SphereCast(previous, collisionRadius, segment / length,
                        out hit, length, collisionMask, QueryTriggerInteraction.Ignore);
                bool crossedWater = hasWater && previous.y >= waterY && next.y <= waterY && previous.y > next.y;
                float waterFraction = crossedWater ? (previous.y - waterY) / (previous.y - next.y) : 2f;
                if (crossedWater && (!groundHit || waterFraction * length < hit.distance))
                    Impact(Vector3.Lerp(previous, next, waterFraction), Vector3.up, true);
                else if (groundHit)
                    Impact(hit.point, hit.normal, false);
                else
                {
                    transform.position = next;
                    FaceFlight(age / duration);
                    if (age >= duration) Impact(destination, Vector3.up, destinationIsWater);
                }
                remaining -= dt;
            }
            // Float rounding must not leave a shell permanently busy just before its endpoint.
            if (IsFlying && duration - age <= .000001f)
                Impact(destination, Vector3.up, destinationIsWater);
        }

        void FaceFlight(float t)
        {
            Vector3 direction = destination - start + Vector3.up * (4f * arcHeight * (1f - 2f * t));
            if (direction.sqrMagnitude > .00001f) transform.rotation = Quaternion.LookRotation(direction);
        }

        void Impact(Vector3 point, Vector3 normal, bool water)
        {
            IsFlying = false; age = 0;
            transform.position = point;
            transform.rotation = Quaternion.identity;
            if (model != null) model.SetActive(false);
            if (flightTrail != null) { flightTrail.emitting = false; flightTrail.Clear(); }
            if (impactRoot != null)
            {
                impactRoot.position = point + normal * .04f;
                impactRoot.rotation = Quaternion.FromToRotation(Vector3.up, normal);
            }
            var effects = water ? waterEffects : groundEffects;
            if (effects != null) foreach (var effect in effects)
                if (effect != null) effect.Play(false);
            owner?.NotifyImpact(point, water);
        }

        public void ResetForPool()
        {
            IsFlying = IsBusy = false; age = 0; owner = null;
            if (flightTrail != null) { flightTrail.emitting = false; flightTrail.Clear(); }
            Clear(groundEffects); Clear(waterEffects);
            if (model != null) model.SetActive(true);
            gameObject.SetActive(false);
        }

        static void Clear(ParticleSystem[] effects)
        {
            if (effects == null) return;
            foreach (var effect in effects)
                if (effect != null) effect.Stop(false, ParticleSystemStopBehavior.StopEmittingAndClear);
        }
    }
}
