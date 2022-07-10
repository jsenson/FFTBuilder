using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CharacterListView : MonoBehaviour {
	[SerializeField] private CharacterDetailView _detailViewPrefab;
	[SerializeField] private Button _newCharacterButton;

	public event Action<CharacterBuild> OnCharacterSelected;

	private readonly List<CharacterDetailView> _views = new List<CharacterDetailView>();

	public void Refresh(List<CharacterBuild> characters) {
		// TODO
	}
}
