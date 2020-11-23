using PlayerManagement;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInitializer : MonoBehaviour
{
	private PlayerStateController playerStateController;
	private PlayerAimingController playerAimingController;
	private PlayerInputController playerInputController;
	private AnimationController animationController;
	private PlayerPuzzleController playerPuzzleController;
	private PlayerSpellController spellController;

	private void Awake () {
		playerStateController = GetComponent<PlayerStateController> ();
		playerAimingController = GetComponent<PlayerAimingController> ();
		animationController = GetComponentInChildren<AnimationController> ();
		playerInputController = GetComponent<PlayerInputController> ();
		playerPuzzleController = GetComponent<PlayerPuzzleController> ();
		spellController = GetComponent<PlayerSpellController> ();
	}

	public void InitializePlayer (Player player) {
		playerAimingController.InitializeComponent (player);
		animationController.InitializeComponent (player);
		playerInputController.InitializeComponent (player);
		playerPuzzleController.InitializeComponent (player);
		playerStateController.InitializeComponent (player);
		//Spell controller initialized through puzzle controller (serves as validation step)
	}

}
