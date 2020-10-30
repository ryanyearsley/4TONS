using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour {

	public MapGenerator mapGenerator;

	void Awake() {
		mapGenerator = FindObjectOfType<MapGenerator> ();
		mapGenerator.GenerateMap ();
	}


}