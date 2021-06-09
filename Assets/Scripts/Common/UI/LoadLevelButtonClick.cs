using UnityEngine.SceneManagement;
using NERDSTORM;

public class LoadLevelButtonClick : AbstractButtonClick {
	public int sceneIndex;
	protected override void OnClick () {
			NerdstormSceneManager.instance.LoadSceneByIndex (sceneIndex);

	}
}
