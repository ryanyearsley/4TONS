using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WizardBuildManager : MonoBehaviour
{
	public TextAsset staffFile;
	public GameObject tilePrefab;
	public Transform staffOrigin;//bottom left corner


	void Awake () {
		int [,] staffTiles = StaffFactory.DeserializeStaffFile (staffFile);
		StaffFactory.BuildStaff (staffTiles, tilePrefab, staffOrigin.position);
	}
}
