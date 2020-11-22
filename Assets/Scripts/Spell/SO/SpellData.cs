using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

[CreateAssetMenu (fileName = "Data", menuName = "ScriptableObjects/Wizardry/Spell Data", order = 2)]
public class SpellData : ScriptableObject
{
	//Informational
	public string spellName;

	//Every spell has a code. EXAMPLE: Lightning bolt: 101 (LIGHT:1) + (SPELL UNLOCK TIER: 01)
	public int spellCode;
	public string description;
	//visual
	public Sprite icon;
	public Sprite spellGemSprite;
	//combat
	public GameObject castObject;
	public int manaCost;
	public float castTime;
	public float castSpeedReduction;
	public float coolDown;
	public SpellCastLocation spellCastLocation;
	public GameObject spellObject;
	[SerializeField]
	public int poolSize;

	public SpellObjectData spellObjectData;
	//puzzle
	public Vector2Int[] coordinates;

}
