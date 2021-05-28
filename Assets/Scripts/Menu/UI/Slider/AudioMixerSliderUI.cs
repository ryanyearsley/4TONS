using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;

public class AudioMixerSliderUI : MonoBehaviour
{
	private SettingsScreenUI settingsScreenUI;

	public Slider slider;
	public AudioMixer mixer;
	public string audioMixerName;
	public float multiplier = 30f;

	protected virtual void Awake () {
		slider = GetComponent<Slider> ();
		slider.onValueChanged.AddListener (OnValueChanged);
	}

	public void SetSliderValue(float value) {
		slider.value = value;
	}
	public float GetSliderValue () {
		return slider.value;
	}
	protected virtual void OnValueChanged (float value) {
		mixer.SetFloat (audioMixerName, Mathf.Log10 (value) * multiplier);
	}

	public void SaveAudioSliderSetting () {
		PlayerPrefs.SetFloat (audioMixerName, slider.value);
	}

}
