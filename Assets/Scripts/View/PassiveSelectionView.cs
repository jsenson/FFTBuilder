using System;
using System.Collections.Generic;
using UnityEngine;

public class PassiveSelectionView : MonoBehaviour {
	[SerializeField] private PassiveSlotView _viewPrefab;
	[SerializeField] private Transform _viewParent;

	public event Action<PassiveSelectionView> OnSlotSelected;

	public Ability.AbilityType SelectedType { get; private set; }

	public CharacterBuild Character { get; private set; }

	private readonly List<PassiveSlotView> _views = new List<PassiveSlotView>();

	public void Refresh(CharacterBuild character) {
		Character = character;
		Clear();
		InitViews();
	}

	public void Clear() {
		foreach (var view in _views) {
			view.OnSelected -= OnViewSelected;
			Destroy(view.gameObject);
		}

		_views.Clear();
	}

	public void ClearSelection() {
		foreach (var view in _views) {
			view.Deselect();
		}
	}

	private void InitViews() {
		// Start at 1 since Class isn't a passive type
		for (int i = 1; i < (int)Ability.AbilityType._Count_; i++) {
			InitView((Ability.AbilityType)i);
		}
	}

	private void InitView(Ability.AbilityType type) {
		var newView = Instantiate<PassiveSlotView>(_viewPrefab, _viewParent);
		newView.OnSelected += OnViewSelected;
		newView.Refresh(Character, type);
		_views.Add(newView);
	}

	private void OnViewSelected(PassiveSlotView sender) {
		foreach (var view in _views) {
			if (view != sender) {
				view.Deselect();
			}
		}

		SelectedType = sender.PassiveType;
		OnSlotSelected?.Invoke(this);
	}
}
