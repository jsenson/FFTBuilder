using System;
using System.Collections;
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
	private int _mainJobIndex = -1;
	private float _errorHeight;

	public void Initialize(Data data) {
		_data = data;
		_mainJobIndex = data.Options.IndexOf(data.MainJob);
		_label.text = $"{data.Name}:";
		InitDropdown(GetJobAtIndex(0));
	}

	public void SetMainJob(Job mainJob) {
		if (!_data.IsMain) {
			int newMainIndex = _data.Options.IndexOf(mainJob);
			if (newMainIndex != _mainJobIndex) {
				Job selectedJob = SelectedJob;
				_data.MainJob = mainJob;
				_mainJobIndex = newMainIndex;
				InitDropdown(selectedJob);
			}
		}
	}

	private void Awake() {
		_button.onClick.AddListener(OnButtonClicked);
		_dropdown.onValueChanged.AddListener(OnSelectionValueChanged);
	}

	private void InitDropdown(Job defaultSelection) {
		_dropdown.ClearOptions();
		_dropdown.AddOptions(_data.Options.Where(j => j != _data.MainJob).Select(j => j.isGeneric ? j.Name : $"{j.Name} <color=#FF0000>({j.UniqueCharacterName})</color>").ToList());

		int newIndex = GetIndexForJob(defaultSelection);
		if (GetJobAtIndex(newIndex) != defaultSelection) {
			_dropdown.value = newIndex;
		} else {
			_dropdown.SetValueWithoutNotify(newIndex);
		}
	}

	private Job GetJobAtIndex(int index) {
		if (_mainJobIndex != -1 && index >= _mainJobIndex) {
			index++;
		}

		return _data.Options[index];
	}

	private int GetIndexForJob(Job job) {
		int index = _data.Options.IndexOf(job);
		if (_mainJobIndex != -1 && index >= _mainJobIndex) {
			index--;
		}

		return index;
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
