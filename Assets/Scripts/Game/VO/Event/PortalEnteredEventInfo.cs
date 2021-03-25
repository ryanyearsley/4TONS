using UnityEngine;

public class PortalEnteredEventInfo {
	public MapDetails previousFloorDetails;
	public MapDetails nextFloorDetails;

	public PortalEnteredEventInfo (MapDetails previousFloorDetails, MapDetails nextFloorDetails) {
		this.previousFloorDetails = previousFloorDetails;
		this.nextFloorDetails = nextFloorDetails;
	}
}
