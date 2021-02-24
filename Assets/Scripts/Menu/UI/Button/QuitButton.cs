using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class QuitButton : AbstractButtonClick
{

	protected override void OnClick () {
		Application.Quit ();

#if UNITY_EDITOR
		UnityEditor.EditorApplication.isPlaying = false;
#endif
	}
}
