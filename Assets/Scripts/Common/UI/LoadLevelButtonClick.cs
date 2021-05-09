using UnityEngine.SceneManagement;

public class LoadLevelButtonClick : AbstractButtonClick {
	public int sceneIndex;
	protected override void OnClick () {
		if (MainMenuManager.Instance != null) {
			MainMenuManager.Instance.LoadScene (sceneIndex);

		} else {
			SceneManager.LoadScene (sceneIndex);
		}
	}
}
