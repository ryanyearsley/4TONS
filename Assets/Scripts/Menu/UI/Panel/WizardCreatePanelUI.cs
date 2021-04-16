using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using System.Collections.Generic;

public class WizardCreatePanelUI : MonoBehaviour
{

	[SerializeField]
	private TextMeshProUGUI schoolNameText;
	[SerializeField]
	private TextMeshProUGUI wizardDescriptionText;
	[SerializeField]
	private TMP_InputField wizardNameInputField;
	[SerializeField]
	private Image wizardSelectImage;
	SpellSchoolData currentSchool;

	private void Start () {
		DisplayWizardInfo (ConstantsManager.instance.spellSchools[0]);
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
		schoolNameText.text = currentSchool.spellSchool.ToString ();
		wizardDescriptionText.text = currentSchool.description;
		wizardSelectImage.sprite = currentSchool.wizardSelectIcon;


	}
	public bool isValidWizard () {
		if (wizardNameInputField.text == "" || currentSchool == null || !SaveManager.instance.isWizardNameAvailable (wizardNameInputField.text))
			return false;
		else return true;
	}
	public WizardSaveData FinalizeWizardCreate () {
		WizardSaveData newWizard = currentSchool.defaultWizard.wizardSaveData.Clone ();
		newWizard.wizardName = wizardNameInputField.text;
		return newWizard;
	}
}
