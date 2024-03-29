﻿using Generic.Models;
using System.Collections.Generic;

namespace Generic.Components.Navigation
{
    public class NavigationViewModel
    {
        public NavigationViewModel()
        {
        }

        public List<NavigationItem> NavItems { get; internal set; }
        public string NavWrapperClass { get; internal set; }
        public string StartingPath { get; internal set; }
        public string CurrentPagePath { get; internal set; }
        public bool IncludeCurrentPageSelector { get; internal set; }
    }
}