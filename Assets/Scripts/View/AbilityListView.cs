using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AbilityListView : MonoBehaviour {
	[SerializeField] private Transform _contentParent;
	[SerializeField] private AbilityDetailView _abilityViewPrefab;
	[SerializeField] private GameObject _spacerPrefab;
	[SerializeField] private Toggle _selectAllToggle;

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
		_selectAllToggle.gameObject.SetActive(false);
	}

	private void OnEnable() {
		_selectAllToggle.onValueChanged.AddListener(OnSelectAllChanged);
	}

	private void OnDisable() {
		_selectAllToggle.onValueChanged.RemoveListener(OnSelectAllChanged);
	}

	private void InitViews() {
		_selectAllToggle.gameObject.SetActive(_data.CanSelectMultiple);
		foreach (var ability in _data.GetCurrentAbilityList()) {
			AddSpacer();
			AddView(ability);
		}

		UpdateSelectAllToggle();
	}

	private void AddView(Ability ability) {
		var newView = Instantiate(_abilityViewPrefab, _contentParent);
		newView.OnToggled += OnAbilityToggled;
		newView.Refresh(ability, IsSelected(ability));
		_views.Add(newView);
	}

	private void AddSpacer() {
		if (_views.Count > 0) {
			var newSpacer = Instantiate(_spacerPrefab, _contentParent);
			_spacers.Add(newSpacer);
		}
	}

	private bool IsSelected(Ability ability) {
		if (ability.Type == Ability.AbilityType.Class) {
			return _data.Character.HasClassAbility(ability);
		} else {
			return _data.Character.GetPassive(ability.Type) == ability;
		}
	}

	private void OnAbilityToggled(AbilityDetailView sender) {
		if (!_data.CanSelectMultiple) {
			foreach (var view in _views) {
				if (view != sender) {
					view.SetSelected(false, false);
				}
			}
		}

		AddOrRemoveAbility(sender.Selected, sender.Ability);
		UpdateSelectAllToggle();
	}

	private void OnSelectAllChanged(bool selected) {
		foreach (var view in _views) {
			if (view.SetSelected(selected, false)) {
				AddOrRemoveAbility(selected, view.Ability);
			}
		}
	}

	private void UpdateSelectAllToggle() {
		bool allSelected = true;
		foreach (var view in _views) {
			if (!view.Selected) {
				allSelected = false;
			}
		}

		_selectAllToggle.SetIsOnWithoutNotify(allSelected);
	}

	private void AddOrRemoveAbility(bool selected, Ability ability) {
		if (selected) {
			if (ability.Type == Ability.AbilityType.Class) {
				_data.Character.AddClassAbility(ability);
			} else {
				_data.Character.SetPassive(ability);
			}
		} else {
			if (ability.Type == Ability.AbilityType.Class) {
				_data.Character.RemoveClassAbility(ability);
			}
		}
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
