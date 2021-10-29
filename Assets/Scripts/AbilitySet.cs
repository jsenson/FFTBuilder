using UnityEngine;

[CreateAssetMenu(menuName = "FFT/Ability Set", fileName = "AbilitySet.asset")]
public class AbilitySet : ScriptableObject {
	[SerializeField] private Ability.Type _type;
	[SerializeField] private Ability[] _abilities;

	public Ability[] Abilities => _abilities;
}
