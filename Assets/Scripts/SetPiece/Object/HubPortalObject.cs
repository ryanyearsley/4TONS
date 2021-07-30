using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;


//virtually the same as a spellobject, except there is no life timer on these.
public class HubPortalObject : PoolObject {

	public SpellSchool school;
	public PortalStatus portalStatus;
	private Collider2D portalCollider;
	[SerializeField]
	private TextMeshProUGUI towerStatusText;

	[SerializeField]
	private GameObject spriteObject;

	private void Awake () {
	}

	public override void SetupObject () {
		portalCollider = GetComponent<Collider2D> ();
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
		towerStatusText.text = "OPEN";
		portalCollider.enabled = true;
		spriteObject.SetActive (true);
	}
	private void SetPortalClosed () {
		portalStatus = PortalStatus.CLOSED;
		towerStatusText.text = "CLOSED";
		portalCollider.enabled = false;
		spriteObject.SetActive (false);
	}
	private void SetPortalComplete () {
		portalStatus = PortalStatus.COMPLETE;
		towerStatusText.text = "COMPLETED";
		portalCollider.enabled = false;
		spriteObject.SetActive (false);
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
