using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "RandomLastWordsData", menuName = "ScriptableObjects/Story/Random Last Words Data", order = 3)]
public class RandomLastWordsData : ScriptableObject
{
	public List<string> lastWords;
	public string GetRandomLastWords()
	{
		int randomIndex = Random.Range(0, lastWords.Count);
		return lastWords[randomIndex];
	}
}
