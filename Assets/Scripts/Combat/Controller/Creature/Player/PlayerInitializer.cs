using PlayerManagement;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInitializer : MonoBehaviour
{

	private PlayerAiming playerAiming;
	private PlayerInputController playerInputController;
	private AnimationController animationController;
	private PlayerPuzzleController playerPuzzleController;

	private void Awake () {
		playerAiming = GetComponent<PlayerAiming> ();
		animationController = GetComponentInChildren<AnimationController> ();
		playerInputController = GetComponent<PlayerInputController> ();

	}

	private void Start () {
	}

	public void InitializePlayer (Player player) {
		playerAiming.InitializeComponent (player);
		animationController.InitializeComponent (player);
		playerInputController.InitializePlayerComponent (player);
	}

}
