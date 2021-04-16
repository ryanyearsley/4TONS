using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PuzzleManager : MonoBehaviour {
	public static PuzzleManager Instance { get; private set; }

	public PuzzleDictionary puzzleDictionary = new PuzzleDictionary();

	private void Awake () {
		Instance = this;
	}

	public void RegisterPuzzleEntity (PuzzleEntity puzzleEntity) {
		int key = puzzleEntity.puzzleObjectTrans.GetInstanceID();
		if (!puzzleDictionary.ContainsKey (key)) {
			puzzleDictionary.Add (key, puzzleEntity);
		} else {
			Debug.Log ("cannot register puzzle. Puzzle already exists for that instance ID");
		}
	}
	public void DeregisterPuzzleEntity (PuzzleEntity puzzleEntity) {
		int key = puzzleEntity.puzzleObjectTrans.GetInstanceID();
		if (!puzzleDictionary.ContainsKey (key)) {
			puzzleDictionary.Add (key, puzzleEntity);
		} else {
			Debug.Log ("cannot register puzzle. Puzzle already exists for that instance ID");
		}
	}

	public void AddSpellGemToPuzzle(PuzzleEntity puzzleEntity, SpellGemSaveData spellGemSaveData) {

	}


} 
