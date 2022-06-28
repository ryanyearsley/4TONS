using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MusicManager : PersistentManager
{

	public AudioSource musicSource;

	public override void InitializePersistentManager () {
		PlaySceneAppropriateMusic ();
	}

	public override void SceneLoaded (Scene scene, LoadSceneMode loadSceneMode) {
		PlaySceneAppropriateMusic ();
	
	}

	public void PlaySceneAppropriateMusic() {
		StopMusic ();
		if (GameManager.instance != null) {
			PlayMusic (GameManager.instance.gameContext.zoneData.music);
		} else
			PlayMusic (ConstantsManager.instance.mainMenuMusic);
	}


	public void PlayMusic (Sound sound) {
		musicSource.clip = sound.singleClip;
		musicSource.Play ();
	}
	public void StopMusic () {
		musicSource.Stop ();
	}
}
