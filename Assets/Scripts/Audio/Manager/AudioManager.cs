using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;


public class AudioManager : MonoBehaviour {

	private static string MASTER_VOLUME_PARAM = "MasterVolume";
	private static string MUSIC_VOLUME_PARAM = "MusicVolume";
	private static string SFX_VOLUME_PARAM = "SFXVolume";
	private static int DB_MULTIPLIER  = 20;

	public SettingsData currentSettings;

	public AudioMixer mixer;
	public Sound[] sounds;

	public Dictionary<string, Sound> soundDictionary = new Dictionary<string, Sound>();


	public static AudioManager instance;
	void Awake () {

		if (instance == null)
			instance = this;
		else if (instance != this)
			Destroy (gameObject);
	}

	void Start () {

		if (SettingsManager.instance != null) {
			currentSettings = SettingsManager.instance.GetSettingsData ();
			SettingsManager.instance.updateSettingsEvent += ApplyAudioSettings;
		}
		ApplyAudioSettings (currentSettings);

		for (int i = 0; i < sounds.Length; i++) {
			RegisterSound (sounds [i]);
		}
		PlaySound ("Music");
	}

	private void ApplyAudioSettings(SettingsData settingsData) {
		mixer.SetFloat (MASTER_VOLUME_PARAM, Mathf.Log10 (settingsData.masterVolume) * DB_MULTIPLIER);
		mixer.SetFloat (MUSIC_VOLUME_PARAM, Mathf.Log10 (settingsData.musicVolume) * DB_MULTIPLIER);
		mixer.SetFloat (SFX_VOLUME_PARAM, Mathf.Log10 (settingsData.sfxVolume) * DB_MULTIPLIER);
	}
	public void RegisterSound (Sound sound) {
		if (!soundDictionary.ContainsKey (sound.clipName)) {
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
}