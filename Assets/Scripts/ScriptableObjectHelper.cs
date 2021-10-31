using System.IO;
using UnityEngine;
using UnityEditor;

public static class ScriptableObjectHelper {
	private const string ASSETS = "Assets";
	private const string ABILITIES = "Abilities";
	private const string JOBS = "Jobs";

	public static Ability GetOrCreateAbility(string name, Ability.Type type, string jobName) {
		Ability ability = GetByName<Ability>(name);
		if (ability == null) {
			if (type == Ability.Type.Class) {
				ability = CreateInstanceAtPath<Ability>(Path.Combine(ABILITIES, type.ToString()), jobName, name);
			} else {
				ability = CreateInstanceAtPath<Ability>(ABILITIES, type.ToString(), name);
			}
		}

		return ability;
	}

	public static Job GetOrCreateJob(string name) {
		Job job = GetByName<Job>(name);
		if (job == null) {
			job = CreateInstanceAtPath<Job>(JOBS, name, name);
		}

		return job;
	}

	public static AbilitySet GetOrCreateAbilitySet(string jobName, Ability.Type type) {
		string name = $"{jobName} - {type}";
		AbilitySet set = GetByName<AbilitySet>(name);
		if (set == null) {
			set = CreateInstanceAtPath<AbilitySet>(JOBS, jobName, name);
		}

		return set;
	}

	public static T GetByName<T>(string name) where T : ScriptableObject {
		T item = null;
		string typeName = typeof(T).Name;
		string[] guids = AssetDatabase.FindAssets($"t:{typeName} {name}");
		if (guids.Length > 0) {
			// Ye gods this is stupid.  There's no way to force an exact match so we have to look through multiple partial matches and pick it out manually...
			string guid = GetExactMatchGuid(guids, typeName, name);
			if (!string.IsNullOrEmpty(guid)) {
				item = AssetDatabase.LoadAssetAtPath<T>(AssetDatabase.GUIDToAssetPath(guid));
			}
		}

		return item;
	}

	private static T CreateInstanceAtPath<T>(string parentFolder, string folderName, string name) where T : ScriptableObject {
		string fullPath = Path.Combine(Application.dataPath, parentFolder, folderName);
		if (!Directory.Exists(fullPath)) {
			AssetDatabase.CreateFolder(Path.Combine(ASSETS, parentFolder), folderName);
		}

		T newObject = ScriptableObject.CreateInstance<T>();
		newObject.name = name;
		AssetDatabase.CreateAsset(newObject, Path.Combine(ASSETS, parentFolder, folderName, $"{name}.asset"));
		return newObject;
	}

	private static string GetExactMatchGuid(string[] guids, string typeName, string assetName) {
		int matchIndex = -1;
		for (int i = 0; i < guids.Length; i++) {
			string path = AssetDatabase.GUIDToAssetPath(guids[i]);
			string fileName = Path.GetFileNameWithoutExtension(path);
			if (fileName.Equals(assetName)) {
				if (matchIndex == -1) {
					matchIndex = i;
				} else {
					Debug.LogWarning($"Multiple {typeName} ScriptableObjects found with the name {assetName}.  Only one will be used.");
					break;
				}
			}
		}

		if (matchIndex > -1) {
			return guids[matchIndex];
		} else {
			return null;
		}
	}
}
