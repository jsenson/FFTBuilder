using UnityEngine;

[CreateAssetMenu(menuName = "FFT/Job Set", fileName = "JobSet.asset")]
public class JobSet : ScriptableObject {
	[SerializeField] private Job[] _jobs;

	public Job[] Jobs => _jobs;
}
