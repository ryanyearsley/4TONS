using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SubMenuSelectButtonClick : AbstractButtonClick { 
	
	[SerializeField]
	private MenuScreen menuScreen;
	protected override void OnClick () {
		AudioManager.instance.PlaySound ("Confirm");
		MainMenuManager.Instance.ChangeMenuScreen(menuScreen);
	}

}