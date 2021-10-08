using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//virtually the same as a spellobject, except there is no life timer on these.
public class PortalController : InteractableObject {
	private Animator animator;

	private CircleCollider2D portalCollider;
	[SerializeField]
	private bool opened;
	public override void SetupObject () {
		animator = GetComponentInChildren<Animator> ();
		portalCollider = GetComponent<CircleCollider2D> ();
		portalCollider.enabled = false;
		opened = false;
		base.SetupObject ();
	}

	public override void ReuseObject() {
		base.ReuseObject ();
		SubscribeToEvents ();
		opened = false;
		animator.SetTrigger ("close");
		portalCollider.enabled = false;
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
		portalCollider.enabled = true;
	}

	public override void InteractWithObject () {
		GauntletGameManager.instance.PortalEntered ();
		portalCollider.enabled = false;
	}

	public void OnLevelEnd(int floorIndex) {
		Destroy ();
	}
	public override void TerminateObjectFunctions () {
		opened = false;
		UnsubscribeFromEvents ();
	}
}
