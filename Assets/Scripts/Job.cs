using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "FFT/Job", fileName = "Job.asset")]
public class Job : ScriptableObject {
	[System.Serializable]
	public struct Requirement {
		public Job Job;
		public int Level;
	}

	[SerializeField] private Requirement[] _unlockRequirements = new Requirement[0];
	[SerializeField] private AbilitySet[] _abilitySets = new AbilitySet[0];
	[SerializeField] private Gender _genderLock;
	[SerializeField] private bool _disableSubjob;

	public Requirement[] GetRequirements() {
		List<Requirement> allReqs = new List<Requirement>();
		foreach (var req in _unlockRequirements) {
			Requirement[] subReqs = req.Job.GetRequirements();
			foreach (var subReq in subReqs) {
				AddRequirement(subReq, allReqs);
			}
		}

		foreach (var req in _unlockRequirements) {
			AddRequirement(req, allReqs);
		}

		return allReqs.ToArray();
	}

#if UNITY_EDITOR
	public void AddAbility(Ability abilityToAdd) {
		if (!HasAbility(abilityToAdd)) {
			var set = Array.Find(_abilitySets, set => set.AbilityType == abilityToAdd.AbilityType);
			if (set == null) {
				set = ScriptableObjectHelper.GetOrCreateAbilitySet(name, abilityToAdd.AbilityType);
				set.AbilityType = abilityToAdd.AbilityType;

				Array.Resize(ref _abilitySets, _abilitySets.Length + 1);
				_abilitySets[_abilitySets.Length - 1] = set;
				UnityEditor.EditorUtility.SetDirty(this);
				UnityEditor.AssetDatabase.SaveAssets();
			}

			set.AddAbility(abilityToAdd);
		}
	}

	private bool HasAbility(Ability abilityToFind) {
		var found = Array.Find(_abilitySets, set =>
			set.AbilityType == abilityToFind.AbilityType &&
			Array.Find(set.Abilities, a =>
				a == abilityToFind) != null);
		return found != null;
	}
#endif

	private void AddRequirement(Requirement toAdd, List<Requirement> list) {
		bool added = false;
		for (int i = 0; i < list.Count; i++) {
			var req = list[i];
			if (req.Job.name.Equals(toAdd.Job.name)) {
				added = true;
				if (toAdd.Level > req.Level) {
					list[i] = toAdd;
				}
			}
		}

		if (!added) {
			list.Add(toAdd);
		}
	}

	[ContextMenu("Print Requirements")]
	private void PrintRequirements() {
		foreach (var req in GetRequirements()) {
			Debug.Log($"lvl{req.Level} {req.Job.name}");
		}
	}
}
