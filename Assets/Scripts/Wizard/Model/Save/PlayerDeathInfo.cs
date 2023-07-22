using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class PlayerDeathInfo
{
	public DateTime endTime;
	public string lastWords;

	public PlayerDeathInfo(DateTime end, string lastWords)
	{
		endTime = end;
		this.lastWords = lastWords;
	}
}
