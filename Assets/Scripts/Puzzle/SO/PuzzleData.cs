﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu (fileName = "Data", menuName = "ScriptableObjects/Puzzle/Puzzle Data")]
public class PuzzleData : ScriptableObject
{
	public int id;
	public string puzzleName;
	public string description;
	public Sprite puzzleSprite;
	public Sprite puzzleIcon;
	public TextAsset puzzleFile;
	public PuzzleType puzzleType;
	public SpellGemSaveDataDictionary defaultSpellGemDictionary;

}
