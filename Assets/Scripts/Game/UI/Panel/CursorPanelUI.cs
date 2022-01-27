using UnityEngine.SceneManagement;
using UnityEngine;
public class CursorPanelUI : AbstractPanelUI
{
	//since the cursor panel is persistent, we initialize OnLevelLoad
	
    void OnEnable () {
        //Tell our 'OnLevelFinishedLoading' function to start listening for a scene change as soon as this script is enabled.
        SceneManager.sceneLoaded += OnLevelFinishedLoading;
    }

    void OnDisable () {
        //Tell our 'OnLevelFinishedLoading' function to stop listening for a scene change as soon as this script is disabled. Remember to always have an unsubscription for every delegate you subscribe to!
        SceneManager.sceneLoaded -= OnLevelFinishedLoading;
    }

    void OnLevelFinishedLoading (Scene scene, LoadSceneMode mode) {
        InitializePanel ();
    }
    protected override void InitializePanel () {
		base.InitializePanel ();
		if (SceneManager.GetActiveScene ().buildIndex == 0) {
			panelObject.SetActive (true);
		}

	}

}
