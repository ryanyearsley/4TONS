using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameTypeSelectButtonClick : AbstractButtonClick { 
	
	[SerializeField]
	private MainMenuScreen gameTypeScreen;
	protected override void OnClick () {
		MainMenuManager.Instance.ChangeMenuScreen(gameTypeScreen);
	}

}