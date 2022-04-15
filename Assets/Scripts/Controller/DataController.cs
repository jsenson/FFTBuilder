using System;
using System.Linq;
using UnityEngine;

public class DataController : MonoBehaviour {
	[SerializeField] private SourceFiles[] _sources;
	[SerializeField] private TypeSelectionView _selectionView;

	public SourceFiles[] Sources => _sources;

	private string _loadedSource;
	private JobImporter _jobImporter = new JobImporter();
	private AbilityImporter _abilityImporter = new AbilityImporter();

	public void Awake() {
		Load(_sources[0]);
		_selectionView.Initialize(_sources.Select(s => s.Name).ToList());
	}

	public void Load(SourceFiles source) {
		Unload();
		_jobImporter.Load(source.JobFile);
		_abilityImporter.Load(source.AbilityFile, _jobImporter.GetAll());
		_loadedSource = source.Name;
	}

	private void OnEnable() {
		_selectionView.onGameValueChanged += OnGameChanged;
		_selectionView.onTypeValueChanged += OnTypeChanged;
	}

	private void OnDisable() {
		_selectionView.onGameValueChanged += OnGameChanged;
		_selectionView.onTypeValueChanged += OnTypeChanged;
	}

	private void Unload() {
		_abilityImporter.Unload();
		_jobImporter.Unload();
		_loadedSource = null;
	}

	private void OnGameChanged(int newIndex) {
		Debug.Log($"OnGameChanged: {newIndex}");
		var source = _sources[newIndex];
		if (_loadedSource != source.Name) {
			Load(source);
			Debug.Log($"Loaded {source.Name}");
		}
	}

	private void OnTypeChanged(UnitType newType) {
		Debug.Log($"OnTypeChanged: {newType}");
	}

	[Serializable]
	public struct SourceFiles {
		public string Name;
		public TextAsset JobFile;
		public TextAsset AbilityFile;
	}
}
