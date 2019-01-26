using UnityEngine;
using System.Collections.Generic;
using Torii.UI;

namespace UI
{
	public class UIResolutionDropdownPopulator : DropdownPopulator
	{
	    protected override void Awake()
	    {
	        base.Awake();
            Populate(getResolutions());
	    }

	    private List<string> getResolutions()
	    {
            List<string> resolutions = new List<string>();
	        foreach (Resolution res in Screen.resolutions)
	        {
                resolutions.Add($"{res.width}x{res.height}");
	        }
	        return resolutions;
	    }
	}
}