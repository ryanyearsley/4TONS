using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

[CreateAssetMenu (fileName = "Data", menuName = "ScriptableObjects/Wizardry/Spell Data", order = 2)]
public class SpellData : ScriptableObject
{
	public string spellName;
	public string description;
	public Sprite icon;
	public Sprite spellGemSprite;
	public SpellObjectData spellObjectData;

	public Vector2Int[] coordinates;

}
