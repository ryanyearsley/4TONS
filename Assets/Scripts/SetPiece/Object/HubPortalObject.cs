using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;


//virtually the same as a spellobject, except there is no life timer on these.
public class HubPortalObject : PoolObject {

	public SpellSchool school;

	public PortalStatus portalStatus;

	private Animator animator;
	private CircleCollider2D portalCollider;
	[SerializeField]
	private TextMeshProUGUI towerStatusText;

	private void Awake () {
	}

	public override void SetupObject () {
		animator = GetComponentInChildren<Animator> ();
		portalCollider = GetComponent<CircleCollider2D> ();
		portalCollider.enabled = false;
		portalStatus = PortalStatus.DISABLED;
		base.SetupObject ();
	}

	public override void ReuseObject () {
		base.ReuseObject ();
		bool completed = PlayerManager.instance.currentPlayers [0].wizardSaveData.CheckIfPlayerCompleteTower (school);
		
		if (completed) {
			SetPortalComplete ();
		} else {
			SetPortalOpen ();
		}
	}

	private void SetPortalOpen() {
		portalStatus = PortalStatus.OPEN;
		animator.SetTrigger ("open");
		towerStatusText.text = "OPEN";
		portalCollider.enabled = true;
	}
	private void SetPortalClosed () {
		portalStatus = PortalStatus.CLOSED;
		animator.SetTrigger ("close");
		towerStatusText.text = "CLOSED";
		portalCollider.enabled = false;
	}
	private void SetPortalComplete () {
		portalStatus = PortalStatus.COMPLETE;
		animator.SetTrigger ("complete");
		towerStatusText.text = "COMPLETED";
		portalCollider.enabled = false;
	}
	public override void TerminateObjectFunctions () {
		portalStatus = PortalStatus.DISABLED;
		portalCollider.enabled = false;
	}
	private void OnTriggerEnter2D (Collider2D collision) {
		if (collision.tag == "Player1") {
			GauntletHubGameManager.instance.TowerEntered (school);
			portalCollider.enabled = false;
		}
	}
}

public enum PortalStatus {
	CLOSED, OPEN, COMPLETE, DISABLED
}
