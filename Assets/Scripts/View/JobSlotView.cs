using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class JobSlotView : MonoBehaviour {
	[SerializeField] private TMP_Text _label;
	[SerializeField] private TMP_Dropdown _dropdown;
	[SerializeField] private Button _button;

	public event Action<JobSlotView, Job> OnSelectionChanged;
	public event Action<JobSlotView> OnExpandClicked;

	public Job SelectedJob => GetJobAtIndex(_dropdown.value);

	public bool IsMain => _data.IsMainJob;

	private Data _data;
	private List<Job> _displayedJobs = new List<Job>();


	public void Initialize(Data data) {
		_data = data;
		_label.text = $"{data.Name}:";
		InitDropdown();
	}

	public void SetRestrictedJobs(List<Job> restrictedJobs) {
		if (!_data.IsMainJob) {
			_data.RestrictedJobs = restrictedJobs;
			InitDropdown(true);
		}
	}

	private void Awake() {
		_button.onClick.AddListener(OnButtonClicked);
		_dropdown.onValueChanged.AddListener(OnSelectionValueChanged);
	}

	private void InitDropdown(bool triggerEvents = false) {
		Job previousSelectedJob = SelectedJob;
		bool includeNonGenerics = _data.IncludeNonGenerics;

		_displayedJobs = _data.Options.Where(j => (includeNonGenerics || j.isGeneric) && (!_data.RestrictedJobs?.Contains(j) ?? true)).ToList();
		_dropdown.ClearOptions();
		_dropdown.AddOptions(_displayedJobs.Select(j => GetDisplayName(j)).ToList());

		int oldJobIndex = _displayedJobs.IndexOf(previousSelectedJob);
		if (oldJobIndex > -1) {
			_dropdown.SetValueWithoutNotify(oldJobIndex);
		} else {
			Job newSelectedJob = GetJobAtIndex(0);
			if (triggerEvents && previousSelectedJob != newSelectedJob) {
				if (_dropdown.value == 0) {
					OnSelectionValueChanged(0);
				}

				_dropdown.value = 0;
			}
		}
	}

	private string GetDisplayName(Job job) {
		if (job.isGeneric) {
			return job.Name;
		} else {
			return $"{job.Name} <color=#FF0000>({job.UniqueCharacterName})</color>";
		}
	}

	private Job GetJobAtIndex(int index) {
		int count = _displayedJobs.Count;
		return count > 0 ? _displayedJobs[Mathf.Clamp(index, 0, count)] : null;
	}

	private void OnButtonClicked() {
		OnExpandClicked?.Invoke(this);
	}

	private void OnSelectionValueChanged(int newIndex) {
		Job selectedJob = GetJobAtIndex(newIndex);
		Debug.Log($"OnSelectionChanged: new Job = {selectedJob.Name}");
		OnSelectionChanged?.Invoke(this, selectedJob);
	}

	public struct Data {
		public string Name;
		public List<Job> Options;
		public List<Job> RestrictedJobs;
		public bool IsMainJob;

		public bool IncludeNonGenerics => RestrictedJobs?.Find(j => !j.isGeneric) == null;
	}
}
