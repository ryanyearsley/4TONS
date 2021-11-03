using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//virtually the same as a spellobject, except there is no life timer on these.
public class PortalController : InteractableObject {
	private Animator animator;


	private InteractableCollider portalCollider;
	[SerializeField]
	private bool opened;
	public override void SetupObject () {
		base.SetupObject ();
		animator = GetComponentInChildren<Animator> ();
		portalCollider = GetComponentInChildren<InteractableCollider> ();
		opened = false;
	}
	public override void ReuseObject () {
		SubscribeToEvents ();
		opened = false;
		animator.SetTrigger ("close");
		portalCollider.SetNonInteractable ();
	}

	public override void TerminateObjectFunctions() {
		portalCollider.SetNonInteractable ();
		UnsubscribeFromEvents ();
		opened = false;
	}
	private void SubscribeToEvents () {
		GameManager.instance.levelCompleteEvent += OnLevelComplete;
		GameManager.instance.levelEndEvent += OnLevelEnd;
	}
	private void UnsubscribeFromEvents () {
		GameManager.instance.levelCompleteEvent -= OnLevelComplete;
		GameManager.instance.levelEndEvent -= OnLevelEnd;
	}
	public void OnLevelComplete(int floorIndex) {
		opened = true;
		animator.SetTrigger ("open");
		portalCollider.SetInteractable ();
	}

	public override void InteractWithObject () {
		GauntletGameManager.instance.PortalEntered ();
		portalCollider.SetNonInteractable ();
	}

	public void OnLevelEnd(int floorIndex) {
		Destroy ();
	}
}
