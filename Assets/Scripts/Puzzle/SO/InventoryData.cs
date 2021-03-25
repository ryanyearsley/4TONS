using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu (fileName = "Data", menuName = "ScriptableObjects/Wizardry/Inventory Data", order = 1)]
public class InventoryData : ScriptableObject
{
	public string inventoryName;
	public string description;
	public Sprite inventorySprite;
	public TextAsset inventoryFile;
}
