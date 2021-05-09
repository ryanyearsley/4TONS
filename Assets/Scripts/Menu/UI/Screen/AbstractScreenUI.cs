using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class AbstractScreenUI : MonoBehaviour
{
	private GameObject screenObject;

	[SerializeField]
	public List<MenuScreen> screenActiveStates;


	protected virtual void Start () {
		screenObject = transform.GetChild (0).gameObject;
		if (screenActiveStates.Contains (MenuScreen.WELCOME)) {
			screenObject.SetActive (true);
		} else {

			screenObject.SetActive (false);
		}
		MainMenuManager.Instance.OnMenuScreenChangeEvent += OnScreenChange;

	}

	protected virtual void OnScreenChange (MenuScreen mainMenuScreen) {
		if (screenActiveStates.Contains (mainMenuScreen)) {
			screenObject.SetActive (true);
		} else {
			screenObject.SetActive (false);
		}
	}
}
