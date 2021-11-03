using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;


public class HubPortalObject : InteractableObject {

	public Zone zone;
	public PortalStatus portalStatus;
	private InteractableCollider portalCollider;
	[SerializeField]
	private TextMeshProUGUI towerStatusText;

	[SerializeField]
	private GameObject spriteObject;

	public override void SetupObject () {
		base.SetupObject ();
		portalCollider = GetComponentInChildren<InteractableCollider> ();
		portalCollider.enabled = false;
		portalStatus = PortalStatus.DISABLED;
	}
	public override void ReuseObject () {
		bool completed = PlayerManager.instance.currentPlayers [0].wizardSaveData.CheckIfPlayerCompleteTower (zone);

		if (completed) {
			SetPortalComplete ();
		} else {
			SetPortalOpen ();
		}
	}

	public override void TerminateObjectFunctions () {
		portalStatus = PortalStatus.DISABLED;
		portalCollider.enabled = false;
	}

	public override void InteractWithObject () {
		GauntletHubGameManager.instance.TowerEntered (zone);
		portalCollider.enabled = false;
	}

	private void SetPortalOpen () {
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
}

public enum PortalStatus {
	CLOSED, OPEN, COMPLETE, DISABLED
}
