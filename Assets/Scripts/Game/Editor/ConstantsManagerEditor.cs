using UnityEngine;
using UnityEditor;

[CustomEditor (typeof (ConstantsManager))]
public class ConstantsManagerEditor : Editor {

    public override void OnInspectorGUI () {
        base.OnInspectorGUI ();
        ConstantsManager constantsManager = (ConstantsManager) target;

        if (GUILayout.Button ("Update ID Mapping")) {
            constantsManager.MapObjectIDsToLegend ();
        }
    }
}
