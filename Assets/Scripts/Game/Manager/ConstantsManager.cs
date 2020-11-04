using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class ConstantsManager : MonoBehaviour {

	//PLAYER CREATION PREFABS
	public GameObject playerWizardTemplatePrefab;
	public GameObject staffTemplatePrefab;
	public GameObject cursorTemplatePrefab;

	public List<SpellSchoolData> spellSchools = new List<SpellSchoolData>();

	public Vector2Int pixelPerfectReferenceResolutionClose;
	public Vector2Int pixelPerfectReferenceResolutionMid;
	public Vector2Int pixelPerfectReferenceResolutionFar;

	//STAFF AND PUZZLE PREFABS
	public GameObject inventoryPuzzleUIPrefab;
	public Tile staffTile;
	public GameObject spellGemPickupPrefab;
	public GameObject spellGemUIPrefab;

	// Singleton Pattern to access this script with ease
	#region Singleton
	public static ConstantsManager instance;
	void SingletonInitialization() {
		if (instance == null) {
			instance = this;
		} else if (instance != this) {
			Destroy (gameObject);
		}
	}
	#endregion
	void Awake () {
		SingletonInitialization ();
	}
}
