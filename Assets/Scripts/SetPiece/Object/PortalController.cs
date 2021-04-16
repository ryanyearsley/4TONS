using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//virtually the same as a spellobject, except there is no life timer on these.
public class PortalController : PoolObject {
	private Animator animator;

	private int useCounter;

	private CircleCollider2D portalCollider;
	[SerializeField]
	private bool opened;
	private void Awake () {
	}

	public override void SetupObject () {
		animator = GetComponentInChildren<Animator> ();
		portalCollider = GetComponent<CircleCollider2D> ();
		portalCollider.enabled = false;
		useCounter = 0;
		opened = false;
		base.SetupObject ();
	}

	public override void ReuseObject() {
		base.ReuseObject ();
		SubscribeToEvents ();
		useCounter++;
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
	}
	public void OnLevelComplete(int floorIndex) {
		opened = true;
		animator.SetTrigger ("open");
		portalCollider.enabled = true;
	}
	
	public void OnLevelEnd(int floorIndex) {
		Destroy ();
	}
	public override void TerminateObjectFunctions () {
		opened = false;
		UnsubscribeFromEvents ();
	}
	private void OnTriggerEnter2D (Collider2D collision) {
		if (collision.tag == "Player1") {
			GauntletGameManager.instance.PortalEntered ();
			portalCollider.enabled = false;
		}
	}
}
