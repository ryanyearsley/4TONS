﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class AbstractScreenUI : MonoBehaviour
{
	private GameObject screenObject;

	[SerializeField]
	public List<MainMenuScreen> screenActiveStates;


	protected virtual void Start () {
		screenObject = transform.GetChild (0).gameObject;
		//InitializePanel (GameManager.instance.gameContext);
		MainMenuManager.Instance.OnMenuScreenChangeEvent += OnScreenChange;
		screenObject.SetActive (false);

	}

	protected virtual void OnScreenChange (MainMenuScreen mainMenuScreen) {
		if (screenActiveStates.Contains (mainMenuScreen)) {
			screenObject.SetActive (true);
		} else {
			screenObject.SetActive (false);
		}
	}
}