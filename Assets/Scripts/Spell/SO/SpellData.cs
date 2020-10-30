using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu (fileName = "Data", menuName = "ScriptableObjects/Wizardry/Spell Data", order = 2)]
public class SpellData : ScriptableObject
{
	public string spellName;
	public string description;
	public Sprite icon;
	public Sprite spellGemSprite;
	public SpellObjectData spellObjectData;

}
