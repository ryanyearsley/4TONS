using System;

//static info about gametype and environment
[Serializable]
public class GameContext 
{
	public WorldData worldData;
	public GameDataLegend legend;
	public GameType gameType;
}
public enum GameType {
	GAUNTLET, TOWER_DEFENSE, DUEL
}
