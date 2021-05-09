using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Base class for all components used on players only
public class PlayerComponent : CreatureComponent, IPlayerComponent
{
	protected PlayerObject playerObject;
	#region Player Component Callbacks


	//first time init
	public override void SetUpComponent (GameObject rootObject) {
		base.SetUpComponent (rootObject);
		playerObject = rootObject.GetComponent<PlayerObject> ();
	}
	//When player is assigned (on ReusePlayerObject)
	public virtual void ReusePlayerComponent (Player player) {
	}
	public override void SubscribeToCreatureEvents () {
		base.SubscribeToCreatureEvents ();
		playerObject.OnChangePlayerStateEvent += OnChangePlayerState;
		playerObject.setAimingModeEvent += OnSetAimingMode;
		playerObject.OnDashEvent += OnDash;
		playerObject.PickUpStaffEvent += OnPickUpStaff;
		playerObject.EquipStaffEvent += OnEquipStaff;
		playerObject.DropStaffEvent += OnDropStaff;
		playerObject.PickUpSpellGemEvent += OnPickUpSpellGem;
		playerObject.DropSpellGemEvent += OnDropSpellGem;
		playerObject.BindSpellGemEvent += OnBindSpellGem;
		playerObject.UnbindSpellGemEvent += OnUnbindSpellGem;
		playerObject.RotateSpellGemEvent += OnRotateSpellGem;
	}

	public override void UnsubscribeFromCreatureEvents () {
		base.UnsubscribeFromCreatureEvents ();
		playerObject.OnChangePlayerStateEvent -= OnChangePlayerState;
		playerObject.setAimingModeEvent -= OnSetAimingMode;
		playerObject.OnDashEvent -= OnDash;
		playerObject.PickUpStaffEvent -= OnPickUpStaff;
		playerObject.EquipStaffEvent -= OnEquipStaff;
		playerObject.DropStaffEvent -= OnDropStaff;
		playerObject.PickUpSpellGemEvent -= OnPickUpSpellGem;
		playerObject.DropSpellGemEvent -= OnDropSpellGem;
		playerObject.BindSpellGemEvent -= OnBindSpellGem;
		playerObject.UnbindSpellGemEvent -= OnUnbindSpellGem;
		playerObject.UnbindSpellGemEvent -= OnUnbindSpellGem;
		playerObject.RotateSpellGemEvent -= OnRotateSpellGem;

	}
	public virtual void OnChangePlayerState(PlayerState playerState) {

	}
	public virtual void OnSetAimingMode(AimingMode aimingMode) {

	}
	public virtual void OnDash(DashInfo dashInfo) {

	}
	public virtual void OnPickUpStaff (PuzzleKey region, PuzzleGameData puzzleGameData) {

	}
	public virtual void OnDropStaff (PuzzleKey region, PuzzleGameData puzzleGameData) {

	}
	public virtual void OnEquipStaff(PuzzleKey region, PuzzleGameData puzzleGameData, StaffEquipType equipType) {

	}

	public virtual void OnPickUpSpellGem(SpellGemGameData spellGemGameData) {

	}
	public virtual void OnDropSpellGem(SpellGemGameData spellGemGameData) {

	}
	public virtual void OnRotateSpellGem (SpellGemGameData spellGemGameData, int rotateIndex) {

	}

	public virtual void OnBindSpellGem (PuzzleGameData puzzleGameData, SpellGemGameData spellGameData, PuzzleBindType bindType) {

	}

	public virtual void OnUnbindSpellGem (PuzzleGameData puzzleGameData, SpellGemGameData spellGameData, PuzzleUnbindType unbindType) {

	}


	#endregion
}
