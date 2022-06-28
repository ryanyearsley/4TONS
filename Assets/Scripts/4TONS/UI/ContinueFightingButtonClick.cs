using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ContinueFightingButtonClick : AbstractButtonClick
{
	public override void OnClick () {
		//GameManager.instance.ResumeCombat ();
		GameManager.instance.OnPause ();
	}
}
