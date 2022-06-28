using System;
using UnityEngine;

public class SpellObject : PoolObject, ISpellObject {

	[SerializeField]
	protected float lifeTime = 5f;
	[SerializeField]
	protected float lifeTimer;
	[SerializeField]
	protected SpellEffect[] spellEffects;

	[NonSerialized]
	public VitalsEntity casterVitalsEntity;
	protected bool isAlive;



	protected AudioSource audioSource;
	protected Transform trans;


	[SerializeField]
	public int debrisPoolSize;
	public GameObject debrisObject;
	[SerializeField]
	protected Sound spellObjectSound;

	[SerializeField]
	private bool interruptSpellSoundOnDestroy = false;
	[SerializeField]
	private Sound spellObjectDestroySound;

	public override void SetupObject () {
		base.SetupObject ();
		if (debrisObject != null)
			PoolManager.instance.CreateObjectPool (debrisObject, debrisPoolSize);

		trans = this.transform;
		spellEffects = trans.GetComponentsInChildren<SpellEffect> ();

		foreach (SpellEffect spellEffect in spellEffects) {
			spellEffect.SetUpSpellEffect ();
		}
		if (spellObjectSound.singleClip != null) {
			AudioManager.instance.RegisterSound (spellObjectSound);
			/*
			audioSource = gameObject.AddComponent<AudioSource> ();
			audioSource.volume = spellObjectSound.volume;
			audioSource.maxDistance = 15f;
			audioSource.spatialBlend = 1f;
			audioSource.rolloffMode = AudioRolloffMode.Linear;*/
		}

		if (spellObjectDestroySound.singleClip != null) {
			AudioManager.instance.RegisterSound (spellObjectDestroySound);
		}
	}

	public virtual void SetSpellObjectTag (string tag) {
		if (tag != this.tag) {
			this.tag = tag;
		}
	}


	public virtual void ReuseSpellObject (VitalsEntity vitalsEntity) {
		casterVitalsEntity = vitalsEntity;
		if (spellObjectSound.singleClip != null)
			AudioManager.instance.PlaySound (spellObjectSound.clipName);
		lifeTimer = 0;
		isAlive = true;
	}

	public virtual void OnWallHit (Collider2D otherColl) {
	}
	public virtual void OnEnemyHit (VitalsEntity enemyVitals) {
		foreach (SpellEffect spellEffect in spellEffects) {
			spellEffect.OnEnemyHit (casterVitalsEntity, enemyVitals);
		}
	}
	public virtual void OnAllyHit (VitalsEntity ally) {
		foreach (SpellEffect spellEffect in spellEffects) {
			spellEffect.OnAllyHit (casterVitalsEntity, ally);
		}
	}

	public virtual void OnBarrierHit (BarrierObject barrierObject) {

	}

	public override void TerminateObjectFunctions () {
		isAlive = false;
		this.tag = "Untagged";
		if (interruptSpellSoundOnDestroy) {
			AudioManager.instance.StopSound (spellObjectSound.clipName);
		}
		if (spellObjectDestroySound.singleClip != null) {
			//audioSource.Stop ();
			AudioManager.instance.PlaySound (spellObjectDestroySound.clipName);
		}
		base.TerminateObjectFunctions ();

	}
	public virtual void Update () {
		if (isAlive) {
			UpdateLifeTimer ();
			foreach (SpellEffect spellEffect in spellEffects) {
				spellEffect.UpdateSpellEffect (casterVitalsEntity);
			}
		}
	}
	protected void UpdateLifeTimer () {
		if (isAlive) {
			lifeTimer += Time.deltaTime;
			if (lifeTimer >= lifeTime) {
				Destroy ();
			}
		}
	}
}
