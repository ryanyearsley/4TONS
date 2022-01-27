
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
    [SerializeField]
    private Image primaryStaffIcon;
    [SerializeField]
    private Image secondaryStaffIcon;
    [SerializeField]
    private Image[] primaryStaffSpellIcons;
    [SerializeField]
    private Image[] secondaryStaffSpellIcons;
    public WizardSaveData wizardSaveData { get; private set; }


    public void DisplayWizardUI (WizardSaveData wizardSaveData) {
        Debug.Log ("displaying " + wizardSaveData.wizardName);
        nameText.text = wizardSaveData.wizardName;
        wizardPortrait.sprite = wizardSaveData.spellSchoolData.wizardData.portrait;
        wizardPortrait.color = Color.white;
        if (wizardSaveData.primaryStaffSaveData.puzzleData != null) {
            UpdateStaffUI (wizardSaveData.primaryStaffSaveData, primaryStaffIcon, primaryStaffSpellIcons);
        }
        if (wizardSaveData.secondaryStaffSaveData.puzzleData != null) {
            UpdateStaffUI (wizardSaveData.secondaryStaffSaveData, secondaryStaffIcon, secondaryStaffSpellIcons);
        }
        this.wizardSaveData = wizardSaveData;
    }

    public void UpdateStaffUI(PuzzleSaveData staffData, Image staffIcon, Image[] spellIcons) {
        staffIcon.sprite = staffData.puzzleData.puzzleIcon;
        foreach (SpellGemSaveData spellGemSaveData in staffData.spellGemSaveDataDictionary.Values) {
            spellIcons [spellGemSaveData.spellBindIndex].sprite = spellGemSaveData.spellData.icon;
        }
    }
    public void BlankOutFields() {
        nameText.text = " - - - - ";
        wizardPortrait.sprite = null;
        wizardPortrait.color = new Color (0, 0, 0, 0);
        wizardSaveData = null;
    }
    

}
