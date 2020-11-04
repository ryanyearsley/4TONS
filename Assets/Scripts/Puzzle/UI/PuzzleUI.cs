using UnityEngine;
using UnityEngine.Tilemaps;


/*
 When the player enters puzzle state, this UI object is enabled/displayed over player's head.
-Contains Inventory display
-Contains Staff display
-Contains Display of active spells/passives
*/

public class PuzzleUI : MonoBehaviour {

	[SerializeField]
	private Tilemap inventoryTilemap;
	[SerializeField]
	private Tilemap staffTilemap;

	private Transform currentSpellGemTrans;


	public void BuildStaffUI (int [,] staffTileData) {
		StaffFactory.BuildStaff (staffTileData, staffTilemap, ConstantsManager.instance.staffTile);
	}

	public void FitSpellgem (GameObject spellGem) {
		GameObject spellGemGO = Instantiate (spellGem);
		spellGemGO.transform.parent = this.transform;
		spellGemGO.transform.localPosition = staffTilemap.CellToWorld (new Vector3Int (4, 4, 0));

	}

	public void EquipSpellGemFromLoad (SpellSaveData spellSaveData) {
		GameObject spellGemUIGo = Instantiate (ConstantsManager.instance.spellGemUIPrefab);
		//spellGemUIGo.GetComponent<SpellGemUI>()
		spellGemUIGo.transform.parent = staffTilemap.gameObject.transform.parent;
		Debug.Log ("Equipping spell: " + spellSaveData.spellData.spellName + " at staff position: " + spellSaveData.spellGemOriginCoordinate.ToString ());
		spellGemUIGo.transform.localPosition = staffTilemap.GetCellCenterWorld((Vector3Int)spellSaveData.spellGemOriginCoordinate);


	}
}
