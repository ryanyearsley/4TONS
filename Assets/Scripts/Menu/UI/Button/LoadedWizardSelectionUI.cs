
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

    public void DisplayWizardUI (WizardSaveData wizardData) {
        Debug.Log ("displaying " + wizardData.wizardName);
        nameText.text = wizardData.wizardName;
        wizardPortrait.sprite = wizardData.spellSchoolData.wizardSelectIcon;
        wizardPortrait.color = Color.white;
        wizardSaveData = wizardData;
    }
    public void BlankOutFields() {
        nameText.text = " - - - - ";
        wizardPortrait.sprite = null;
        wizardPortrait.color = new Color (0, 0, 0, 0);
        wizardSaveData = null;
    }
    

}
