using UnityEngine;
using UnityEngine.UI;

public class PlayerUI : MonoBehaviour {
	public Text playerName;
	public Image playerPortait;
	public SpellUI[] spellUIs;
	public StaffSlotUI primaryStaffSlot;
	public StaffSlotUI secondaryStaffSlot;

	public void InitializePlayerUI (Player player) {
		playerName.text = player.wizardSaveData.wizardName;
		playerName.color = player.wizardSaveData.spellSchoolData.schoolGemColor;
		playerPortait.sprite = player.wizardSaveData.spellSchoolData.portrait;
		if (player.wizardSaveData.primaryStaffSaveData.puzzleData != null)
			primaryStaffSlot.UpdateStaffSlotUI (player.wizardSaveData.primaryStaffSaveData.puzzleData);

		if (player.wizardSaveData.secondaryStaffSaveData.puzzleData != null)
			secondaryStaffSlot.UpdateStaffSlotUI (player.wizardSaveData.secondaryStaffSaveData.puzzleData);
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
