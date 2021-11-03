using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;


public class AudioManager : PersistentManager {
	#region Singleton
	public static AudioManager instance { get; private set; }
	private void InitializeSingleton () {
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

	public SettingsData currentSettings;

	public AudioMixer mixer;
	public Sound[] sounds;
	public AudioClip[] musicTracks;
	public Sound musicSound;

	public Dictionary<string, Sound> soundDictionary = new Dictionary<string, Sound>();



	protected override void Awake () {
		base.Awake ();
		InitializeSingleton ();
	}
	protected override void Start () {
		base.Start ();
		if (SettingsManager.instance != null) {
			currentSettings = SettingsManager.instance.GetSettingsData ();
		}
		ApplyAudioSettings (currentSettings);

		for (int i = 0; i < sounds.Length; i++) {
			RegisterSound (sounds [i]);
		}
		RegisterSound (musicSound);

		PlayMusic (SceneManager.GetActiveScene().buildIndex);
	}
	public override void SceneLoaded (Scene scene, LoadSceneMode loadSceneMode) {
		StopMusic ();
		PlayMusic (scene.buildIndex);
	}

	private void ApplyAudioSettings (SettingsData settingsData) {
		mixer.SetFloat (MASTER_VOLUME_PARAM, Mathf.Log10 (settingsData.masterVolume) * DB_MULTIPLIER);
		mixer.SetFloat (MUSIC_VOLUME_PARAM, Mathf.Log10 (settingsData.musicVolume) * DB_MULTIPLIER);
		mixer.SetFloat (SFX_VOLUME_PARAM, Mathf.Log10 (settingsData.sfxVolume) * DB_MULTIPLIER);
	}
	public void RegisterSound (Sound sound) {
		sound.clipName = sound.singleClip.name;
		if (sound != null && !soundDictionary.ContainsKey (sound.clipName)) {
			GameObject _go = new GameObject ("Sound_" + sound.clipName);
			_go.transform.SetParent (this.transform);
			sound.SetSource (_go.AddComponent<AudioSource> ());
			soundDictionary.Add (sound.clipName, sound);
			Debug.Log ("AudioManager: Spell Registered. Clip name: " + sound.clipName);
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

	public void PlayMusic (int trackIndex) {
		musicSound.singleClip = musicTracks [trackIndex];
		musicSound.Play ();
	}
	public void StopMusic () {
		musicSound.Stop ();
	}
}