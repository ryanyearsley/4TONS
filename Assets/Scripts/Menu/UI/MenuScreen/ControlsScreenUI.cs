using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ControlsScreenUI : AbstractScreenUI {

	public GameObject controlMappingElementPrefab;
	public ControlSchemeData defaultControls;
	public RectTransform controlMappingGroupRect;

	private ControlMappingElementUI moveControlUI;
	private ControlMappingElementUI aimControlUI;
	private ControlMappingElementUI dodgecontrolUI;
	private ControlMappingElementUI pauseControlUI;
	private ControlMappingElementUI spell1ControlUI;
	private ControlMappingElementUI spell2ControlUI;
	private ControlMappingElementUI spell3ControlUI;
	private ControlMappingElementUI spell4ControlUI;
	private ControlMappingElementUI togglePuzzleControlUI;
	private ControlMappingElementUI grabSpellGemControlUI;
	private ControlMappingElementUI rotateSpellGemControlUI;
	private ControlMappingElementUI flipSpellGemControlUI;

	protected override void Start () {
		base.Start ();
		CreateControlMappingElements ();
		DisplayControlData (defaultControls.controlData);
	}

	private void CreateControlMappingElements() {
		Debug.Log ("Creating control mapping ui elements");
		moveControlUI = Instantiate (controlMappingElementPrefab, controlMappingGroupRect).GetComponent<ControlMappingElementUI> ();
		aimControlUI = Instantiate (controlMappingElementPrefab, controlMappingGroupRect).GetComponent<ControlMappingElementUI> ();
		dodgecontrolUI = Instantiate (controlMappingElementPrefab, controlMappingGroupRect).GetComponent<ControlMappingElementUI> ();
		pauseControlUI = Instantiate (controlMappingElementPrefab, controlMappingGroupRect).GetComponent<ControlMappingElementUI> ();
		spell1ControlUI = Instantiate (controlMappingElementPrefab, controlMappingGroupRect).GetComponent<ControlMappingElementUI> ();
		spell2ControlUI = Instantiate (controlMappingElementPrefab, controlMappingGroupRect).GetComponent<ControlMappingElementUI> ();
		spell3ControlUI = Instantiate (controlMappingElementPrefab, controlMappingGroupRect).GetComponent<ControlMappingElementUI> ();
		spell4ControlUI = Instantiate (controlMappingElementPrefab, controlMappingGroupRect).GetComponent<ControlMappingElementUI> ();
		togglePuzzleControlUI = Instantiate (controlMappingElementPrefab, controlMappingGroupRect).GetComponent<ControlMappingElementUI> ();
		grabSpellGemControlUI = Instantiate (controlMappingElementPrefab, controlMappingGroupRect).GetComponent<ControlMappingElementUI> ();
		rotateSpellGemControlUI = Instantiate (controlMappingElementPrefab, controlMappingGroupRect).GetComponent<ControlMappingElementUI> ();
		flipSpellGemControlUI = Instantiate (controlMappingElementPrefab, controlMappingGroupRect).GetComponent<ControlMappingElementUI> ();

	}
	public void DisplayControlData (ControlData controlData) {
		moveControlUI.DisplayControlUI (controlData.moveControl);
		aimControlUI.DisplayControlUI (controlData.aimControl);
		dodgecontrolUI.DisplayControlUI (controlData.dodgeControl);
		pauseControlUI.DisplayControlUI (controlData.pauseControl);
		spell1ControlUI.DisplayControlUI (controlData.spell1Control);
		spell2ControlUI.DisplayControlUI (controlData.spell2Control);
		spell3ControlUI.DisplayControlUI (controlData.spell3Control);
		spell4ControlUI.DisplayControlUI (controlData.spell4Control);
		togglePuzzleControlUI.DisplayControlUI (controlData.toggleControl);
		rotateSpellGemControlUI.DisplayControlUI (controlData.rotateControl);
		grabSpellGemControlUI.DisplayControlUI (controlData.grabControl);
		flipSpellGemControlUI.DisplayControlUI (controlData.dropControl);
	}
}
