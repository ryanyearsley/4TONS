using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class VitalsManager : MonoBehaviour {
	public static VitalsManager Instance { get; private set; }


	private FactionDictionary factionDictionary = new FactionDictionary();

	private VitalsDictionary vitalsDictionary = new VitalsDictionary();
	// Start is called before the first frame update
	void Awake () {
		Instance = this;
	}
	 
	
	public void RegisterVitals(VitalsEntity vitalsEntity) {

		//register root + all creature colliders in global vitalsDictionary.
		int rootId = vitalsEntity.creatureObject.transform.GetInstanceID ();
		if (!vitalsDictionary.ContainsKey(rootId))
			vitalsDictionary.Add (rootId, vitalsEntity);
	}

	public void DeclareAllegiance(VitalsEntity vitalsEntity) {
		//if faction for this tag doesnt exist, create it.
		string entityTag = vitalsEntity.creatureObject.tag;
		if (!factionDictionary.ContainsKey (entityTag)) {
			Faction faction = new Faction();
			faction.factionTag = entityTag;
			factionDictionary.Add (entityTag, faction);
		}
		//register with faction
		factionDictionary [entityTag].factionMemberEntities.Add (vitalsEntity);
	}

	public List<VitalsEntity> AcquirePotentialTargets (VitalsEntity vitalsEntity) {
		string entityTag = vitalsEntity.creatureObject.tag;
		List<VitalsEntity> potentialTargetVitals = new List<VitalsEntity>();
		foreach (KeyValuePair<string, Faction> faction in factionDictionary) {
			if (faction.Key != entityTag) {
				potentialTargetVitals.AddRange (faction.Value.factionMemberEntities);
			}
		}
		return potentialTargetVitals;

	}
	public void DeregisterVitals(VitalsEntity vitalsEntity) {
		vitalsDictionary.Remove (vitalsEntity.creatureObject.gameObject.GetInstanceID ());
	}

	public VitalsEntity GetVitalsEntitybyID (int objectId) {
		if (vitalsDictionary.ContainsKey (objectId)) {
			return vitalsDictionary [objectId];
		} else return null;
	}

	public void DeregisterVitalsObject (int objectId) {
		vitalsDictionary.Remove (objectId);
	}
	public void ApplyDamage (int objectId, float damage) {
		if (vitalsDictionary.ContainsKey (objectId)) {
			VitalsEntity vitalsEntity = vitalsDictionary [objectId];
			if (vitalsEntity.health != null)
				vitalsEntity.health.ApplyDamage (damage);
		}
	}
	public void Heal (int objectId, float healAmount) {
		if (vitalsDictionary.ContainsKey (objectId)) {
			VitalsEntity vitalsEntity = vitalsDictionary [objectId];
			if (vitalsEntity.health != null)
				vitalsEntity.health.Heal (healAmount);
		}
	}

	public void ApplyManaDamage (int objectId, float manaDamage) {
		if (vitalsDictionary.ContainsKey (objectId)) {
			VitalsEntity vitalsEntity = vitalsDictionary [objectId];
			if (vitalsEntity.resource != null)
				vitalsEntity.resource.ApplyResourceDamage (manaDamage);
		}
	}

	public bool SubtractManaCost (int objectId, float manaCost) {
		if (vitalsDictionary.ContainsKey (objectId)) {
			VitalsEntity vitalsEntity = vitalsDictionary [objectId];
			if (vitalsEntity.resource != null)
				vitalsEntity.resource.SubtractResourceCost (manaCost);
		}
		return false;
	}

	public void RegenerateMana (int objectId, float manaRegenAmount) {
		if (vitalsDictionary.ContainsKey (objectId)) {
			VitalsEntity vitalsEntity = vitalsDictionary [objectId];
			if (vitalsEntity.resource != null)
				vitalsEntity.resource.RegenerateMana (manaRegenAmount);
		}
	}

	public void Snare (int objectId, SpeedAlteringEffect debuffInfo) {
		if (vitalsDictionary.ContainsKey (objectId)) {
			VitalsEntity vitalsEntity = vitalsDictionary [objectId];
			if (vitalsEntity.movement != null)
				vitalsEntity.movement.OnAddDebuff (debuffInfo);
		}
	}

}
