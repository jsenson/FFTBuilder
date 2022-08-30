using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AbilityListView : MonoBehaviour {
	[SerializeField] private Transform _contentParent;
	[SerializeField] private AbilityDetailView _abilityViewPrefab;

	private readonly List<AbilityDetailView> _views;

	public void Refresh(Data data) {
		foreach(var ability in data.GetCurrentAbilityList()) {
			Debug.Log(ability.Name);
		}
	}

	public void Clear() {
		// TODO
	}

	public struct Data {
		public CharacterBuild Character;
		public Job Job;
		public Ability.AbilityType Type;
		
		public bool CanSelectMultiple => Type == Ability.AbilityType.Class;

		public List<Ability> GetCurrentAbilityList() {
			return Character.GetAvailablePassivesList(Type, Job);
		}
	}
}
