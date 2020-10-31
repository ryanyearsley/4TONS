using System;
[Serializable]
public class StaffSaveData {
	[NonSerialized]
	public StaffData staffData;
	public string staffPath;
	
	public SpellSaveData[] equippedSpellgemSaveData;
}
