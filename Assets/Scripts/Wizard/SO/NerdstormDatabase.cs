using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu (fileName = "WizardSaveDatabase", menuName = "ScriptableObjects/Editor/WizardSaveDatabase", order = 1)]
public class NerdstormDatabase : ScriptableObject {
	private bool initialized = false;
	[SerializeField]
	private PuzzleData inventoryData;

	[SerializeField]
	private SpellSchoolData[] spellSchoolDatas;




	
}
