using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class AbstractScreenUI : MonoBehaviour
{
	protected GameObject screenObject;

	[SerializeField]
	public List<MenuScreen> screenActiveStates;


	protected virtual void Start () {
		screenObject = transform.GetChild (0).gameObject;
		screenObject.SetActive (true);
		MainMenuManager.Instance.OnMenuScreenChangeEvent += OnScreenChange;
		MainMenuManager.Instance.OnPlayerJoinEvent += OnPlayerJoin;
		screenObject.SetActive (false);
	}


	protected virtual void OnScreenChange (MenuScreen mainMenuScreen) {
		if (screenActiveStates.Contains (mainMenuScreen)) {
			screenObject.SetActive (true);
		} else {
			screenObject.SetActive (false);
		}
	}

	protected virtual void OnPlayerJoin(Player player) {

	}
}
