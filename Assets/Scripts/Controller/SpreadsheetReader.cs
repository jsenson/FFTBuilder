using System.IO;
using GemBox.Spreadsheet;
using SimpleFileBrowser;
using UnityEngine;

public class SpreadsheetReader {
	private static string InitialPath = Application.dataPath;

	static SpreadsheetReader() {
		SpreadsheetInfo.SetLicense("FREE-LIMITED-KEY");
	}

	public CharacterBuild Load() {
		FileBrowser.SetFilters(false, ".xlsx");
		FileBrowser.ShowLoadDialog(
			onSuccess: OnLoadFileSelected,
			onCancel: null,
			pickMode: FileBrowser.PickMode.Files,
			initialPath: InitialPath
		);

		// TODO: How return thing?  Maybe use FileBrowser.WaitForLoadDialog instead and do some kinda enumerator jazz.
		return null;
	}

	private void OnLoadFileSelected(string[] paths) {
		string path = paths[0];
		InitialPath = Path.GetDirectoryName(path);
		var file = ExcelFile.Load(path);
		var sheet = file.Worksheets[0];

		foreach (var col in sheet.Columns) {
			foreach (var cell in col.Cells) {
				if (cell.Value != null) {
					Debug.Log($"{cell.Name}: {cell.Value}");
				}
			}
		}

		// TODO: parse the sheet and load into a CharacterBuild
	}
}
