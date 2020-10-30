using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelSelectButtonClick : AbstractButton {

	[SerializeField]
	private int sceneIndex;
	protected override void OnClick () {
		SceneManager.LoadScene (sceneIndex);
	}

}