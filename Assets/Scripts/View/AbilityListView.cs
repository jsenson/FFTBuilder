using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AbilityListView : MonoBehaviour {
	[SerializeField] private Transform _contentParent;
	[SerializeField] private AbilityDetailView _abilityViewPrefab;
	[SerializeField] private GameObject _spacerPrefab;

	private readonly List<AbilityDetailView> _views = new List<AbilityDetailView>();
	private readonly List<GameObject> _spacers = new List<GameObject>();
	private Data _data;

	public void Refresh(Data data) {
		_data = data;
		Clear();
		InitViews();
	}

	public void Clear() {
		foreach (var view in _views) {
			view.OnToggled -= OnAbilityToggled;
			Destroy(view.gameObject);
		}

		foreach (var spacer in _spacers) {
			Destroy(spacer);
		}

		_spacers.Clear();
		_views.Clear();
	}

	private void InitViews() {
		foreach (var ability in _data.GetCurrentAbilityList()) {
			AddSpacer();
			AddView(ability);
		}
	}

	private void AddView(Ability ability) {
		var newView = Instantiate(_abilityViewPrefab, _contentParent);
		newView.OnToggled += OnAbilityToggled;
		newView.Refresh(ability);
		_views.Add(newView);
	}

	private void AddSpacer() {
		if (_views.Count > 0) {
			var newSpacer = Instantiate(_spacerPrefab, _contentParent);
			_spacers.Add(newSpacer);
		}
	}

	private void OnAbilityToggled(AbilityDetailView sender) {
		Debug.Log($"{sender.Ability.Name}: {sender.Selected}");
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
