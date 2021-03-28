using UnityEngine.SceneManagement;

public class LoadLevelButtonClick : AbstractButtonClick {
	public int sceneIndex;
	protected override void OnClick () {
		SceneManager.LoadScene (sceneIndex);
	}
}
