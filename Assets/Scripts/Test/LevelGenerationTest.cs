using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelGenerationTest : MonoBehaviour
{
    [SerializeField]
    private TextAsset levelDataCsv;

    [SerializeField]
    private GameObject[] blockSet;

    [SerializeField]
    private int[,] levelData;

    void Start()
    {
        levelData = LevelFactory.DeserializeLevelFile(levelDataCsv);
        LevelFactory.BuildLevel(levelData, blockSet);   
    }
}
