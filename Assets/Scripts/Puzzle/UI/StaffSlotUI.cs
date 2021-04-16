using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StaffSlotUI : MonoBehaviour {
	[SerializeField]
	private Sprite emptyStaffSlotSprite;
	[SerializeField]
	private Image staffImage;
	[SerializeField]
	private Image staffBackgroundImage;



	public void UpdateStaffSlotUI (PuzzleData puzzleData) {
		staffImage.sprite = puzzleData.puzzleIcon;
	}
	public void OnDropStaff () {
		staffImage.sprite = emptyStaffSlotSprite;
	}

	public void OnUnequipStaff () {
		staffBackgroundImage.enabled = false;
	}
	public void OnEquipStaff() {
		staffBackgroundImage.enabled = true;
	}
}
