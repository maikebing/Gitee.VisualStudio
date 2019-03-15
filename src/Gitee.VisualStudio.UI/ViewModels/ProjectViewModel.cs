using Gitee.VisualStudio.Shared;
using Gitee.VisualStudio.Shared.Controls;
using System;
using System.Windows;

namespace Gitee.VisualStudio.UI.ViewModels
{
    public class Owner : IEquatable<Owner>
    {
        public string Name { get; set; }
        public string AvatarUrl { get; set; }
        public bool IsExpanded { get; set; }
        public string DisplayName => Name;

        public override bool Equals(object obj)
        {
            if (obj == null || GetType() != obj.GetType())
            {
                return false;
            }

            var p = (Owner)obj;
            return Name == p.Name;
        }
        public override int GetHashCode()
        {
            return Name.GetHashCode();
        }

        public bool Equals(Owner other)
        {
            return Name == other.Name;
        }
    }

    public class ProjectViewModel
    {
        public string Name { get; set; }
        public string Url { get; set; }
        public Owner Owner { get; set; }
        public Octicon Icon { get; set; }
        public string DisplayName { set; get; }
        public bool IsActive { get; set; }
        public string Description { get;  set; }
        public Visibility DescriptionVisibility => !string.IsNullOrEmpty(Description) ? (Description.Length > 10 ? Visibility.Visible : Visibility.Collapsed) : Visibility.Hidden;
        public ProjectViewModel(Project repository)
        {
            Name = repository.Name;
            Url = repository.Url;
            Description = repository.Description;
            DisplayName = repository.DisplayName;
            if (repository.Owner != null)
            {
                Owner = new Owner
                {
                    Name = repository.Owner.Name,
                    AvatarUrl = repository.Owner.AvatarUrl
                };
            }

            Icon = repository.Icon;
        }
    }
}
