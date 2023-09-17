using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using System.Collections.Generic;
using System.Collections;

public class WizardCreatePanelUI : MonoBehaviour
{

	[SerializeField]
	private TMP_InputField wizardNameInputField;
	[SerializeField]
	private Image wizardSelectImage;
	[SerializeField]
	private Image wizardStaffImage;
	[SerializeField]
	private Image wizardSpellImage;
	[SerializeField]
	private TextMeshProUGUI errorMessageText;

	private int maxNameLength = 15;
	SpellSchoolData currentSchool;
	[SerializeField]
	private SpellSchoolData defaultSchool;

	private void Start () {
		DisplayWizardInfo (defaultSchool);
		SelectRandomName();
	}

	public void SelectRandomName() {
		RandomNameData rnd = ConstantsManager.instance.randomWizardNames;
		int randomIndex = UnityEngine.Random.Range (1, rnd.names.Count);
		wizardNameInputField.text = rnd.names [randomIndex];
	}

	public void DisplayWizardInfo (SpellSchoolData schoolData) {
		Debug.Log ("Displaying wizard info. Wizard name: " + schoolData.spellSchool);
		currentSchool = schoolData;

		//on-screen...
		wizardSelectImage.sprite = currentSchool.wizardData.wizardSelectIcon;
		WizardSaveData gauntletDefaultData = currentSchool.wizardData.gauntletStartData.wizardSaveData;
		wizardStaffImage.sprite = gauntletDefaultData.primaryStaffSaveData.puzzleData.puzzleIcon;
		SpellGemSaveData primaryGemSaveData = null;
		foreach (SpellGemSaveData gemData in gauntletDefaultData.primaryStaffSaveData.spellGemSaveDataDictionary.Values) {
			primaryGemSaveData = gemData;
		}
		if (primaryGemSaveData != null) {
			wizardSpellImage.sprite = primaryGemSaveData.spellData.icon;
		}

	}
	public bool isValidWizard () {
		StopAllCoroutines ();
		errorMessageText.text = String.Empty;
		if (currentSchool == null) {
			AudioManager.instance.PlaySound ("Error");
			StartCoroutine (DisplayWizardCreateErrorMessage ("Choose a wizard."));
			return false;
		}
		else if (wizardNameInputField.text.Length > maxNameLength) {
			AudioManager.instance.PlaySound ("Error");
			StartCoroutine (DisplayWizardCreateErrorMessage ("That name is too long."));
			return false;
		}
		else if (!WizardSaveDataManager.instance.isWizardNameAvailable (wizardNameInputField.text)) {

			AudioManager.instance.PlaySound ("Error");
			StartCoroutine (DisplayWizardCreateErrorMessage ("That name is taken."));

			return false;
		} 
		else if (wizardNameInputField.text == "") {
			AudioManager.instance.PlaySound ("Error");
			StartCoroutine (DisplayWizardCreateErrorMessage ("Enter a name."));
			return false;
		}
		else return true;
	}
	public IEnumerator DisplayWizardCreateErrorMessage(string message) {
		errorMessageText.text = message;
		yield return new WaitForSeconds (3);
		errorMessageText.text = String.Empty;
	}
	public WizardSaveData FinalizeWizardCreate () {
		WizardSaveData newWizard = currentSchool.wizardPrebuildData.wizardSaveData.Clone ();
		newWizard.wizardName = wizardNameInputField.text;
		return newWizard;
	}
}
