using System;
using System.Collections;
using System.IO;
using GemBox.Spreadsheet;
using SimpleFileBrowser;
using UnityEngine;

public class FileDialog {
	private static string InitialPath = Application.dataPath;

	public event Action<int> OnReadGameIndexFromFile;

	private JobImporter _jobImporter;
	private AbilityImporter _abilityImporter;

	static FileDialog() {
		SpreadsheetInfo.SetLicense("FREE-LIMITED-KEY");
	}

	public FileDialog(JobImporter jobImporter, AbilityImporter abilityImporter) {
		_jobImporter = jobImporter;
		_abilityImporter = abilityImporter;
	}

	public void UpdateImporters(JobImporter jobImporter, AbilityImporter abilityImporter) {
		_jobImporter = jobImporter;
		_abilityImporter = abilityImporter;
	}

	public IEnumerator SaveAsync(int gameIndex, CharacterBuild[] characters, Action onSuccess, Action onCancel) {
		FileBrowser.SetFilters(false, ".fft");
		yield return FileBrowser.WaitForSaveDialog(
			pickMode: FileBrowser.PickMode.Files,
			initialPath: InitialPath,
			initialFilename: "build.fft"
		);

		if (FileBrowser.Success) {
			SaveToFile(gameIndex, characters, FileBrowser.Result[0]);
			onSuccess?.Invoke();
		} else {
			onCancel?.Invoke();
		}
	}

	public IEnumerator LoadAsync(Action<int, CharacterBuild[]> onSuccess, Action onCancel) {
		FileBrowser.SetFilters(false, ".fft");
		yield return FileBrowser.WaitForLoadDialog(
			pickMode: FileBrowser.PickMode.Files,
			initialPath: InitialPath
		);

		if (FileBrowser.Success) {
			var characters = LoadFromFile(FileBrowser.Result[0], out int gameIndex);
			onSuccess?.Invoke(gameIndex, characters);
		} else {
			onCancel?.Invoke();
		}
	}

	private CharacterBuild[] LoadFromFile(string path, out int gameIndex) {
		using var fs = new FileStream(path, FileMode.Open);
		using var reader = new BinaryReader(fs);
		var buildReader = new CharacterBuild.Reader();

		gameIndex = reader.ReadInt32();
		OnReadGameIndexFromFile?.Invoke(gameIndex);
		int count = reader.ReadInt32();
		var builds = new CharacterBuild[count];
		for (int i = 0; i < count; i++) {
			builds[i] = buildReader.Read(reader, _jobImporter, _abilityImporter);
		}

		return builds;
	}

	private void SaveToFile(int gameIndex, CharacterBuild[] characters, string path) {
		var backup = File.Exists(path) ? File.ReadAllBytes(path) : null;
		try {
			using var fs = new FileStream(path, FileMode.Create);
			using var writer = new BinaryWriter(fs);
			var buildWriter = new CharacterBuild.Writer();

			writer.Write(gameIndex);
			writer.Write(characters.Length);
			for (int i = 0; i < characters.Length; i++) {
				buildWriter.Write(characters[i], writer);
			}
		} catch (Exception ex) {
			Debug.LogException(ex);
			if (backup != null && File.Exists(path)) {
				File.WriteAllBytes(path, backup);
			}
		}
	}
}
