using UnityEditor;

public static class DeveloperMenu {
	[MenuItem("Developer/Import Abilities...")]
	private static void ImportAbilityFile() {
		void onProgress(string abilityInfo, float progress) {
			EditorUtility.DisplayProgressBar("Importing Abilities", abilityInfo, progress);
		}

		string path = EditorUtility.OpenFilePanel("Import Abilities", "Assets/Import Files", "txt");
		if (!string.IsNullOrEmpty(path)) {
			var importer = new AbilityImportFile(path);
			importer.WriteScriptableObjects(onProgress);
		}

		EditorUtility.ClearProgressBar();
	}
}
