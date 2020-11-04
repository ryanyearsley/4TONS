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
public class PlayerPuzzleController : MonoBehaviour
{
    private StaffData staffData;

    public void InitializeComponent (WizardSaveData wizardSaveData, Player player) {
        staffData = wizardSaveData.primaryStaffSaveData.staffData;
    }

    private void BuildStaff (StaffData staffData) {
        //int [,] staffTiles = StaffFactory.DeserializeStaffFile (staffData.staffFile);
        //affFactory.BuildStaff (staffTiles, ConstantsManager.instance.staffTilePrefab, staffOrigin.position, grid);
    }
}
