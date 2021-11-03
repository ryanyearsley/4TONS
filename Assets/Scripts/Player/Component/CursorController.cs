using TMPro;
using UnityEngine;

public class CursorController : MonoBehaviour {


	PlayerObject playerObject;

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
		playerObject = player.currentPlayerObject;
		spriteRenderer.color = player.wizardSaveData.spellSchoolData.schoolColor;
	}

	public void OnChangeState (PlayerState playerState) {
		Debug.Log ("CursorController: Changing player state to " + playerState);
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