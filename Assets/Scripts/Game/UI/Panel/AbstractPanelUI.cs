using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AbstractPanelUI : MonoBehaviour {
	protected GameObject panelObject;

	[SerializeField]
	public List<GameState> panelActiveStates;

	public virtual void Start () {
		InitializePanel ();
	}
	public virtual void SubscribeToEvents() {

	}
	protected virtual void InitializePanel () {
		panelObject = transform.GetChild (0).gameObject;
		panelObject.SetActive (true);

		if (GameManager.instance != null) {
			GameManager.instance.UIChangeEvent += OnUIChange;
		}

		OnUIChange (GameState.LOADING);
	}

	protected virtual void OnUIChange (GameState gameState) {
		if (panelActiveStates.Contains (gameState)) {
			panelObject.SetActive (true);
		} else {
			panelObject.SetActive (false);
		}
	}
}
