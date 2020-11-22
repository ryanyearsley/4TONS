using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CursorController : MonoBehaviour {

	PlayerStateController playerStateController;

	[SerializeField]
	private Sprite combatCursor;
	[SerializeField]
	private Sprite puzzleBrowsingCursor;
	[SerializeField]
	private Sprite puzzleHoverCursor;
	[SerializeField]
	private Sprite puzzleHoldCursor;
	private SpriteRenderer spriteRenderer;
	private Transform cursorCenter;

	private void Awake () {
		spriteRenderer = GetComponentInChildren<SpriteRenderer> ();
	}

	public void InitializeComponent (Player player) {
		playerStateController = player.currentPlayerStateController;
		playerStateController.OnChangeStateEvent += OnChangeState;
	}

	private void OnDisable () {
		playerStateController.OnChangeStateEvent -= OnChangeState;
	}

	public void OnChangeState (PlayerState playerState) {
		switch (playerState) {
			case (PlayerState.COMBAT): {
					spriteRenderer.sprite = combatCursor; 
					break;
				}
			case (PlayerState.PUZZLE_BROWSING): {
					spriteRenderer.sprite = puzzleBrowsingCursor;
					break;
				}
			case (PlayerState.PUZZLE_MOVING_SPELLGEM): {
					spriteRenderer.sprite = puzzleHoldCursor;
					break;
			}
		}
	}

	private void LateUpdate () {

		if (cursorCenter != null) {
			Vector4 position = new Vector4 (cursorCenter.position.x, cursorCenter.position.y, 0f, 0f);
			spriteRenderer.material.SetVector ("CursorCenterPosition", position);

		}
	}

	public void SetCursorCenter (Transform cursorCenter) {
		this.cursorCenter = cursorCenter;
	}
}