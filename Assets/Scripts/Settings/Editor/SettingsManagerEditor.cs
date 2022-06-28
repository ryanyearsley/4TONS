using UnityEngine;
using UnityEditor;

[CustomEditor (typeof (SettingsManager))]
public class SettingsManagerEditor : Editor {

    public override void OnInspectorGUI () {
        base.OnInspectorGUI ();
        SettingsManager settingsManager = (SettingsManager) target;

        if (GUILayout.Button ("Reset Save Data")) {
            settingsManager.ResetAudioSettings ();
            //settingsManager.DeleteAllWizardData ();
        }
    }
}
