using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor (typeof (AutoSetLayoutElement))]
public class AutoSetLayoutElementsEditor : Editor {

	private AutoSetLayoutElement setLayout;

	public override void OnInspectorGUI () {
		base.OnInspectorGUI ();

		if (setLayout == null)
			setLayout = (AutoSetLayoutElement)target;

		if (GUILayout.Button ("Update Layout Element")) {
			setLayout.UpdateLayoutElement ();
		}
	}
}