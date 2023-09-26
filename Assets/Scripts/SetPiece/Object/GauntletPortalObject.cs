using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//virtually the same as a spellobject, except there is no life timer on these.
public class GauntletPortalObject : InteractableObject {
	private Animator animator;

	[SerializeField]
	private Sound portalOpenSound;


	private InteractableCollider portalCollider;
	[SerializeField]
	private bool opened;
	public override void SetupObject () {
		base.SetupObject ();
		animator = GetComponentInChildren<Animator> ();
		portalCollider = GetComponentInChildren<InteractableCollider> ();
		opened = false;
		if (portalOpenSound.singleClip != null) {
			AudioManager.instance.RegisterSound(portalOpenSound);
		}
	}
	public override void ReuseObject () {
		SubscribeToEvents ();
		opened = false;
		animator.SetTrigger ("lock");
		portalCollider.SetNonInteractable ();
	}

	public override void TerminateObjectFunctions() {
		portalCollider.SetNonInteractable ();
		UnsubscribeFromEvents ();
		opened = false;
	}
	protected virtual void SubscribeToEvents () {
		GameManager.instance.levelCompleteEvent += OnLevelComplete;
		GameManager.instance.levelEndEvent += OnLevelEnd;
	}
	protected virtual void UnsubscribeFromEvents () {
		GameManager.instance.levelCompleteEvent -= OnLevelComplete;
		GameManager.instance.levelEndEvent -= OnLevelEnd;
	}
	public void OnLevelComplete(int floorIndex) {
		if (!opened)
		{
			opened = true;
			animator.SetTrigger("open");
			portalCollider.SetInteractable();
			AudioManager.instance.PlaySound(portalOpenSound.clipName);
		}
	}

	public override void InteractWithObject () {
		GameManager.instance.LevelEnd ();
		portalCollider.SetNonInteractable ();
	}

	public void OnLevelEnd(int floorIndex) {
		Destroy ();
	}
}
