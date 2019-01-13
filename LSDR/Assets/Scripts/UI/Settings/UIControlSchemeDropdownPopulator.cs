using System.Collections;
using System.Collections.Generic;
using System.Linq;
using InputManagement;
using Torii.UI;
using UnityEngine;

public class UIControlSchemeDropdownPopulator : DropdownPopulator 
{
	protected override void Awake () 
	{
		base.Awake();
        Populate(ControlSchemeManager.Schemes.Select(scheme => scheme.Name).ToList());
	}
}
