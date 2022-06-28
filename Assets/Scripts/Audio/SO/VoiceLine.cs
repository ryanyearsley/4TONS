using System;
using UnityEngine;

[Serializable]
public class VoiceLine
{
	public AudioClip audioClip;

	[TextArea(3,4)]
	public string subtitleText;
}
