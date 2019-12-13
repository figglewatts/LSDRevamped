using System;
using UnityEngine;

namespace Torii.UnityEditor
{
    [AttributeUsage(AttributeTargets.Field)]
    public class BrowseFileSystemAttribute : PropertyAttribute
    {
        public BrowseType Type { get; }
        public string[] FileFilters { get; }
        
        public string Name { get; set; }

        public BrowseFileSystemAttribute(BrowseType type, string[] fileFilters = null, string name = "")
        {
            Type = type;
            FileFilters = fileFilters;
            Name = name;
        }
    }

    public enum BrowseType
    {
        File,
        Directory
    }
}
