using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AbilityListView : MonoBehaviour {
	[SerializeField] private AbilityDetailView _abilityViewPrefab;

	private readonly List<AbilityDetailView> _views;

	public void Clear() {
		// TODO
	}
}
