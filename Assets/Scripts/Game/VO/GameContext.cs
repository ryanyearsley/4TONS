using System;

//static info about gametype and environment
[Serializable]
public class GameContext 
{
	public ZoneData zoneData;
	public ObjectiveData objectiveData;
}

//i.e. Gametype
public enum Objective {
	Gauntlet, Zombie_Horde, Tutorial, NOT_SELECTED
}