﻿using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
using Rewired;

public class PlayerUI : MonoBehaviour {

	public Text playerName;
	public Image playerPortait;
	public SpellUI[] spellUIs;
	public StaffSlotUI primaryStaffSlot;
	public StaffSlotUI secondaryStaffSlot;

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

		IEnumerable<ControllerMap> keyboardMaps = rewiredController.controllers.maps.GetMapsInCategory(ControllerType.Keyboard, 0, 2);
		IEnumerable<ControllerMap> mouseMaps = rewiredController.controllers.maps.GetMapsInCategory(ControllerType.Mouse, 0, 2);
		kbmBindingDictionary = new Dictionary<string, string>();
		kbmBindingDictionary.Add ("Spell0", null);
		kbmBindingDictionary.Add ("Spell1", null);
		kbmBindingDictionary.Add ("Spell2", null);
		kbmBindingDictionary.Add ("Spell3", null);


		foreach (ControllerMap map in keyboardMaps) {
			foreach (ActionElementMap actionElementMap in map.ButtonMaps) {
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
			foreach (ActionElementMap actionElementMap in map.ButtonMaps) {
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
		IEnumerable<ControllerMap> controllerMaps = rewiredController.controllers.maps.GetMapsInCategory(ControllerType.Joystick, 0, 2);
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


		playerName.text = player.wizardSaveData.wizardName;
		playerName.color = player.wizardSaveData.spellSchoolData.schoolColor;
		playerPortait.sprite = player.wizardSaveData.spellSchoolData.wizardData.portrait;
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
}
