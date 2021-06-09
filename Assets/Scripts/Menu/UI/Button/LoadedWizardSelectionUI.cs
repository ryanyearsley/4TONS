
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEditor;

public class LoadedWizardSelectionUI : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI nameText;
    [SerializeField]
    private Image wizardPortrait;
    public WizardSaveData wizardSaveData { get; private set; }

    public void DisplayWizardUI (WizardSaveData wizardSaveData) {
        Debug.Log ("displaying " + wizardSaveData.wizardName);
        nameText.text = wizardSaveData.wizardName;
        wizardPortrait.sprite = wizardSaveData.spellSchoolData.wizardData.wizardSelectIcon;
        wizardPortrait.color = Color.white;
        this.wizardSaveData = wizardSaveData;
    }
    public void BlankOutFields() {
        nameText.text = " - - - - ";
        wizardPortrait.sprite = null;
        wizardPortrait.color = new Color (0, 0, 0, 0);
        wizardSaveData = null;
    }
    

}
