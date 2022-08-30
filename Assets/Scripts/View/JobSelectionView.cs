using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class JobSelectionView : MonoBehaviour {
	[SerializeField] private VerticalLayoutGroup _layoutGroup;
	[SerializeField] private JobSlotView _viewPrefab;

	public event Action<CharacterBuild, Job> OnShowJobDetailsClicked;

	private CharacterBuild _character;
	private JobSlotView _mainView;
	private List<JobSlotView> _subViews = new List<JobSlotView>();

	public void Refresh(CharacterBuild character) {
		Clear();
		_character = character;
		_mainView = CreateView("Main", -1);
		PopulateSubViews(_mainView.SelectedJob);
	}

	public void Clear() {
		ClearMain();
		ClearSubs();
	}

	public void ClearSelection() {
		_mainView.Deselect();
		foreach (var view in _subViews) {
			view.Deselect();
		}
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
			Destroy(_mainView.gameObject);
			_mainView = null;
		}
	}

	private void PopulateSubViews(Job selectedMainJob) {
		int numSubJobs = selectedMainJob.NumSubjobs;
		for (int i = 0; i < numSubJobs; i++) {
			var view = CreateView("Sub", i);
			_subViews.Add(view);
		}
	}

	private JobSlotView CreateView(string name, int jobIndex) {
		var view = Instantiate(_viewPrefab, _layoutGroup.transform);
		view.OnSelectionChanged += OnJobChanged;
		view.OnExpandClicked += OnExpandClicked;
		view.Initialize(new JobSlotView.Data() {
			Name = name,
			Character = _character,
			JobIndex = jobIndex
		});

		return view;
	}

	private void OnJobChanged(JobSlotView sender, Job selectedJob) {
		if (sender.IsMain) {
			_character.SetMainJob(selectedJob);
		} else {
			_character.SetSubJob(_subViews.IndexOf(sender), selectedJob);
		}

		if (sender.IsMain && selectedJob.NumSubjobs != _subViews.Count) {
			ClearSubs();
			PopulateSubViews(selectedJob);
			return;
		} else {
			foreach (var view in _subViews) {
				if (view != sender) {
					view.Refresh();
				}
			}
		}

		if (sender.Selected) {
			OnShowJobDetailsClicked?.Invoke(_character, selectedJob);
		}
	}

	private void OnExpandClicked(JobSlotView sender) {
		if (sender != _mainView) {
			_mainView.Deselect();
		}

		foreach (var view in _subViews) {
			if (view != sender) {
				view.Deselect();
			}
		}

		OnShowJobDetailsClicked?.Invoke(_character, sender.SelectedJob);
	}

	public struct Data {
		public CharacterBuild Character;

		public Data(CharacterBuild character) {
			Character = character;
		}
	}
}
