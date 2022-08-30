using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class CharacterListView : MonoBehaviour {
	[SerializeField] private Transform _viewParent;
	[SerializeField] private CharacterDetailView _detailViewPrefab;
	[SerializeField] private Button _newCharacterButton;

	public event Action<CharacterBuild> OnCharacterSelected;
	public event Action<CharacterBuild> OnCharacterCreated;
	public event Action<CharacterBuild, bool> OnCharacterDeleted;

	private readonly List<CharacterDetailView> _views = new List<CharacterDetailView>();
	private Data _data;

	public void Refresh(Data data) {
		_data = data;
		Clear();
		foreach(var character in data.Characters) {
			AddView(character);
		}
	}

	public CharacterBuild[] GetCharacters() {
		return _views.Select(v => v.Character).ToArray();
	}

	private void OnEnable() {
		_newCharacterButton.onClick.AddListener(CreateNewCharacter);
	}

	private void OnDisable() {
		_newCharacterButton.onClick.RemoveListener(CreateNewCharacter);
	}

	private void Clear() {
		foreach(var view in _views) {
			view.OnSelected -= SelectView;
			view.OnDeletePressed -= DeleteView;
			Destroy(view.gameObject);
		}

		_views.Clear();
	}

	private void AddView(CharacterBuild character) {
		var newView = Instantiate(_detailViewPrefab, _viewParent);
		newView.Refresh(character);
		newView.OnSelected += SelectView;
		newView.OnDeletePressed += DeleteView;
		newView.OnTypeChanged += OnTypeChanged;
		_views.Add(newView);
	}

	private void SelectView(CharacterDetailView view) {
		foreach(var v in _views) {
			if (v != view) {
				v.Deselect();
			}
		}

		OnCharacterSelected?.Invoke(view.Character);
	}

	private void DeleteView(CharacterDetailView view) {
		_views.Remove(view);
		Destroy(view.gameObject);
		OnCharacterDeleted?.Invoke(view.Character, view.Selected);
	}

	private void CreateNewCharacter() {
		var newCharacter = new CharacterBuild(_data.JobImporter, _data.AbilityImporter);
		AddView(newCharacter);
		OnCharacterCreated?.Invoke(newCharacter);
	}

	private void OnTypeChanged(CharacterDetailView sender) {
		if (sender.Selected) {
			OnCharacterSelected?.Invoke(sender.Character);
		}
	}

	public struct Data {
		public List<CharacterBuild> Characters;
		public JobImporter JobImporter;
		public AbilityImporter AbilityImporter;
	}
}
