using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class JobSelectionView : MonoBehaviour {
	[SerializeField] private VerticalLayoutGroup _layoutGroup;
	[SerializeField] private JobSlotView _viewPrefab;

	public event Action<Job> OnShowJobDetailsClicked;

	private Data _data;
	private JobSlotView _mainView;
	private List<JobSlotView> _subViews = new List<JobSlotView>();

	public void Initialize(Data data) {
		Clear();
		_data = data;
		_mainView = CreateView("Main", data.MainJobOptions, null);
		PopulateSubViews(_mainView.SelectedJob);
	}

	private void Clear() {
		ClearMain();
		ClearSubs();
	}

	private void ClearSubs() {
		foreach (var view in _subViews) {
			// Too lazy to do pooling.
			Destroy(view.gameObject);
		}

		_subViews.Clear();
	}

	private void ClearMain() {
		if (_mainView != null) {
			GameObject.Destroy(_mainView.gameObject);
			_mainView = null;
		}
	}

	private void PopulateSubViews(Job selectedMainJob) {
		int numSubJobs = selectedMainJob.NumSubjobs;
		for (int i = 0; i < numSubJobs; i++) {
			_subViews.Add(CreateView("Sub", _data.SubJobOptions, selectedMainJob));
		}
	}

	private JobSlotView CreateView(string name, List<Job> options, Job mainJob) {
		var view = Instantiate(_viewPrefab, _layoutGroup.transform);
		view.OnSelectionChanged += OnJobChanged;
		view.OnExpandClicked += OnExpandClicked;
		view.Initialize(new JobSlotView.Data() {
			Name = name,
			MainJob = mainJob,
			Options = options
		});

		return view;
	}

	private void OnJobChanged(JobSlotView sender, Job selectedJob) {
		if (sender.IsMain) {
			if (selectedJob.NumSubjobs != _subViews.Count) {
				ClearSubs();
				PopulateSubViews(selectedJob);
			} else {
				foreach (var view in _subViews) {
					view.SetMainJob(selectedJob);
				}
			}
		} else {
			// Do anything on selecting a sub?  Prevent selecting the same subjob twice?
			// Gotta add tracking to subview to know what the previous selection was and revert.
		}
	}

	private void OnExpandClicked(JobSlotView sender) {
		OnShowJobDetailsClicked?.Invoke(sender.SelectedJob);
	}

	public struct Data {
		public List<Job> MainJobOptions;
		public List<Job> SubJobOptions;
	}
}
