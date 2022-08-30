using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class DataController : MonoBehaviour {
	[SerializeField] private SourceFiles[] _sources;

	public JobImporter JobImporter => _jobImporter;
	public AbilityImporter AbilityImporter => _abilityImporter;
	public List<CharacterBuild> Characters => _characters;

	private string _loadedSource;
	private readonly JobImporter _jobImporter = new JobImporter();
	private readonly AbilityImporter _abilityImporter = new AbilityImporter();
	private readonly List<CharacterBuild> _characters = new List<CharacterBuild>();

	private void Awake() {
		Load(_sources[0]);
	}

	public void Load(int gameIndex) {
		Load(_sources[Mathf.Clamp(gameIndex, 0, _sources.Length)]);
	}

	public void Unload() {
		_abilityImporter.Unload();
		_jobImporter.Unload();
		_characters.Clear();
		_loadedSource = null;
	}

	public List<string> GetGameNames() {
		return _sources.Select(s => s.Name).ToList();
	}

	private void Load(SourceFiles source) {
		if (source.Name != _loadedSource) {
			Unload();
			_jobImporter.Load(source.JobFile);
			_abilityImporter.Load(source.AbilityFile, _jobImporter.GetAllByReference());
			_loadedSource = source.Name;
			LoadCharacters();
		}
	}

	private void LoadCharacters() {
		// No saved char set loading yet.
		_characters.Add(CharacterBuild.GetDefault(_jobImporter, _abilityImporter));
	}

	[Serializable]
	public struct SourceFiles {
		public string Name;
		public TextAsset JobFile;
		public TextAsset AbilityFile;
	}
}
