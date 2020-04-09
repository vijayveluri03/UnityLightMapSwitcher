using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(TimeOfDay))]
public class TimeOfDayInspector : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        TimeOfDay timeOfDay = (TimeOfDay)target;

        if (GUILayout.Button("Day"))
        {
            timeOfDay.Day();
        }
        if (GUILayout.Button("Night"))
        {
            timeOfDay.Night();
        }

    }
}
