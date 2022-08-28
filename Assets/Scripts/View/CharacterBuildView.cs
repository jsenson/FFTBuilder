using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterBuildView : MonoBehaviour {
	[SerializeField] private CharacterBuildStepView _stepPrefab;

	private readonly List<CharacterBuildStepView> _views = new List<CharacterBuildStepView>();
}
