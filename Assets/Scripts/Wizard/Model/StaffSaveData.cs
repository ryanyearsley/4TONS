using System;
using System.Collections.Generic;

[Serializable]
public class StaffSaveData {

	public StaffData staffData;
	public string staffPath;
	public List<SpellSaveData> equippedSpellsSaveData;
}
