using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerTutorialComponent : PlayerComponent {

	public SpellData autoBindSpellData;
	public SpellData manualBindSpellData;
	public override void SubscribeToCreatureEvents () {
		Debug.Log ("Animation Controller subscribing to events");
		base.SubscribeToCreatureEvents ();
		creatureObject.OnSetFaceDirEvent += OnSetFaceDirection;
		creatureObject.OnSetVelocityEvent += OnSetVelocity;

	}
	public override void UnsubscribeFromCreatureEvents () {
		base.UnsubscribeFromCreatureEvents ();
		creatureObject.OnSetFaceDirEvent -= OnSetFaceDirection;
		creatureObject.OnSetVelocityEvent -= OnSetVelocity;

	}

	private bool movementComplete = false;
	public void OnSetVelocity (Vector2 velocity) {
		if (!movementComplete && velocity != Vector2.zero) {
			bool taskComplete = TutorialManager.instance.SetTaskComplete (TutorialTask.MOVEMENT);
			if (taskComplete) {
				movementComplete = true;
			}
		}
	}

	private bool aimingComplete = false;
	private bool aimedLeft = false;
	private bool aimedRight = false;
	public void OnSetFaceDirection (int dir) {

		if (aimingComplete == false) {
			if (dir < 0) {
				aimedLeft = true;
			} else {
				aimedRight = true;
			}
			if (aimedLeft && aimedRight) {
				TutorialManager.instance.SetTaskComplete (TutorialTask.AIMING);
			}
		}
	}
	//BASICS

	//
	public override void OnDash (DashInfo dashInfo) {
		TutorialManager.instance.SetTaskComplete (TutorialTask.DODGE);
	}

	private bool primaryPickedUp = false;
	private bool secondaryPickedUp = false;
	//PUZZLE STAFF
	public override void OnPickUpStaff (PuzzleKey region, PuzzleGameData puzzleGameData) {
		if (region == PuzzleKey.PRIMARY_STAFF) {
			TutorialManager.instance.SetTaskComplete (TutorialTask.PICK_UP_STAFF_PRIMARY);
		} else if (region == PuzzleKey.SECONDARY_STAFF) {
			TutorialManager.instance.SetTaskComplete (TutorialTask.PICK_UP_STAFF_SECONDARY);
		}
	}

	//PUZZLE SPELLGEM

	private int playerStateChangeCounter = 0;
	public override void OnChangePlayerState (PlayerState playerState) {
		if (playerState == PlayerState.PUZZLE_BROWSING) {
			playerStateChangeCounter++;
		}
		else if (playerStateChangeCounter >= 1 && playerState == PlayerState.COMBAT) {
			TutorialManager.instance.SetTaskComplete (TutorialTask.PUZZLE_TOGGLE);
		}
	}

	public override void OnPickUpSpellGem (SpellGemGameData spellGemGameData) {
		TutorialManager.instance.SetTaskComplete (TutorialTask.PICK_UP_SPELLGEM);
	}
	public override void OnBindSpellGem (PuzzleGameData puzzleGameData, SpellGemGameData spellGameData, PuzzleBindType bindType) {

		if (bindType == PuzzleBindType.AUTOMATIC)
			TutorialManager.instance.SetTaskComplete (TutorialTask.AUTO_BIND_SPELLGEM);
		else if (bindType == PuzzleBindType.MANUAL)
			TutorialManager.instance.SetTaskComplete (TutorialTask.MANUAL_BIND_SPELLGEM);
	}
	public override void OnUnbindSpellGem (PuzzleGameData puzzleGameData, SpellGemGameData spellGameData, PuzzleUnbindType unbindType) {
		TutorialManager.instance.SetTaskComplete (TutorialTask.UNBIND_SPELLGEM);
	}
	//COMBAT
	public override void OnAttack (AttackInfo attackInfo) {
		if (attackInfo.spellData != null) {
			if (attackInfo.spellData == autoBindSpellData) {
				TutorialManager.instance.SetTaskComplete (TutorialTask.CAST_LEECHBOLT);
			} else if (attackInfo.spellData == manualBindSpellData) {
				TutorialManager.instance.SetTaskComplete (TutorialTask.CAST_TENTACLES);
			}
		}
	}

	public override void OnRotateSpellGem (SpellGemGameData spellGemGameData, int rotateIndex) {
		TutorialManager.instance.SetTaskComplete (TutorialTask.ROTATE_SPELLGEM);
	}
	public override void OnDropSpellGem (SpellGemGameData spellGemGameData) {
		TutorialManager.instance.SetTaskComplete (TutorialTask.DROP_SPELLGEM);
	}

	private bool primaryEquipped = false;
	private bool secondaryEquipped = false;
	public override void OnEquipStaff (PuzzleKey region, PuzzleGameData puzzleGameData, StaffEquipType equipType) {
		if (equipType == StaffEquipType.MANUAL_SWAP) {
			if (region == PuzzleKey.PRIMARY_STAFF && !primaryEquipped) {
				primaryEquipped = true;
				TutorialManager.instance.SetTaskComplete (TutorialTask.MANUAL_EQUIP_PRIMARY);

			} else if (region == PuzzleKey.SECONDARY_STAFF && !secondaryEquipped) {
				secondaryEquipped = true;
				TutorialManager.instance.SetTaskComplete (TutorialTask.MANUAL_EQUIP_SECONDARY);
			}
		}
	}

	public override void OnDropStaff (PuzzleKey region, PuzzleGameData puzzleGameData) {
		TutorialManager.instance.SetTaskComplete (TutorialTask.DROP_STAFF);
	}

}
