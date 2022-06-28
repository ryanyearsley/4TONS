using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StaffSlotUI : MonoBehaviour {
	[SerializeField]
	private Image staffImage;

	private Sprite emptyStaffSprite;
	void Awake() {
		emptyStaffSprite = staffImage.sprite;
	}
	public void UpdateStaffSlotUI (PuzzleData puzzleData) {
		staffImage.sprite = puzzleData.puzzleIcon;
	}
	public void OnDropStaff () {
		staffImage.sprite = emptyStaffSprite;
		staffImage.color = new Color (1, 1, 1, 1f);
	}

	public void OnUnequipStaff () {
		staffImage.color = new Color (1, 1, 1, 0.5f);
	}
	public void OnEquipStaff() {
		staffImage.color = new Color (1, 1, 1, 1f);
	}
}
