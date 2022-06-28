using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu (fileName = "RandomNameData", menuName = "ScriptableObjects/Story/Random Name Data", order = 2)]
public class RandomNameData : ScriptableObject {
	public List<string> names;
	public string GetRandomName () {
		int randomIndex = Random.Range (0, names.Count);
		return names [randomIndex];
	}
}
