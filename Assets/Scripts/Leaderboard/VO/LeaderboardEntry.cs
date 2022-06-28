using System;
using PlayFab;
using PlayFab.ClientModels;

[Serializable]
public class LeaderboardEntry {
	public int rank;
	public string name;
	public float time;
	public string timeString;


	public LeaderboardEntry(PlayerLeaderboardEntry entry) {
		this.rank = entry.Position;
		this.name = entry.DisplayName;
		//Playfab only stores an int.
		////player's time is multiplied by 1,000 going into db
		///and must be divided coming out.
		this.time = ((float)entry.StatValue) / 1000; 
		TimeSpan ts = TimeSpan.FromSeconds(time);
		this.timeString = string.Format ("{0:00}:{1:00}:{2:00}", ts.Minutes, ts.Seconds, ts.Milliseconds);
	}
	public LeaderboardEntry (string name, float time) {
		this.name = name;
		this.time = time; 
		TimeSpan ts = TimeSpan.FromSeconds(time);
		this.timeString = string.Format ("{0:00}:{1:00}:{2:00}", ts.Minutes, ts.Seconds, ts.Milliseconds);

	}
	public void FormatData() {
		TimeSpan ts = TimeSpan.FromSeconds(time);
		this.timeString = string.Format ("{0:00}:{1:00}:{2:00}", ts.Minutes, ts.Seconds, ts.Milliseconds);
	}

}