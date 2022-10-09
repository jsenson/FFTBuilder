using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class DataController : MonoBehaviour {
	[SerializeField] private SourceFiles[] _sources;

	public JobImporter JobImporter => _jobImporter;
	public AbilityImporter AbilityImporter => _abilityImporter;
	public List<CharacterBuild> Characters => _characters;
	public int LoadedIndex { get; private set; }

	private string _loadedSource;
	private readonly JobImporter _jobImporter = new JobImporter();
	private readonly AbilityImporter _abilityImporter = new AbilityImporter();
	private readonly List<CharacterBuild> _characters = new List<CharacterBuild>();

	public void Load(int gameIndex) {
		gameIndex = Mathf.Clamp(gameIndex, 0, _sources.Length);
		Load(_sources[gameIndex]);
		LoadedIndex = gameIndex;
	}

	public void Unload() {
		_abilityImporter.Unload();
		_jobImporter.Unload();
		_characters.Clear();
		_loadedSource = null;
		LoadedIndex = -1;
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
		}
	}

	[Serializable]
	public struct SourceFiles {
		public string Name;
		public TextAsset JobFile;
		public TextAsset AbilityFile;
	}
}
