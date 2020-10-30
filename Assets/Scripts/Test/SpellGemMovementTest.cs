using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpellGemMovementTest : MonoBehaviour
{

    public PuzzleUI spellgemUI;
    public GameObject spellGem;

	private void Start () {
		spellgemUI.FitSpellgem (spellGem);
	}
}
