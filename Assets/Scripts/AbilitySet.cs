using System;
using UnityEngine;

[CreateAssetMenu(menuName = "FFT/Ability Set", fileName = "AbilitySet.asset")]
public class AbilitySet : ScriptableObject {
	[SerializeField] private Ability.Type _type;
	[SerializeField] private Ability[] _abilities;

	public Ability.Type AbilityType => _type;

	public Ability[] Abilities => _abilities;

#if UNITY_EDITOR
	public void AddAbility(Ability abilityToAdd) {
		Array.Resize(ref _abilities, _abilities.Length + 1);
		_abilities[_abilities.Length - 1] = abilityToAdd;
		UnityEditor.EditorUtility.SetDirty(this);
	}
#endif
}
