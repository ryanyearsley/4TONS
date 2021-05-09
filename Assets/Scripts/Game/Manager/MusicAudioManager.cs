using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicAudioManager : AudioManager
{
	public new static MusicAudioManager instance;
	protected override void Awake () {

		if (instance == null)
			instance = this;
		else if (instance != this)
			Destroy (gameObject);
	}
	protected override void Start () {
		base.Start ();
		PlaySound ("Music");
	}
}
