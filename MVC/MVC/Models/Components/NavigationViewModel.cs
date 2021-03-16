using Generic.Models;
using System.Collections.Generic;

namespace Generic.ViewModels
{
    public class NavigationViewModel
    {
        public NavigationViewModel()
        {
        }

        public List<NavigationItem> NavItems { get; internal set; }
        public string NavWrapperClass { get; internal set; }
        public string StartingPath { get; internal set; }
    }
}