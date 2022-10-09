using System;
using System.Collections;
using System.Collections.Generic;
using GemBox.Spreadsheet;
using SimpleFileBrowser;
using UnityEngine;

public class SheetExporter {
	private static string InitialPath = Application.dataPath;

	static SheetExporter() {
		SpreadsheetInfo.SetLicense("FREE-LIMITED-KEY");
	}

	public IEnumerator ExportAsync(CharacterBuild[] characters, Action onSuccess, Action onCancel) {
		FileBrowser.SetFilters(false, ".xlsx");
		yield return FileBrowser.WaitForSaveDialog(
			pickMode: FileBrowser.PickMode.Files,
			initialPath: InitialPath,
			initialFilename: "FFT Build.xlsx"
		);

		if (FileBrowser.Success) {
			SaveToFile(characters, FileBrowser.Result[0]);
			onSuccess?.Invoke();
		} else {
			onCancel?.Invoke();
		}
	}

	private void SaveToFile(CharacterBuild[] characters, string path) {
		var file = new ExcelFile();
		var sheet = file.Worksheets.Add("Builds");

		for (int i = 0; i < characters.Length; i++) {
			int checkCol = i * 2;
			int col = checkCol + 1;
			int row = 0;
			var character = characters[i];

			row = SetName(sheet.Cells[row, col], character);
			row++;
			row = SetMainJob(sheet.Cells[row, col], character);
			row = SetSubJobs(sheet.Cells[row, col], character);
			row = SetPassives(sheet.Cells[row, col], character);
			row++;

			var buildSteps = character.GetRequirements();
			SetCheckmarks(sheet.Cells[row, checkCol], buildSteps);
			SetBuildSteps(sheet.Cells[row, col], buildSteps);
		}

		sheet.PrintOptions.FitWorksheetWidthToPages = 1;
		file.Save(path);
	}

	private int SetName(ExcelCell cell, CharacterBuild character) {
		cell.Value = character.Name;
		cell.Style.Font.Weight = ExcelFont.BoldWeight;
		return cell.Row.Index + 1;
	}

	private int SetMainJob(ExcelCell cell, CharacterBuild character) {
		cell.Value = character.MainJob.Name;
		Debug.Log($"Main Job: ({cell.Row.Index}, {cell.Column.Index}) = {cell.Value}");
		return cell.Row.Index + 1;
	}

	private int SetSubJobs(ExcelCell startCell, CharacterBuild character) {
		int row = startCell.Row.Index;
		var jobs = character.GetCurrentSubJobs();
		for (int i = 0; i < jobs.Length; i++) {
			var cell = startCell.Worksheet.Cells[row + i, startCell.Column.Index];
			cell.Value = jobs[i].Name;
			Debug.Log($"Sub Job: ({cell.Row.Index}, {cell.Column.Index}) = {cell.Value}");
		}

		return row + jobs.Length;
	}

	private int SetPassives(ExcelCell startCell, CharacterBuild character) {
		int row = startCell.Row.Index;
		for (int i = 0; i < (int)Ability.AbilityType._Count_; i++) {
			var type = (Ability.AbilityType)i;
			if (type != Ability.AbilityType.Class) {
				var cell = startCell.Worksheet.Cells[row++, startCell.Column.Index];
				cell.Value = character.GetPassive(type)?.Name ?? "<none>";
				Debug.Log($"Passive: ({cell.Row.Index}, {cell.Column.Index}) = {cell.Value}");
			}
		}

		// This is horrible... gave up on possibly allowing multiple passives of the same type
		Debug.Log(row.ToString());
		return row;
	}

	private void SetCheckmarks(ExcelCell startCell, List<Job.Requirement> buildSteps) {
		int row = startCell.Row.Index;
		for (int i = 0, count = buildSteps.Count; i < count; i++) {
			var cell = startCell.Worksheet.Cells[row + i, startCell.Column.Index];
			cell.SetValue(false);
			// TODO: Formatting to a checkbox?
		}
	}

	private void SetBuildSteps(ExcelCell startCell, List<Job.Requirement> buildSteps) {
		int row = startCell.Row.Index;
		for (int i = 0, count = buildSteps.Count; i < count; i++) {
			var cell = startCell.Worksheet.Cells[row + i, startCell.Column.Index];
			var step = buildSteps[i];
			cell.Value = $"{step.Level} {step.Job.Name}";
			Debug.Log($"Step: ({cell.Row.Index}, {cell.Column.Index}) = {cell.Value}");
		}
	}
}
