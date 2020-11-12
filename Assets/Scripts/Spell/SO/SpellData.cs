using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

[CreateAssetMenu (fileName = "Data", menuName = "ScriptableObjects/Wizardry/Spell Data", order = 2)]
public class SpellData : ScriptableObject
{
	//Informational
	public string spellName;
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
	public Vector3Int[] coordinates;

}
