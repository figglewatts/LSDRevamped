﻿using System.Collections;
using System.Collections.Generic;
using System.Linq;
using InputManagement;
using Torii.UI;
using UnityEngine;

/// <summary>
/// Populates the control scheme selector dropdown menu.
/// </summary>
public class UIControlSchemeDropdownPopulator : DropdownPopulator 
{
	protected override void Awake () 
	{
		base.Awake();
	    PopulateDropdown();
	}

    public void PopulateDropdown()
    {
        var list = ControlSchemeManager.Schemes.Select(scheme => scheme.Name).ToList();
        list.Sort();
        Populate(list);
    }
}