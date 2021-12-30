using System;
using UnityEngine;

[CreateAssetMenu (fileName = "CustomCreatureData", menuName = "ScriptableObjects/Combat/Creature Data", order = 2)]

public class CreatureData : SpawnObjectData {
	public string creatureName;
	public float maxHealth;
	public float maxResource;
	public float movementSpeed;
	public CreatureAudio creatureAudio;
}

[Serializable]
public class CreatureAudio {
	public Sound attackSound;
	public AudioClip attackClip;
	public AudioClip gruntSound;
	public AudioClip deathSound;
	public AudioClip happySound;
	public AudioClip sadSound;
}
