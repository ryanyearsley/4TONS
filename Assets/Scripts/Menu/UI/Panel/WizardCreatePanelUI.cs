using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

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
		schoolNameText.text = currentSchool.school.ToString ();
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
		wizard.spellSchoolDataPath = AssetDatabase.GetAssetPath (currentSchool);
		return wizard;
	}
}
