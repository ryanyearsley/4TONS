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
	public GameObject castObject;
	public int manaCost;
	public float castTime;
	public float castSpeedReduction;
	public float coolDown;
	public SpellCastLocation spellCastLocation;
	public AudioClip spellCastSound;
	public AudioClip spellChannelSound;
	public AudioClip spellEndSound;
	public GameObject spellObject;
	[SerializeField]
	public int poolSize;

	public Sprite spellPreviewSprite;

	public SpellObjectData spellObjectData;
	public PuzzlePieceData puzzlePieceData;

}
