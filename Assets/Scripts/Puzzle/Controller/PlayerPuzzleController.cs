using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Rewired;


/*
This class...
-Initializes player's spellgem system
    
    
1. Reads player input
2. Moves and binds spellgems accordingly
*/

public enum PuzzlePhase {
    BROWSING_INVENTORY, FITTING_SPELLGEM
}
[RequireComponent(typeof(Grid))]
public class PlayerPuzzleController : MonoBehaviour
{
    private PlayerStateController playerStateController;

    private StaffData staffData;

    private List<SpellSaveData> inventorySpells;

    private PuzzleUI puzzleUI;

    private int[,] staff;

    public void InitializeComponent (Player player) {
        playerStateController = GetComponentInParent<PlayerStateController> ();
        Debug.Log ("Initializing player puzzle controller");
        GameObject go = Instantiate (ConstantsManager.instance.inventoryPuzzleUIPrefab);
        go.transform.parent = this.transform.root;
        puzzleUI = go.GetComponent<PuzzleUI> ();
        staff = StaffFactory.DeserializeStaffFile (player.currentWizard.primaryStaffSaveData.staffData.staffFile);
        go.SetActive (false);
        staffData = player.currentWizard.primaryStaffSaveData.staffData;
        puzzleUI.BuildStaffUI (staff);
        foreach (SpellSaveData spellSaveData in player.currentWizard.primaryStaffSaveData.equippedSpellsSaveData) {
            if (CheckSpellEquipEligibility (spellSaveData)) {
                EquipSpellFromLoad (spellSaveData);
			}

        }
    }

    public void AddSpellToInventory() {

	}

    public bool CheckSpellEquipEligibility(SpellSaveData spellSaveData) {
        bool canEquip = true;
        Vector2Int centerPoint = spellSaveData.spellGemOriginCoordinate;

        foreach (Vector2Int spellGemCoordinate in spellSaveData.spellData.coordinates) {
            Vector2Int relativePosition = centerPoint + spellGemCoordinate;
            if (staff[relativePosition.x, relativePosition.y] != 1) {
                canEquip = false;
			}
		}
        return canEquip;
	}
    public void EquipSpellFromLoad(SpellSaveData spellSaveData) {
        Vector2Int centerPoint = spellSaveData.spellGemOriginCoordinate;

        foreach (Vector2Int spellGemCoordinate in spellSaveData.spellData.coordinates) {
            Vector2Int relativePosition = centerPoint + spellGemCoordinate;
            if (staff [relativePosition.x, relativePosition.y] != 0) {
                staff [relativePosition.x, relativePosition.y] = 2;
            }
        }
        puzzleUI.EquipSpellGemFromLoad (spellSaveData);
    }

}
