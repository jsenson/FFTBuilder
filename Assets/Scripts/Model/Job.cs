using System;
using System.Collections.Generic;
using UnityEngine;

public class Job : IEquatable<Job> {
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
	public bool IsGeneric => string.IsNullOrEmpty(UniqueCharacterName);

	private List<IBuildStep> _unlockRequirements = new List<IBuildStep>();
	private List<AbilitySet> _abilitySets = new List<AbilitySet>();

	public List<IBuildStep> GetRequirements() {
		var requirements = new List<IBuildStep>();
		foreach (var req in _unlockRequirements) {
			req.AppendToList(requirements);
		}

		return requirements;
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

	public void AddUnlockRequirement(IBuildStep newRquirement) {
		_unlockRequirements.Add(newRquirement);
	}

	public void PrintRequirements() {
		foreach (var req in GetRequirements()) {
			Debug.Log(req.Description);
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
		if (obj is Job other) {
			return Equals(other);
		}

		return false;
	}

	public override int GetHashCode() {
		return base.GetHashCode() ^ Reference.GetHashCode();
	}
}
