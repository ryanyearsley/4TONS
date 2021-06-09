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

	[SerializeField]
	private TMP_Text tooltipText;

	private bool pointingRight;

	private void Awake () {
		spriteRenderer = GetComponentInChildren<SpriteRenderer> ();
		ClearToolTip ();
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
					ClearToolTip ();
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

	public void UpdateToolTipText(string text) {
		tooltipText.text = text;
	}
	public void UpdateToolTipOrientation () {
		if (transform.position.x > playerObject.transform.position.x) {
			SetToolTipPointingRight ();
		} else  {
			SetToolTipPointingLeft ();
		}
	}
	private void SetToolTipPointingRight() {
		if (!pointingRight) {
			pointingRight = true;
			Debug.Log ("CursorController: Displaying ToolTip Pointing Right");
			tooltipText.rectTransform.localEulerAngles = new Vector3 (0, 0, 0);
			tooltipText.alignment = TextAlignmentOptions.MidlineRight;
		}
	}
	private void SetToolTipPointingLeft () {
		if (pointingRight) {
			pointingRight = false;
			Debug.Log ("CursorController: Displaying ToolTip Pointing Left");
			tooltipText.rectTransform.localEulerAngles = new Vector3 (0, 0, 180);
			tooltipText.alignment = TextAlignmentOptions.MidlineLeft;
		}
	}
	public void ClearToolTip () {
		tooltipText.text = "";
	}
}