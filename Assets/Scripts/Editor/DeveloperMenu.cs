using UnityEditor;

public static class DeveloperMenu {
	[MenuItem("Developer/Import Abilities...")]
	private static void ImportAbilityFile() {
		void onProgress(string abilityInfo, float progress) {
			EditorUtility.DisplayProgressBar("Importing Abilities", abilityInfo, progress);
		}

		string path = EditorUtility.OpenFilePanel("Import Abilities", "Assets/Import Files", "tsv");
		if (!string.IsNullOrEmpty(path)) {
			try {
				var importer = new AbilityImportFile(path);
				importer.WriteScriptableObjects(onProgress);
			} catch (System.Exception ex) {
				UnityEngine.Debug.LogException(ex);
			}
		}

		EditorUtility.ClearProgressBar();
		AssetDatabase.SaveAssets();
	}
}
