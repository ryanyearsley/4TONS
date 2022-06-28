using System.Collections;
using System.IO;
using System.Collections.Generic;
using UnityEngine;

public class LocalLeaderboardSaveManager : PersistentManager {

	public static LocalLeaderboardSaveManager instance;
	protected override void Awake () {
		base.Awake ();
		if (instance == null) {
			instance = this;
		} else if (instance != this) {
			Destroy (gameObject);
		}
		DontDestroyOnLoad (this.gameObject);
		CreatePersistentDataDirectories ();
	}
	protected override void Start () {
		base.Start ();
	}
	private void CreatePersistentDataDirectories () {
		if (!Directory.Exists (Application.persistentDataPath + "/gauntlet_leaderboards")) {
			Directory.CreateDirectory (Application.persistentDataPath + "/gauntlet_leaderboards");
		}
	}

	public LocalLeaderboard GetLocalGauntletLeaderboard(Zone zone) {
		string leaderboardSavePath = Application.persistentDataPath + "/gauntlet_leaderboards/" + zone.ToString() +".json";
		if (File.Exists (leaderboardSavePath)) {
			//retrieve file, 
			return JsonUtility.FromJson<LocalLeaderboard> (File.ReadAllText(leaderboardSavePath));
		} else return null;
	}
	public void UpdateLocalGauntletLeaderboard (Zone zone, string name, float time) {
		string leaderboardSavePath = Application.persistentDataPath + "/gauntlet_leaderboards/" + zone.ToString() +".json";

		LocalLeaderboard leaderboard = null;
		if (File.Exists(leaderboardSavePath)) {
			//retrieve file, 
			leaderboard = JsonUtility.FromJson<LocalLeaderboard>(leaderboardSavePath);
			
		} else {
			//create new
			leaderboard = CreateNewGauntletLeaderboard (zone);
		}
		leaderboard.AddTime (name, time);
		leaderboard.SortList ();
		string json = JsonUtility.ToJson(leaderboard);
		File.WriteAllText (leaderboardSavePath, json);

	}

	public LocalLeaderboard CreateNewGauntletLeaderboard (Zone zone) {
		LocalLeaderboard newLeaderboard = new LocalLeaderboard();
		newLeaderboard.leaderboardName = zone.ToString ();
		return newLeaderboard;
	}
}
