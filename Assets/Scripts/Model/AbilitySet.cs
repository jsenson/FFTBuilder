using System.Collections.Generic;

public class AbilitySet {
	public Ability.AbilityType Type { get; set; }

	public List<Ability> Abilities {
		get {
			if (_abilities == null) {
				_abilities = new List<Ability>();
			}

			return _abilities;
		}
	}

	private List<Ability> _abilities;
}
