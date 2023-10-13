using UnityEngine;
using System;
using System.Collections.Generic;
using UnityEngine.UI;
using Rewired;
using TMPro;

public class PlayerUI : MonoBehaviour {

	public SpellUI[] spellUIs;
	public StaffSlotUI primaryStaffSlot;
	public StaffSlotUI secondaryStaffSlot;

	[SerializeField]
	private Sprite puzzlePhaseSprite;
	[SerializeField]
	private Sprite combatPhaseSprite;
	[SerializeField]
	private Image stateImage;

	[SerializeField]
	private TMP_Text changeStateBind;

	[SerializeField]
	private Image dodgeImage;
	[SerializeField]
	private TMP_Text dodgeBindText;
	// Start is called before the first frame update

	private Dictionary<string, string> currentBindingDictionary;

	private Dictionary<string, string> kbmBindingDictionary = new Dictionary<string, string>();

	private Dictionary<string, string> controllerBindingDictionary = new Dictionary<string, string>();

	public void InitializePlayerUI (Player player) {

		Rewired.Player rewiredController = ReInput.players.GetPlayer (player.controllerIndex);
		//category ID: 2
		Dictionary<string, string> bindAbbreviationDictionary = new Dictionary<string, string>();
		bindAbbreviationDictionary.Add ("Left Mouse Button", "LMB");
		bindAbbreviationDictionary.Add ("Right Mouse Button", "RMB");
		bindAbbreviationDictionary.Add ("Mouse Button 4", "MB4");
		bindAbbreviationDictionary.Add ("Mouse Button 5", "MB5");


		bindAbbreviationDictionary.Add ("Right Trigger", "RT");
		bindAbbreviationDictionary.Add ("Left Trigger", "LT");
		bindAbbreviationDictionary.Add ("Right Shoulder", "RB");
		bindAbbreviationDictionary.Add ("Left Shoulder", "LB");
		bindAbbreviationDictionary.Add("Left Stick Button", "LSB");
		bindAbbreviationDictionary.Add("Right Stick Button", "RSB");

		IEnumerable<ControllerMap> keyboardMaps = rewiredController.controllers.maps.GetMaps(ControllerType.Keyboard, 0);
		IEnumerable<ControllerMap> mouseMaps = rewiredController.controllers.maps.GetMaps(ControllerType.Mouse, 0);
		kbmBindingDictionary = new Dictionary<string, string>();
		kbmBindingDictionary.Add("MoveUp", null);
		kbmBindingDictionary.Add("MoveDown", null);
		kbmBindingDictionary.Add("MoveLeft", null);
		kbmBindingDictionary.Add("MoveRight", null);
		kbmBindingDictionary.Add ("Spell0", null);
		kbmBindingDictionary.Add ("Spell1", null);
		kbmBindingDictionary.Add ("Spell2", null);
		kbmBindingDictionary.Add ("Spell3", null);
		kbmBindingDictionary.Add ("Dash", null);
		kbmBindingDictionary.Add ("GrabItem", null);
		kbmBindingDictionary.Add ("TogglePuzzle", null);
		kbmBindingDictionary.Add ("AutoBindItem", null);
		kbmBindingDictionary.Add("SwitchToPrimaryStaff", null);
		kbmBindingDictionary.Add("SwitchToSecondaryStaff", null);
		kbmBindingDictionary.Add("SwitchToAlternateStaff", null);
		kbmBindingDictionary.Add("RotateItemCCW", null);
		kbmBindingDictionary.Add("RotateItemCW", null);
		kbmBindingDictionary.Add("DropItem", null);


		foreach (ControllerMap map in keyboardMaps) {
			foreach (ActionElementMap actionElementMap in map.AllMaps) {
				if (kbmBindingDictionary.ContainsKey (actionElementMap.actionDescriptiveName)) {
					Debug.Log ("PlayerUI: Found keyboard binding. Setting bind string. Action: " + actionElementMap.actionDescriptiveName + ", Binding: " + actionElementMap.elementIdentifierName);
					string bindString = actionElementMap.elementIdentifierName;
					if (bindAbbreviationDictionary.ContainsKey (bindString)) {
						bindString = bindAbbreviationDictionary [bindString];
					}
					kbmBindingDictionary [actionElementMap.actionDescriptiveName] = bindString;
				}
			}
		}

		foreach (ControllerMap map in mouseMaps) {
			foreach (ActionElementMap actionElementMap in map.AllMaps) {
				if (kbmBindingDictionary.ContainsKey (actionElementMap.actionDescriptiveName)) {
					Debug.Log ("PlayerUI: Found mouse binding. Setting bind string. Action: " + actionElementMap.actionDescriptiveName + ", Binding: " + actionElementMap.elementIdentifierName);
					string bindString = actionElementMap.elementIdentifierName;
					if (bindAbbreviationDictionary.ContainsKey (bindString)) {
						bindString = bindAbbreviationDictionary [bindString];
					}
					kbmBindingDictionary [actionElementMap.actionDescriptiveName] = bindString;
				}
			}
		}

		UpdateBindings (ControllerType.Mouse);


		controllerBindingDictionary = new Dictionary<string, string>();
		controllerBindingDictionary.Add ("Spell0", null);
		controllerBindingDictionary.Add ("Spell1", null);
		controllerBindingDictionary.Add ("Spell2", null);
		controllerBindingDictionary.Add ("Spell3", null);
		controllerBindingDictionary.Add("Dash", null);
		controllerBindingDictionary.Add("GrabItem", null);
		controllerBindingDictionary.Add("TogglePuzzle", null);
		controllerBindingDictionary.Add("AutoBindItem", null);
		controllerBindingDictionary.Add("SwitchToPrimaryStaff", null);
		controllerBindingDictionary.Add("SwitchToSecondaryStaff", null);
		controllerBindingDictionary.Add("SwitchToAlternateStaff", null);
		controllerBindingDictionary.Add("RotateItemCCW", null);
		controllerBindingDictionary.Add("RotateItemCW", null);
		controllerBindingDictionary.Add("DropItem", null);
		IEnumerable<ControllerMap> controllerMaps = rewiredController.controllers.maps.GetMaps(ControllerType.Joystick, 0);
		foreach (ControllerMap map in controllerMaps) {
			foreach (ActionElementMap actionElementMap in map.AllMaps) {
				Debug.Log ("PlayerUI: checking controller action: " + actionElementMap.actionDescriptiveName +", binding: " + actionElementMap.elementIdentifierName);

				if (controllerBindingDictionary.ContainsKey (actionElementMap.actionDescriptiveName)) {
					Debug.Log ("PlayerUI: Found controller binding. Setting bind string. Action: " + actionElementMap.actionDescriptiveName + ", Binding: " + actionElementMap.elementIdentifierName);
					string bindString = actionElementMap.elementIdentifierName;
					if (bindAbbreviationDictionary.ContainsKey(bindString)) {
						bindString = bindAbbreviationDictionary [bindString];
					}
					controllerBindingDictionary [actionElementMap.actionDescriptiveName] = bindString;
				}
			}
		}

		//playerName.color = player.wizardSaveData.spellSchoolData.schoolColor;
		if (player.wizardSaveData.primaryStaffSaveData.puzzleData != null)
			primaryStaffSlot.UpdateStaffSlotUI (player.wizardSaveData.primaryStaffSaveData.puzzleData);

		if (player.wizardSaveData.secondaryStaffSaveData.puzzleData != null)
			secondaryStaffSlot.UpdateStaffSlotUI (player.wizardSaveData.secondaryStaffSaveData.puzzleData);
	}
	
