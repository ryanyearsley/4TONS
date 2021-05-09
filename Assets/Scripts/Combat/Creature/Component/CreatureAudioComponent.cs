using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[RequireComponent (typeof (AudioSource))]
public class CreatureAudioComponent : CreatureComponent {
	private AudioSource source;
	private CreatureAudioClipsPackage clipsPackage;
	public override void SetUpComponent (GameObject rootObject) {
		base.SetUpComponent (rootObject);
		source = GetComponent<AudioSource> ();
		clipsPackage = creatureObject.creatureData.clipsPackage;
	}



	public override void OnAttack (AttackInfo attackInfo) {
		if (clipsPackage.attackSound != null) {
			source.Stop ();
			source.PlayOneShot (clipsPackage.attackSound);
		}
	}
	public override void OnHit (HitInfo hitInfo) {

		if (clipsPackage.gruntSound != null) {
			source.Stop ();
			source.PlayOneShot (clipsPackage.gruntSound);
		}
	}

	public override void OnDeath () {
		if (clipsPackage.deathSound != null) {
			source.Stop ();
			source.PlayOneShot (clipsPackage.deathSound);
		}
	}

	public void PlaySound(AudioClip audioClip) {
		source.Stop ();
		source.PlayOneShot (audioClip);
	}
}