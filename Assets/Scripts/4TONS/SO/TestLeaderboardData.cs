using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu (fileName = "Leaderboard Data", menuName = "ScriptableObjects/4TONS/LeaderboardData")]

public class TestLeaderboardData : ScriptableObject
{
	public bool initialized;
	public List<LeaderboardEntry> testLeaderboardEntries;
	public float bestTime;
	public float worstTime;
	//public AnimationCurve timeDeltaCurve;
	public RandomNameData randomNameData;
}
