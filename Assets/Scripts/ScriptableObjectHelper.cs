using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEditor;

public static class ScriptableObjectHelper {
	private const string ASSETS = "Assets";
	private const string ABILITIES = "Abilities";
	private const string JOBS = "Jobs";

	public static Ability GetOrCreateAbility(string name, Ability.Type type, string jobName) {
		string jobPath = Path.Combine(JOBS, jobName);
		Ability ability = GetByName<Ability>(name, jobPath, a => a.AbilityType == type);
		if (ability == null) {
			ability = CreateInstanceAtPath<Ability>(Path.Combine(jobPath, ABILITIES), type.ToString(), name);
		}

		return ability;
	}

	public static Job GetOrCreateJob(string name) {
		Job job = GetByName<Job>(name, JOBS);
		if (job == null) {
			job = CreateInstanceAtPath<Job>(JOBS, name, name);
		}

		return job;
	}

	public static AbilitySet GetOrCreateAbilitySet(string jobName, Ability.Type type) {
		string name = $"{jobName} - {type}";
		AbilitySet set = GetByName<AbilitySet>(name, Path.Combine(JOBS, jobName));
		if (set == null) {
			set = CreateInstanceAtPath<AbilitySet>(JOBS, jobName, name);
		}

		return set;
	}

	private static T GetByName<T>(string name, string rootFolder, System.Func<T, bool> validate = null) where T : ScriptableObject {
		T item = null;
		string typeName = typeof(T).Name;
		string[] guids = AssetDatabase.FindAssets($"t:{typeName} {name}", new string[] { Path.Combine(ASSETS, rootFolder) });
		if (guids.Length > 0) {
			// Ye gods this is stupid.  There's no way to force an exact match so we have to look through multiple partial matches and pick it out manually...
			// guids = GetExactMatchGuids(guids, name);
			guids = guids.Where(guid => AssetWithGuidHasName(guid, name)).ToArray();
			var assets = new List<T>();
			foreach (string guid in guids) {
				T asset = AssetDatabase.LoadAssetAtPath<T>(AssetDatabase.GUIDToAssetPath(guid));
				if (validate == null || validate(asset)) {
					assets.Add(asset);
				}
			}

			if (assets.Count > 1) {
				Debug.LogWarning($"Multiple {typeName} ScriptableObjects found with the name {name}.  Only one will be used.");
			}

			item = assets.FirstOrDefault();
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
		AssetDatabase.SaveAssets();
		return newObject;
	}

	private static bool AssetWithGuidHasName(string guid, string assetName) {
		string path = AssetDatabase.GUIDToAssetPath(guid);
		string fileName = Path.GetFileNameWithoutExtension(path);
		return fileName.Equals(assetName);
	}
}
