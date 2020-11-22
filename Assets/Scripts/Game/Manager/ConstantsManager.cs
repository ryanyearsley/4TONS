using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class ConstantsManager : MonoBehaviour {
	//Camera details
	public Vector2Int pixelPerfectReferenceResolutionClose;
	public Vector2Int pixelPerfectReferenceResolutionMid;
	public Vector2Int pixelPerfectReferenceResolutionFar;

	//PLAYER CREATION PREFABS
	public GameObject playerWizardTemplatePrefab;
	public GameObject staffTemplatePrefab;
	public GameObject cursorTemplatePrefab;
	public GameObject InteractButtonPrefab;

	public List<SpellSchoolData> spellSchools = new List<SpellSchoolData>();
	public Dictionary<SpellSchool, SpellSchoolData> spellSchoolDictionary = new Dictionary<SpellSchool, SpellSchoolData>();

	//STAFF AND PUZZLE PREFABS
	public GameObject puzzleUIPrefab;
	public Tile staffTile;
	public GameObject spellGemPickupPrefab;
	public GameObject spellGemUIPrefab;

	[SerializeField]
	private Vector2Int defaultInventorySize;
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
		foreach (SpellSchoolData spellSchoolData in spellSchools) {
			spellSchoolDictionary.Add (spellSchoolData.spellSchool, spellSchoolData);
		}
	}
}
