using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class PlayerDeathInfo
{
	public string date;
	public string lastWords;

	public PlayerDeathInfo(string date, string lastWords)
	{
		this.date = date;
		this.lastWords = lastWords;
	}
}
