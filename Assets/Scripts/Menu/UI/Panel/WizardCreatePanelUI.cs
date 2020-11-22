using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
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

	private void OnEnable () {
		DisplayWizardInfo (ConstantsManager.instance.spellSchools[0]);
	}

	public void DisplayWizardInfo (SpellSchoolData schoolData) {
		currentSchool = schoolData;
		schoolNameText.text = currentSchool.spellSchool.ToString ();
		wizardDescriptionText.text = currentSchool.description;
		wizardSelectImage.sprite = currentSchool.wizardSelectIcon;
	}
	public bool isValidWizard () {
		if (wizardNameInputField.text == "" || currentSchool == null)
			return false;
		return true;
	}
	public WizardSaveData FinalizeWizard () {
		WizardSaveData wizard = new WizardSaveData();
		wizard.wizardName = wizardNameInputField.text;
		wizard.spellSchoolData = currentSchool;
		Debug.Log (Application.persistentDataPath + AssetDatabase.GetAssetPath (currentSchool));
		wizard.spellSchoolDataPath = AssetDatabase.GetAssetPath (currentSchool);
		wizard.primaryStaffSaveData = new StaffSaveData();
		wizard.primaryStaffSaveData.staffData = currentSchool.staff;

		wizard.secondaryStaffSaveData = new StaffSaveData ();
		wizard.inventorySaveDataDictionary = new PuzzleSaveDataDictionary ();
		wizard.primaryStaffSaveData.staffPath = AssetDatabase.GetAssetPath(currentSchool.staff);
		return wizard;
	}
}
