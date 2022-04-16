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

	public bool IsMain => _data.IsMain;

	private Data _data;
	private List<Job> _displayedJobs = new List<Job>();


	public void Initialize(Data data) {
		_data = data;
		_label.text = $"{data.Name}:";
		InitDropdown();
	}

	public void SetMainJob(Job mainJob) {
		if (!_data.IsMain) {
			if (_data.MainJob != mainJob) {
				_data.MainJob = mainJob;
				InitDropdown(true);
			}
		}
	}

	private void Awake() {
		_button.onClick.AddListener(OnButtonClicked);
		_dropdown.onValueChanged.AddListener(OnSelectionValueChanged);
	}

	private void InitDropdown(bool triggerEvents = false) {
		int currentIndex = _dropdown.value;
		Job previousSelectedJob = SelectedJob;

		_displayedJobs = _data.Options.Where(j => j != _data.MainJob).ToList();
		_dropdown.ClearOptions();
		_dropdown.AddOptions(_displayedJobs.Select(j => j.isGeneric ? j.Name : $"{j.Name} <color=#FF0000>({j.UniqueCharacterName})</color>").ToList());

		if (currentIndex >= _displayedJobs.Count) {
			currentIndex = _displayedJobs.Count - 1;
		}

		Job newSelectedJob = GetJobAtIndex(currentIndex);
		if (triggerEvents && previousSelectedJob != newSelectedJob) {
			if (_dropdown.value == currentIndex) {
				OnSelectionValueChanged(currentIndex);
			}

			_dropdown.value = currentIndex;
		} else {
			_dropdown.SetValueWithoutNotify(currentIndex);
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
		public Job MainJob;

		public bool IsMain => MainJob == null;
	}
}
