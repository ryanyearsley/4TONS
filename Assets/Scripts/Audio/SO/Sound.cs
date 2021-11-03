using UnityEngine;
using UnityEngine.Audio;


[System.Serializable]
public class Sound  {


	private AudioSource source;

	public string clipName;
	public AudioClip singleClip;

	public float volume = 1;
	public float pitch = 1;
	public AudioMixerGroup mixerGroup;

	public bool loop = false;
	public bool randomize = false;
	public AudioClip[] randomClips;
	public void SetSource (AudioSource _source) {

		source = _source;
		source.volume = volume;
		source.pitch = pitch;
		source.loop = loop;
		source.outputAudioMixerGroup = mixerGroup;

		if (randomize && randomClips.Length > 0) {
			source.clip = randomClips [Random.Range (0, randomClips.Length)];
			source.pitch = Random.Range (0.5f, 1.5f);
		} else {
			source.clip = singleClip;
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