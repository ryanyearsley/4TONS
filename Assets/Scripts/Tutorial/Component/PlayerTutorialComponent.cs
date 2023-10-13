using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerTutorialComponent : PlayerComponent
{

	public SpellData leechboltSpellData;
	public SpellData tentacleSpellData;
	public SpellData harvestFleshSpellData;
	public override void SubscribeToCreatureEvents()
	{
		base.SubscribeToCreatureEvents();
		creatureObject.OnSetFaceDirEvent += OnSetFaceDirection;
		creatureObject.OnSetVelocityEvent += OnSetVelocity;

	}
	public override void UnsubscribeFromCreatureEvents()
	{
		base.UnsubscribeFromCreatureEvents();
		creatureObject.OnSetFaceDirEvent -= OnSetFaceDirection;
		creatureObject.OnSetVelocityEvent -= OnSetVelocity;

	}

	private bool movementComplete = false;
	public void OnSetVelocity(Vector2 velocity)
	{
		if (!movementComplete && velocity != Vector2.zero)
		{
			bool taskComplete = TutorialManager.instance.SetTaskComplete(TutorialTask.MOVEMENT);
			if (taskComplete)
			{
				movementComplete = true;
			}
		}
	}

	private bool aimingComplete = false;
	private bool aimedLeft = false;
	private bool aimedRight = false;
	public void OnSetFaceDirection(int dir)
	{

		if (aimingComplete == false)
		{
			if (dir < 0)
			{
				aimedLeft = true;
			}
			else
			{
				aimedRight = true;
			}
			if (aimedLeft && aimedRight)
			{
				TutorialManager.instance.SetTaskComplete(TutorialTask.AIMING);
			}
		}
	}
	//BASICS

	//
	public override void OnDash(DashInfo dashInfo)
	{
		TutorialManager.instance.SetTaskComplete(TutorialTask.DODGE);
	}

	private bool primaryPickedUp = false;
	private bool secondaryPickedUp = false;
	//PUZZLE STAFF
	public override void OnPickUpStaff(PuzzleKey region, PuzzleGameData puzzleGameData)
	{
		if (region == PuzzleKey.PRIMARY_STAFF)
		{
			TutorialManager.instance.SetTaskComplete(TutorialTask.PICK_UP_STAFF_PRIMARY);
		}
		else if (region == PuzzleKey.SECONDARY_STAFF)
		{
			TutorialManager.instance.SetTaskComplete(TutorialTask.PICK_UP_STAFF_SECONDARY);
		}
	}

	//PUZZLE SPELLGEM

	private int playerStateChangeCounter = 0;
	public override void OnChangePlayerState(PlayerState playerState)
	{
		if (playerState == PlayerState.PUZZLE_BROWSING)
		{
			playerStateChangeCounter++;
			TutorialManager.instance.SetTaskComplete(TutorialTask.PUZZLE_TOGGLE_ON);
		}
		else if (playerStateChangeCounter >= 1 && playerState == PlayerState.COMBAT)
		{
			TutorialManager.instance.SetTaskComplete(TutorialTask.PUZZLE_TOGGLE_OFF);
		}
	}

	public override void OnPickUpSpellGem(SpellGemGameData spellGemGameData)
	{
		TutorialManager.instance.SetTaskComplete(TutorialTask.PICK_UP_SPELLGEM);
	}
	public override void OnBindSpellGem(PuzzleGameData puzzleGameData, SpellGemGameData spellGameData, PuzzleBindType bindType)
	{

		if (puzzleGameData.puzzleKey == PuzzleKey.PRIMARY_STAFF)
		{
			if (spellGameData.spellData == leechboltSpellData)
			{
				TutorialManager.instance.SetTaskComplete(TutorialTask.BIND_LEECHBOLT);
			}
			else if (spellGameData.spellData == tentacleSpellData) 
			{
				TutorialManager.instance.SetTaskComplete(TutorialTask.BIND_TENTACLES);
			}
			else if (spellGameData.spellData == harvestFleshSpellData)
			{
				TutorialManager.instance.SetTaskComplete(TutorialTask.BIND_HARVEST);
			}
		}
		else if (puzzleGameData.puzzleKey == PuzzleKey.INVENTORY)
		{
			if (spellGameData.spellData == tentacleSpellData)
			{
				TutorialManager.instance.SetTaskComplete(TutorialTask.BIND_TENTACLE_INVENTORY);
			}
		}

		if (bindType == PuzzleBindType.HOT_SWAP || bindType == PuzzleBindType.COLD_SWAP)
		{
			TutorialManager.instance.SetTaskComplete(TutorialTask.QUICK_SWAP_SPELLGEMS);
		}
	}
	public override void OnUnbindSpellGem(PuzzleGameData puzzleGameData, SpellGemGameData spellGameData, PuzzleUnbindType unbindType)
	{
		TutorialManager.instance.SetTaskComplete(TutorialTask.UNBIND_SPELLGEM);
	}
	//COMBAT
	public override void OnAttack(AttackInfo attackInfo)
	{
		Debug.Log("PlayerTutorialComponent: OnAttack.");
		if (attackInfo.spellData != null)
		{
			if (attackInfo.spellData == leechboltSpellData)
			{
				TutorialManager.instance.SetTaskComplete(TutorialTask.CAST_LEECHBOLT);
			}
			else if (attackInfo.spellData == tentacleSpellData)
			{
				TutorialManager.instance.SetTaskComplete(TutorialTask.CAST_TENTACLES);
			}
			else if (attackInfo.spellData == harvestFleshSpellData)
			{
				TutorialManager.instance.SetTaskComplete(TutorialTask.CAST_HARVEST);
			}
		}
	}

	public override void OnRotateSpellGem(SpellGemGameData spellGemGameData, int rotateIndex)
	{
		TutorialManager.instance.SetTaskComplete(TutorialTask.ROTATE_SPELLGEM);
	}
	public override void OnDropSpellGem(SpellGemGameData spellGemGameData)
	{
		TutorialManager.instance.SetTaskComplete(TutorialTask.DROP_SPELLGEM);
	}

	public override void OnEquipStaff(PuzzleKey region, PuzzleGameData puzzleGameData, StaffEquipType equipType)
	{
		if (equipType == StaffEquipType.MANUAL_SWAP)
		{
			if (region == PuzzleKey.PRIMARY_STAFF)
			{
				TutorialManager.instance.SetTaskComplete(TutorialTask.MANUAL_EQUIP_PRIMARY);

			}
			else if (region == PuzzleKey.SECONDARY_STAFF)
			{
				TutorialManager.instance.SetTaskComplete(TutorialTask.MANUAL_EQUIP_SECONDARY);
			}
		} else if (equipType == StaffEquipType.QUICK_SWAP)
		{
			TutorialManager.instance.SetTaskComplete(TutorialTask.QUICK_SWAP_STAFF);
		}
	}

	public override void OnDropStaff(PuzzleKey region, PuzzleGameData puzzleGameData)
	{
		TutorialManager.instance.SetTaskComplete(TutorialTask.DROP_STAFF);
	}

}
