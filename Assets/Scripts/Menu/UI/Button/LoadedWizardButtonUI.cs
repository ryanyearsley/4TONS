
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEditor;

public class LoadedWizardButtonUI : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI nameText;
    [SerializeField]
    private Image wizardPortrait;
    public WizardSaveData wizardSaveData { get; private set; }

    public void DisplayWizardUI (WizardSaveData wizardData) {
        Debug.Log ("displaying " + wizardData.wizardName);
        nameText.text = wizardData.wizardName;
        SpellSchoolData spellSchoolData =  (SpellSchoolData) AssetDatabase.LoadAssetAtPath (
            wizardData.spellSchoolDataPath, typeof(SpellSchoolData));
		wizardPortrait.sprite = spellSchoolData.wizardSelectIcon;
        wizardPortrait.color = Color.white;
        wizardSaveData = wizardData;
    }
    public void BlankOutFields() {
        nameText.text = "CREATE NEW";
        wizardPortrait.sprite = null;
        wizardPortrait.color = new Color (0, 0, 0, 0);
        wizardSaveData = null;
    }
    
    public bool IsVacant () {
        if (wizardSaveData == null) {
            return true;
        } else return false;
	}

}
