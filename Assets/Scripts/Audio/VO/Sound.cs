using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;


[System.Serializable]
public class Sound {

	[SerializeField]
	private AudioSource source;

	public string clipName;
	public AudioClip singleClip;
	public bool randomize = false;
	public AudioClip[] randomClips;

	public float volume = 1;
	public float pitch = 1;
	public AudioMixerGroup mixerGroup;

	public bool loop = false;
	public bool playOnAwake = false;
	public void SetSource (AudioSource _source) {

		source = _source;
		source.volume = volume;
		source.pitch = pitch;
		source.loop = loop;
		source.playOnAwake = playOnAwake;
		source.outputAudioMixerGroup = mixerGroup;

		if (randomize && randomClips.Length > 0) {
			source.clip = randomClips [Random.Range (0, randomClips.Length)];
			source.pitch = Random.Range (0.5f, 1.5f);
		} else {
			source.clip = singleClip;
		}

		if (playOnAwake) {
			source.Play ();
		}
	}

	public void Play () {
		if (randomize) {
			source.pitch = Random.Range (0.5f, 1.5f);
		}
		if (randomClips.Length > 0) {
			source.PlayOneShot (randomClips [Random.Range (0, randomClips.Length)]);
		} else {
			source.PlayOneShot (singleClip);
		}
	}

	public void Stop () {
		source.Stop ();
	}
}