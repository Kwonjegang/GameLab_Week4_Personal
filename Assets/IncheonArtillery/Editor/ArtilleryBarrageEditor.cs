using Incheon.Artillery;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(ArtilleryBarrage))]
public sealed class ArtilleryBarrageEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        var barrage = (ArtilleryBarrage)target;
        EditorGUILayout.Space();
        EditorGUILayout.HelpBox("LaunchPoint = 날아오는 출발점 / ImpactTarget = 떨어질 목표점\n아래 발사 버튼은 Play 중에 사용합니다.", MessageType.Info);
        using (new EditorGUI.DisabledScope(!Application.isPlaying))
        {
            if (GUILayout.Button("포탄 한 발 발사")) barrage.Fire();
            if (GUILayout.Button("설정한 수만큼 일제 사격")) barrage.FireVolley();
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("자동 포격 시작")) barrage.StartBarrage();
            if (GUILayout.Button("자동 포격 중지")) barrage.StopBarrage();
            EditorGUILayout.EndHorizontal();
        }
        if (Application.isPlaying)
        {
            EditorGUILayout.LabelField("사용 중 / 준비된 포탄", barrage.ActiveCount + " / " + barrage.PoolCount);
            EditorGUILayout.LabelField("발사 / 착탄 / 한도 초과 생략", barrage.FiredCount + " / " + barrage.ImpactCount + " / " + barrage.DroppedCount);
            Repaint();
        }
    }
}
