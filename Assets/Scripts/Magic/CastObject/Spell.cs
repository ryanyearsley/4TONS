using System;
using UnityEngine;

public enum SpellCastLocation {
	Staff, Cursor, PlayerFeet, PlayerCenter
}
public enum SpellCastType {
	CAST, CHANNEL, SPECIAL
}

public abstract class Spell : MonoBehaviour {

	protected PlayerObject playerObject;
	//static
	public SpellData spellData;

	[SerializeField]
	public int debrisPoolSize;
	public GameObject debrisObject;
	//dynamic
	[NonSerialized]
	public bool onCooldown = false;

	protected Transform spellCastTransform;
	protected float cdTimer;

	[NonSerialized]
	public SpellUI spellUI;
	void Awake () {
		SetUpSpell ();
	}


	public virtual void SetUpSpell () {
		if (spellData.spellObject != null)
			PoolManager.instance.CreateSpellObjectPool (spellData.spellObject, spellData.poolSize);

		if (spellData.spellCastSound.singleClip != null)
			AudioManager.instance.RegisterSound (spellData.spellCastSound);
	}

	public virtual void ConfigureSpellToPlayer (PlayerObject playerObject) {
		this.playerObject = playerObject;
		transform.parent = playerObject.transform;
		switch (spellData.spellCastLocation) {
			case SpellCastLocation.Staff:
				spellCastTransform = playerObject.creaturePositions.staffAimTransform;
				break;
			case SpellCastLocation.Cursor:
				spellCastTransform = playerObject.creaturePositions.targetTransform;
				break;
			case SpellCastLocation.PlayerFeet:
				spellCastTransform = playerObject.creaturePositions.feetTransform;
				break;
		}
	}

	public virtual void UpdateSpellUI() {
		if (spellUI != null) {
			spellUI.SetSpellUIToSpell (spellData);
			float fillPercentage = 1 - cdTimer/spellData.coolDown;
			spellUI.UpdateSpellUICooldown (fillPercentage, cdTimer);
		}
	}

	private void Update () {
		if (onCooldown) {
			cdTimer -= Time.deltaTime;

			if (cdTimer <= 0) {
				onCooldown = false;
			}

			if (spellUI != null) {
				float fillPercentage = 1 - cdTimer/spellData.coolDown;
				spellUI.UpdateSpellUICooldown (fillPercentage, cdTimer);
			}
		}
	}

	public void OnBindSpell () {

	}

	public virtual bool isCastEligible () {
		if (playerObject.currentPlayerState != PlayerState.COMBAT
			|| onCooldown
			|| !playerObject.canAttack
			|| !playerObject.vitalsEntity.resource.HasEnoughMana (spellData.manaCost)
			)
			return false;
		return true;
	}

	public virtual void SpellButtonDown () {
		if (isCastEligible ()) {
			CastSpell ();
		}
	}
	public virtual void SpellButtonHold () {
	}
	public virtual void SpellButtonUp () {

	}
	public virtual void CancelCast() {

	}
	public virtual void CastSpell () {
		playerObject.OnCastSpell (this, SpellCastType.CAST);
		cdTimer = spellData.coolDown;
		spellUI.GreyOutSpellUI ();
		onCooldown = true;
	}
	public virtual void EndSpell () {
	}

}
