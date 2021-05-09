using System;
using UnityEngine;

[CreateAssetMenu (fileName = "CustomCreatureData", menuName = "ScriptableObjects/Combat/Creature Data", order = 2)]

public class CreatureData : SpawnObjectData {
	public string creatureName;
	public float health;
	public float resourceMax;
	public float movementSpeed;

	public CreatureAudioClipsPackage clipsPackage;
}

[Serializable]
public class CreatureAudioClipsPackage {
	public AudioClip attackSound;
	public AudioClip gruntSound;
	public AudioClip deathSound;

	public AudioClip happySound;
	public AudioClip sadSound;
}
