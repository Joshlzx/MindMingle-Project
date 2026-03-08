using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(RhythmChartGenerator))]
public class RhythmChartGeneratorEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector(); // shows all normal fields

        RhythmChartGenerator gen = (RhythmChartGenerator)target;

        if (GUILayout.Button("Generate Notes From Chart"))
        {
            gen.GenerateNotes(); // calls your method
        }
    }
}