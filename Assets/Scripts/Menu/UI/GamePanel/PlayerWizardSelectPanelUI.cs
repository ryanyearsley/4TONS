
using UnityEngine;


//CURRENTLY NOT IN USE! 
//This panel is to be used for a hybrid character creation/selection screen for multiplayer.
public enum WizardSelectPhase {
	SELECT, CREATE_NEW, READY
}
public class PlayerWizardSelectPanelUI : MonoBehaviour {
	[SerializeField]
	public int playerIndex { get; private set; }
	[SerializeField]
	private WizardSelectPhase currentWizardSelectPhase = WizardSelectPhase.SELECT;

	[SerializeField]
	private GameObject selectWizardPanel;
	[SerializeField]
	private GameObject createNewWizardPanel;
	[SerializeField]
	private GameObject readyPanel;

	private WizardCreatePanelUI wizardCreatePanelUI;
	private WizardSelectPlayerPanelUI existingWizardPanelUI;

	private void Start () {
	}
	public void InitializePanel(int index) {
		existingWizardPanelUI = selectWizardPanel.GetComponent<WizardSelectPlayerPanelUI> ();
		wizardCreatePanelUI = createNewWizardPanel.GetComponent<WizardCreatePanelUI> ();
		playerIndex = index;
		ChangeWizardSelectPhase (0);
	}

	public void ChangeWizardSelectPhase(int phase) {
		currentWizardSelectPhase = (WizardSelectPhase) phase;
		selectWizardPanel.SetActive (false);
		createNewWizardPanel.SetActive (false);
		switch (currentWizardSelectPhase) {
			case WizardSelectPhase.SELECT:
				selectWizardPanel.SetActive (true); 
				break;
			case WizardSelectPhase.CREATE_NEW:
				createNewWizardPanel.SetActive (true);
				break;
			case WizardSelectPhase.READY:
				readyPanel.SetActive (true);
				break;
			default:
				break;
		}
	}
	
}
