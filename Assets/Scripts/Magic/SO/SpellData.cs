using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

[CreateAssetMenu (fileName = "Data", menuName = "ScriptableObjects/Wizardry/Spell Data", order = 2)]
public class SpellData : ScriptableObject
{
	//Informational
	public string spellName;
	public SpellSchool spellSchool;
	public SpellSchoolData spellSchoolData;
	//Every spell has a code. EXAMPLE: Lightning bolt: 101 (LIGHT:1) + (SPELL UNLOCK TIER: 01)
	public int id;
	public string description;
	//visual
	public Sprite icon;
	//combat
	public float manaCost;
	public float castTime;
	public float castSpeedReduction;
	public float coolDown;

	public GameObject castObject;
	[SerializeField]
	public int poolSize;
	public GameObject spellObject;
	public SpellCastLocation spellCastLocation;
	public Sound spellCastSound;
	public AudioClip spellEndSound;


	public PuzzlePieceData puzzlePieceData;

}
