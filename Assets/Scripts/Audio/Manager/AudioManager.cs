using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;

//1. Updates audio mixer settings on init, and on audio setting update event
//2. Registers appropriate audio sources to dictionary.
//3. Deletes all sounds on new scene load + registers generic sounds again.
public class AudioManager : PersistentManager {
	#region Singleton
	public static AudioManager instance { get; private set; }
	protected override void InitializeSingleton () {
		if (instance == null) {
			instance = this;
		} else if (instance != this) {
			Destroy (gameObject);
		}
	}
	#endregion


	private static string MASTER_VOLUME_PARAM = "MasterVolume";
	private static string MUSIC_VOLUME_PARAM = "MusicVolume";
	private static string SFX_VOLUME_PARAM = "SFXVolume";
	private static int DB_MULTIPLIER  = 20;

	public AudioSettingsData currentAudioSettings;

	public AudioMixer mixer;
	public Sound[] menuSounds;
	public Sound[] genericGameSounds;

	public Dictionary<string, Sound> soundDictionary = new Dictionary<string, Sound>();

	public override void InitializePersistentManager () {
		if (SettingsManager.instance != null) {
			currentAudioSettings = SettingsManager.instance.GetAudioSettings ();
			ApplyAudioSettings (currentAudioSettings);
		} else {
			Debug.Log ("AudioManager: Cannot load audio settings (SettingsManager null)");
		}
		RegisterSounds ();
	}

	public override void SubscribeToEvents () {
		base.SubscribeToEvents ();
		SettingsManager.instance.updateAudioEvent += ApplyAudioSettings;
	}
	public override void UnsubscribeFromEvents () {
		base.UnsubscribeFromEvents ();
		SettingsManager.instance.updateAudioEvent -= ApplyAudioSettings;
	}

	public override void SceneLoaded (Scene scene, LoadSceneMode loadSceneMode) {
		RegisterSounds ();
	}

	public void RegisterSounds () {
		soundDictionary.Clear ();
		foreach (Transform child in transform) {
			GameObject.Destroy (child.gameObject);
		}

		foreach (Sound sound in menuSounds) {
			RegisterSound (sound);
		}

		if (SceneManager.GetActiveScene().buildIndex != 0) {
			foreach (Sound sound in genericGameSounds) {
				RegisterSound (sound);
			}
		}
	}

	private void ApplyAudioSettings (AudioSettingsData audioSettingsData) {
		Debug.Log ("AudioManager: Applying Audio setting update.");
		mixer.SetFloat (MASTER_VOLUME_PARAM, Mathf.Log10 (audioSettingsData.masterVolume) * DB_MULTIPLIER);
		mixer.SetFloat (MUSIC_VOLUME_PARAM, Mathf.Log10 (audioSettingsData.musicVolume) * DB_MULTIPLIER);
		mixer.SetFloat (SFX_VOLUME_PARAM, Mathf.Log10 (audioSettingsData.sfxVolume) * DB_MULTIPLIER);
	}
	public void RegisterSound (Sound sound) {
		sound.clipName = sound.singleClip.name;
		if (sound != null && !soundDictionary.ContainsKey (sound.clipName)) {
			GameObject _go = new GameObject ("Sound_" + sound.clipName);
			_go.transform.SetParent (this.transform);
			sound.SetSource (_go.AddComponent<AudioSource> ());
			soundDictionary.Add (sound.clipName, sound);
		}
	}

	public void PlaySound (string _name) {
		if (soundDictionary.ContainsKey (_name)) {
			soundDictionary [_name].Play ();
		}
	}
	public void StopSound (string _name) {
		if (soundDictionary.ContainsKey (_name)) {
			soundDictionary [_name].Stop ();
		}
	}

}