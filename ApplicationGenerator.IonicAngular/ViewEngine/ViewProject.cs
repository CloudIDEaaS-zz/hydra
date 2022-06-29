using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utils;
using VisualStudioProvider;

namespace AbstraX.ViewEngine
{
    public class ViewProject :  IViewProject
    {
        public string ProjectPath { get;  }
        public Dictionary<string, View> Views { get; }
        public string ViewsPath { get; }
        public VSProject Project { get; }

        public ViewProject(string projectPath)
        {
            var viewsPath = Path.Combine(Path.GetDirectoryName(projectPath), "Views");

            this.ProjectPath = projectPath;
            this.ViewsPath = viewsPath;
            this.Project = new VSProject(projectPath);

            Views = new Dictionary<string, View>();
        }

        IEnumerable<IView> IViewProject.Views
        {
            get
            {
                return this.Views.Values.Cast<IView>();
            }
        }

        public View AddView(string viewRelativePath)
        {
            var view = new View(viewRelativePath, this);

            Views.Add(viewRelativePath, view);

            return view;
        }

        public bool ContainsView(string viewRelativePath)
        {
            return Views.ContainsKey(viewRelativePath);
        }
    }
}
