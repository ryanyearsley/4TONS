using System.Collections;
using System.Collections.Generic;
using PlayFab.ClientModels;

public static class LeaderboardUtilities {
	public static List<LeaderboardEntry> ConvertPlayfabLeaderboardEntries (List<PlayerLeaderboardEntry> playfabLbEntries) {
		List <LeaderboardEntry> output = new List<LeaderboardEntry>();

		for (int i = 0; i < playfabLbEntries.Count; i++) {
			output.Add (new LeaderboardEntry (playfabLbEntries [i]));
		}
		return output;
	}

	public static void GenerateFakeTestData(TestLeaderboardData lbData) {
		float firstAndLastDelta = lbData.worstTime - lbData.bestTime;
		for (int i = 0; i < lbData.testLeaderboardEntries.Count; i++) {
			LeaderboardEntry lbEntry = lbData.testLeaderboardEntries[i];
			lbEntry.rank = i + 1;
			lbEntry.name = lbData.randomNameData.GetRandomName ();
			float rankPercentile = (float) i / lbData.testLeaderboardEntries.Count;
			lbEntry.time = lbData.bestTime + (firstAndLastDelta * rankPercentile);
			lbEntry.FormatData ();
		}
		lbData.initialized = true;
	}
}
