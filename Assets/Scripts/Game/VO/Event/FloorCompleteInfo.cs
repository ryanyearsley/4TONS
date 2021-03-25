using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloorCompleteInfo
{
	public int nextFloorIndex;
	public int totalFloors;
	public FloorCompleteInfo (int nextFloorIndex, int totalFloors) {
		this.nextFloorIndex = nextFloorIndex;
		this.totalFloors = totalFloors;
	}
}
