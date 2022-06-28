using UnityEngine;


[System.Serializable]
public class AudioSettingsData {

	[Range (0f, 1f)] public float masterVolume;
	[Range (0f, 1f)] public float musicVolume;
	[Range (0f, 1f)] public float sfxVolume;
}
