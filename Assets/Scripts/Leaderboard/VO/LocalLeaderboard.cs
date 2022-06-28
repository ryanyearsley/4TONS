using System;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
[Serializable]
public class LocalLeaderboard 
{
	public string leaderboardName;
	public List<LeaderboardEntry> leaderboardEntries = new List<LeaderboardEntry>();

	public void AddTime(string name, float time) {
		leaderboardEntries.Add (new LeaderboardEntry (name, time));
	}

	public void SortList() {
		leaderboardEntries = leaderboardEntries.OrderBy (o => o.time).ToList ();
		for (int i = 0; i < leaderboardEntries.Count; i++) {
			leaderboardEntries [i].rank = i + 1;
		}
	}


}
