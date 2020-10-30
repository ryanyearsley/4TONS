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
    DORMANT, 
}
public class PlayerSpellGemController : MonoBehaviour
{
    private Rewired.Player controller;

    private SpellGem currentSpellGem;

    private StaffData staffData;

    void Start() {
        controller = ReInput.players.GetPlayer (0);
    }

    void Update()
    {
        if (controller.GetButtonDown ("UIUp") ){

		}
    }

    private void BuildStaff (StaffData staffData) {
        //int [,] staffTiles = StaffFactory.DeserializeStaffFile (staffData.staffFile);
        //affFactory.BuildStaff (staffTiles, ConstantsManager.instance.staffTilePrefab, staffOrigin.position, grid);
    }
}
