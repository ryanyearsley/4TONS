using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartGameButtonClick : AbstractButtonClick
{
	public override void OnClick () {
		MainMenuManager.Instance.StartGame ();
	}
}
