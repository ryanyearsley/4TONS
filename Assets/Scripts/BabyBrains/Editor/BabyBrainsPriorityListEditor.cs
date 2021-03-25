
using UnityEditor;
using UnityEngine;

[CustomEditor (typeof (BehaviourPriorityData))]
public class BabyBrainsPriorityListEditor : Editor {

	public override void OnInspectorGUI () {
		base.OnInspectorGUI ();
		BehaviourPriorityData priorityData = (BehaviourPriorityData) target;

		if (GUILayout.Button ("Assign Behaviour Weights")) {
			priorityData.AssignBehaviourWeights ();
		}
	}
}