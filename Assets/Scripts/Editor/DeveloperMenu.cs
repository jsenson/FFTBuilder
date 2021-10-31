using UnityEngine;
using UnityEditor;

public static class DeveloperMenu {
	[MenuItem("Developer/Import Abilities...")]
	private static void ImportAbilityFile() {
		string path = EditorUtility.OpenFilePanel("Import Abilities", "Assets/Import Files", "txt");
		if (!string.IsNullOrEmpty(path)) {
			var importer = new AbilityImportFile(path);
			importer.WriteScriptableObjects();
			Debug.Log("Done");
		}
	}
}
