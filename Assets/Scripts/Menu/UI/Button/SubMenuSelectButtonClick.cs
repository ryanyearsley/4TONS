using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SubMenuSelectButtonClick : AbstractButtonClick { 
	
	[SerializeField]
	private MenuScreen menuScreen;
	protected override void OnClick () {
		MainMenuManager.Instance.ChangeMenuScreen(menuScreen);
	}

}