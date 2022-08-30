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
	[SerializeField] private TintOnSelect _selection;

	public event Action<JobSlotView, Job> OnSelectionChanged;
	public event Action<JobSlotView> OnExpandClicked;

	public Job SelectedJob => GetJobAtIndex(_dropdown.value);

	public bool IsMain => _data.IsMainJob;

	public bool Selected => _selection.Selected;

	private Data _data;
	private List<Job> _displayedJobs = new List<Job>();

	public void Initialize(Data data) {
		_data = data;
		_label.text = $"{data.Name}:";
		InitDropdown();
	}

	public void Refresh() {
		InitDropdown(true);
	}

	public void Deselect() {
		_selection.Deselect();
	}

	private void OnEnable() {
		_button.onClick.AddListener(OnButtonClicked);
		_dropdown.onValueChanged.AddListener(OnSelectionValueChanged);
	}

	private void OnDisable() {
		_button.onClick.RemoveListener(OnButtonClicked);
		_dropdown.onValueChanged.RemoveListener(OnSelectionValueChanged);
	}

	private void InitDropdown(bool triggerEvents = false) {
		Job previousSelectedJob = SelectedJob;
		_displayedJobs = _data.GetCurrentOptions();
		_dropdown.ClearOptions();
		_dropdown.AddOptions(_displayedJobs.Select(j => GetDisplayName(j)).ToList());

		int selection = _displayedJobs.IndexOf(_data.CurrentJob);
		if (selection < 0) {
			selection = _displayedJobs.IndexOf(previousSelectedJob);
			if (selection < 0) {
				selection = 0;
			}
		}

		Job newSelectedJob = GetJobAtIndex(selection);
		if (previousSelectedJob != newSelectedJob) {
			if (triggerEvents) {
				if (_dropdown.value == selection) {
					OnSelectionValueChanged(selection);
				}

				_dropdown.value = selection;
			} else {
				_dropdown.SetValueWithoutNotify(selection);
			}
		}
	}

	private string GetDisplayName(Job job) {
		if (job.IsGeneric) {
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
		if (!_selection.Selected) {
			_selection.Select();
			OnExpandClicked?.Invoke(this);
		}
	}

	private void OnSelectionValueChanged(int newIndex) {
		Job selectedJob = GetJobAtIndex(newIndex);
		OnSelectionChanged?.Invoke(this, selectedJob);
	}

	public struct Data {
		public string Name;
		public CharacterBuild Character;
		public int JobIndex;

		public bool IsMainJob => JobIndex < 0;
		public Job CurrentJob => IsMainJob ? Character.MainJob : Character.GetSubJob(JobIndex);

		public List<Job> GetCurrentOptions() {
			return IsMainJob ? Character.GetMainJobList() : Character.GetSubJobList(JobIndex);
		}
	}
}
