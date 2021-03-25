using UnityEngine.SceneManagement;
using UnityEngine;
public class CursorPanelUI : AbstractPanelUI
{
	//since the cursor panel is persistent, we initialize OnLevelLoad
	void OnLevelWasLoaded () {
		InitializePanel ();
	}
	protected override void InitializePanel () {
		base.InitializePanel ();
		Debug.Log ("initializing cursor panel. scene build index: " + SceneManager.GetActiveScene ().buildIndex);
		if (SceneManager.GetActiveScene ().buildIndex == 0) {
			panelObject.SetActive (true);
		}

	}

}
