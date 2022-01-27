using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectGameTypeButtonClick : AbstractButtonClick
{

	public Objective objective;

	public override void OnClick () {
		MainMenuManager.Instance.ConfirmObjectiveSelection (objective);
	}
}
