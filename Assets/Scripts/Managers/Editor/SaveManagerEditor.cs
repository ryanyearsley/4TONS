using UnityEngine;
using UnityEditor;

[CustomEditor (typeof (SaveManager))]
public class SaveManagerEditor : Editor {

    public override void OnInspectorGUI () {
        base.OnInspectorGUI ();
        SaveManager saveManager = (SaveManager) target;

        if (GUILayout.Button ("Reset Save Data")) {
            saveManager.SaveDataToDisk (saveManager.defaultData);
        }
    }
}
