using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu (fileName = "Data", menuName = "ScriptableObjects/Wizardry/Staff Data", order = 1)]
public class StaffData : ScriptableObject
{
	public string staffName;
	public string description;
	public Sprite staffSprite;
	public TextAsset staffFile;

}
