using UnityEngine;


/*
 When the player enters puzzle state, this UI object is enabled/displayed over player's head.
-Contains Inventory display
-Contains Staff display
-Contains Display of active spells/passives
*/

public class PuzzleUI : MonoBehaviour
{
	[SerializeField]
	private Grid inventoryGrid;
	[SerializeField]
	private Grid staffGrid;

	private Transform currentSpellGemTrans;


	public void FitSpellgem(GameObject spellGem) {
		GameObject spellGemGO = Instantiate (spellGem);
		spellGemGO.transform.parent = staffGrid.transform;
		spellGemGO.transform.localPosition = staffGrid.GetCellCenterLocal (new Vector3Int (4, 4, 0));

	}
}
