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
		playerObject.OnDashEvent += OnDash;
		playerObject.OnBindSpellGemEvent += OnBindSpellGem;
		playerObject.OnUnbindSpellGemEvent += OnUnbindSpellGem;
		playerObject.OnUpdateSpellBindingEvent += OnUpdateSpellBindingEvent;
	}

	public override void UnsubscribeFromCreatureEvents () {
		base.UnsubscribeFromCreatureEvents ();
		playerObject.OnChangePlayerStateEvent -= OnChangePlayerState;
		playerObject.OnDashEvent -= OnDash;
		playerObject.OnBindSpellGemEvent -= OnBindSpellGem;
		playerObject.OnUnbindSpellGemEvent -= OnUnbindSpellGem;
		playerObject.OnUpdateSpellBindingEvent -= OnUpdateSpellBindingEvent;

	}

	public virtual void OnChangePlayerState(PlayerState playerState) {

	}
	public virtual void OnDash(DashInfo dashInfo) {

	}
	public virtual void OnBindSpellGem (PuzzleGroupingDetails details, SpellSaveData spellSaveData) {

	}

	public virtual void OnUnbindSpellGem (PuzzleGroupingDetails details, SpellSaveData spellSaveData) {

	}
	public virtual void OnUpdateSpellBindingEvent (int spellBindIndex, Spell spell) {

	}
	#endregion
}
