using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ControlsPanelUI : MonoBehaviour
{
	
	[SerializeField]
	private Text controlsText;

	private int currentControlsTextIndex;
	private List<string> controlsTexts = new List<string>();

	[SerializeField]
	private string disabledText;
	[TextArea(15,20)]
	[SerializeField]
	private string combatControlsText;
	[TextArea(15,20)]
	[SerializeField]
	private string puzzleControlsText;

	private void Start () {
		controlsTexts.Add (disabledText);
		controlsTexts.Add (combatControlsText);
		controlsTexts.Add (puzzleControlsText);
		controlsText.text = disabledText;
		currentControlsTextIndex = 0;
	}
	void Update () {
		if (Input.GetKeyDown (KeyCode.C)) {
			CycleControlsUI ();
		}
	}
	private void CycleControlsUI () {
		if (currentControlsTextIndex >= controlsTexts.Count - 1)
			currentControlsTextIndex = 0;
		else
			currentControlsTextIndex++;
		controlsText.text = controlsTexts [currentControlsTextIndex];
	}
}