	public void UpdateBindings(ControllerType controllerType) {
		Debug.Log ("PlayerUI: Updating Bindings to " + controllerType.ToString ());
		if (controllerType == ControllerType.Mouse) {
			currentBindingDictionary = kbmBindingDictionary;
		} else if (controllerType == ControllerType.Joystick) {
			currentBindingDictionary = controllerBindingDictionary;
		}
		for (int i = 0; i < spellUIs.Length; i++) {
			string actionString = "Spell" + i;
			if (currentBindingDictionary.ContainsKey(actionString)) {
				spellUIs [i].UpdateBindingString (currentBindingDictionary[actionString]);
			}
		}
		if (currentBindingDictionary.ContainsKey("Dash"))
		{
			dodgeBindText.text = currentBindingDictionary["Dash"];
		}
		if (currentBindingDictionary.ContainsKey("TogglePuzzle"))
		{
			changeStateBind.text = currentBindingDictionary["TogglePuzzle"];
		}
	}

	public string GetCurrentBinding (string rewiredAction)
	{
		if (currentBindingDictionary.TryGetValue(rewiredAction, out string binding))
		{
			return binding;
		}
		else return rewiredAction;
	}

	public void UpdatePlayerState(PlayerState state)
	{  
		switch (state)
		{
			case (PlayerState.COMBAT):
				{
					stateImage.sprite = puzzlePhaseSprite;
					break;
				}
			case (PlayerState.PUZZLE_BROWSING):
			case (PlayerState.PUZZLE_MOVING_SPELLGEM):
				{
					stateImage.sprite = combatPhaseSprite;
					break;
				}
		}
	}

	public void OnPickUpStaff (PuzzleKey region, PuzzleGameData puzzleGameData) {

		if (region == PuzzleKey.PRIMARY_STAFF) {
			primaryStaffSlot.UpdateStaffSlotUI (puzzleGameData.puzzleData);
		} else if (region == PuzzleKey.SECONDARY_STAFF) {
			secondaryStaffSlot.UpdateStaffSlotUI (puzzleGameData.puzzleData);
		}

	}
	public void OnDropStaff (PuzzleKey region) {

		if (region == PuzzleKey.PRIMARY_STAFF) {
			primaryStaffSlot.OnDropStaff ();
		} else if (region == PuzzleKey.SECONDARY_STAFF) {
			secondaryStaffSlot.OnDropStaff ();
		}
	}

	public void OnEquipStaff (PuzzleKey region) {

		if (region == PuzzleKey.PRIMARY_STAFF) {
			primaryStaffSlot.OnEquipStaff ();
			secondaryStaffSlot.OnUnequipStaff ();
		} else if (region == PuzzleKey.SECONDARY_STAFF) {
			secondaryStaffSlot.OnEquipStaff ();
			primaryStaffSlot.OnUnequipStaff ();
		}
	}
	public void UpdateDodgeUICooldown(float cdTimer, float cooldown)
	{
		float fillPercentage = cdTimer / cooldown;
		dodgeImage.fillAmount = fillPercentage;
		dodgeBindText.text = Math.Round(cooldown - cdTimer, 1).ToString();
	}

	public void ClearDodgeUI()
	{
		dodgeImage.fillAmount = 1;
		if (currentBindingDictionary.ContainsKey("Dash"))
		{
			dodgeBindText.text = currentBindingDictionary["Dash"];
		}
	}
}
