using System;
using System.Collections.Generic;
using UnityEngine;

public class Job : IEquatable<Job> {
	public struct Requirement {
		public Job Job;
		public int Level;

		public override string ToString() {
			return $"{Level} {Job}";
		}
	}

	public enum SlotRestriction {
		Both,
		Main,
		Sub
	}

	public string Reference { get; set; }
	public string Name { get; set; }
	public string AbilityName { get; set; }
	public UnitType UnitType { get; set; }
	public SlotRestriction ValidSlots { get; set; }
	public int NumSubjobs { get; set; }
	public string UniqueCharacterName { get; set; }
	public bool isGeneric => string.IsNullOrEmpty(UniqueCharacterName);

	private List<Requirement> _unlockRequirements = new List<Requirement>();
	private List<AbilitySet> _abilitySets = new List<AbilitySet>();

	public List<Requirement> GetRequirements() {
		List<Requirement> allReqs = new List<Requirement>();
		foreach (var req in _unlockRequirements) {
			List<Requirement> subReqs = req.Job.GetRequirements();
			foreach (var subReq in subReqs) {
				AppendRequirement(subReq, allReqs);
			}
		}

		foreach (var req in _unlockRequirements) {
			AppendRequirement(req, allReqs);
		}

		return allReqs;
	}

	public List<Ability> GetAbilities(Ability.AbilityType type) {
		return _abilitySets.Find(set => set.Type == type)?.Abilities ?? new List<Ability>();
	}

	public void AddAbility(Ability ability) {
		if (!HasAbility(ability)) {
			var set = _abilitySets.Find(set => set.Type == ability.Type);
			if (set == null) {
				set = new AbilitySet();
				set.Type = ability.Type;
				_abilitySets.Add(set);
			}

			set.Abilities.Add(ability);
		}
	}

	public bool RemoveAbility(Ability ability) {
		var set = _abilitySets.Find(set => set.Type == ability.Type);
		if (set != null && set.Abilities.Remove(ability)) {
			if (set.Abilities.Count == 0) {
				_abilitySets.Remove(set);
			}

			return true;
		}

		return false;
	}

	public void AddUnlockRequirement(Requirement newRquirement) {
		_unlockRequirements.Add(newRquirement);
	}

	public void PrintRequirements() {
		foreach (var req in GetRequirements()) {
			Debug.Log($"{req.Level} {req.Job.Name}");
		}
	}

	public bool IsUnitType(UnitType type) {
		if (UnitType == type) {
			return true;
		}

		if (type == UnitType.Male || type == UnitType.Female) {
			return UnitType == UnitType.Human;
		}

		return false;
	}

	public override string ToString() {
		return Name;
	}

	private bool HasAbility(Ability abilityToFind) {
		return _abilitySets.Find(set => set.Type == abilityToFind.Type && set.Abilities.Contains(abilityToFind)) != null;
	}

	private void AppendRequirement(Requirement toAdd, List<Requirement> list) {
		bool added = false;
		for (int i = 0; i < list.Count; i++) {
			var req = list[i];
			if (req.Job.Name.Equals(toAdd.Job.Name)) {
				added = true;
				if (toAdd.Level > req.Level) {
					req.Level = toAdd.Level;
					list[i] = req;
				}
			}
		}

		if (!added) {
			list.Add(toAdd);
		}
	}

	public static bool operator ==(Job a, Job b) {
		if (System.Object.ReferenceEquals(a, null)) {
			return System.Object.ReferenceEquals(b, null);
		} else {
			return a.Equals(b);
		}
	}

	public static bool operator !=(Job a, Job b) {
		return !(a == b);
	}

	public bool Equals(Job other) {
		return !System.Object.ReferenceEquals(other, null) && Reference == other.Reference;
	}

	public override bool Equals(object obj) {
		if (obj is Ability other) {
			return Equals(other);
		}

		return false;
	}

	public override int GetHashCode() {
		return base.GetHashCode() ^ Reference.GetHashCode();
	}
}
