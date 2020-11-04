using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent (typeof (Grid))]
public class LevelGenerationTest : MonoBehaviour
{
    [SerializeField]
    private TextAsset levelDataCsv;

    [SerializeField]
    private GameObject[] blockSet;

    [SerializeField]
    private int[,] levelData;


    private Grid grid;

	private void Awake () {
        grid = GetComponent<Grid> ();
	}
	void Start()
    {
        levelData = LevelFactory.DeserializeLevelFile(levelDataCsv);
        //LevelFactory.BuildLevel(levelData, blockSet, grid);   
    }
}
