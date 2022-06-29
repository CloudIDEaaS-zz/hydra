using EnvDTE;
using Microsoft.Build.Construction;
using Microsoft.Build.Evaluation;
using Microsoft.VisualStudio.Shell.Interop;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;

namespace AddinExpress.Installer.WiXDesigner
{
	internal class BuildManager : IBuildManager
	{
		private IServiceProvider _serviceProvider;

		private bool _listening;

		private BuildEvents _buildEvents;

		public const string RunWithBuildFlag = "DesignerForWixAfterBuild";

		public bool IsListeningToBuildEvents
		{
			get
			{
				return this._listening;
			}
			set
			{
				if (value != this._listening)
				{
					if (!value)
					{
						this.GetBuildEvents().OnBuildBegin -= new _dispBuildEvents_OnBuildBeginEventHandler(this.buildEvents_OnBuildBegin);
						this.GetBuildEvents().OnBuildDone -= new _dispBuildEvents_OnBuildDoneEventHandler(this.buildEvents_OnBuildDone);
						this.GetBuildEvents().OnBuildProjConfigBegin -= new _dispBuildEvents_OnBuildProjConfigBeginEventHandler(this.buildEvents_OnBuildProjConfigBegin);
						this.GetBuildEvents().OnBuildProjConfigDone -= new _dispBuildEvents_OnBuildProjConfigDoneEventHandler(this.buildEvents_OnBuildProjConfigDone);
					}
					else
					{
						this.GetBuildEvents().OnBuildBegin += new _dispBuildEvents_OnBuildBeginEventHandler(this.buildEvents_OnBuildBegin);
						this.GetBuildEvents().OnBuildDone += new _dispBuildEvents_OnBuildDoneEventHandler(this.buildEvents_OnBuildDone);
						this.GetBuildEvents().OnBuildProjConfigBegin += new _dispBuildEvents_OnBuildProjConfigBeginEventHandler(this.buildEvents_OnBuildProjConfigBegin);
						this.GetBuildEvents().OnBuildProjConfigDone += new _dispBuildEvents_OnBuildProjConfigDoneEventHandler(this.buildEvents_OnBuildProjConfigDone);
					}
					this._listening = value;
				}
			}
		}

		private IEnumerable<string> ProjectNames
		{
			get
			{
				IVsSolution service = this._serviceProvider.GetService(typeof(SVsSolution)) as IVsSolution;
				uint num = 0;
				service.GetProjectFilesInSolution(1, 0, null, out num);
				string[] strArrays = new string[num];
				service.GetProjectFilesInSolution(1, num, strArrays, out num);
				return strArrays;
			}
		}

		public BuildManager(IServiceProvider provider)
		{
			if (provider == null)
			{
				throw new ArgumentNullException("provider");
			}
			this._serviceProvider = provider;
		}

		private static IEnumerable<string> AllItemGroupsInProject(ProjectRootElement project)
		{
			List<string> strs = new List<string>();
			foreach (ProjectItemGroupElement itemGroup in project.ItemGroups)
			{
				foreach (ProjectItemElement item in itemGroup.Items)
				{
					if (item.ItemType == "Reference")
					{
						continue;
					}
					string str = string.Concat("@(", item.ItemType, ")");
					if (strs.Contains(str))
					{
						continue;
					}
					strs.Add(str);
				}
			}
			return strs;
		}

		public ICollection<string> AllItemsInProject(IVsProject project)
		{
			if (project == null)
			{
				throw new ArgumentNullException("project");
			}
			string directoryName = Path.GetDirectoryName(ProjectUtilities.GetProjectFilePath(project));
			IVsHierarchy vsHierarchy = project as IVsHierarchy;
			List<string> strs = this.ChildrenOf(vsHierarchy, -2).ConvertAll<string>((uint id) => {
				string str = null;
				project.GetMkDocument(id, out str);
				if (str != null && str.Length > 0 && !Path.IsPathRooted(str))
				{
					str = CommonUtilities.AbsolutePathFromRelative(str, directoryName);
				}
				return str;
			});
			strs.RemoveAll((string name) => !File.Exists(name));
			return strs;
		}

		private void buildEvents_OnBuildBegin(vsBuildScope Scope, vsBuildAction Action)
		{
			if ((Action == vsBuildAction.vsBuildActionBuild || Action == vsBuildAction.vsBuildActionRebuildAll) && this.BuildStarted != null)
			{
				this.BuildStarted();
			}
		}

		private void buildEvents_OnBuildDone(vsBuildScope Scope, vsBuildAction Action)
		{
			if (this.BuildStopped != null)
			{
				this.BuildStopped();
			}
		}

		private void buildEvents_OnBuildProjConfigBegin(string Project, string ProjectConfig, string Platform, string SolutionConfig)
		{
			if (this.BuildProjConfigBegin != null)
			{
				this.BuildProjConfigBegin(Project, ProjectConfig, Platform, SolutionConfig);
			}
		}

		private void buildEvents_OnBuildProjConfigDone(string Project, string ProjectConfig, string Platform, string SolutionConfig, bool Success)
		{
			if (this.BuildProjConfigDone != null)
			{
				this.BuildProjConfigDone(Project, ProjectConfig, Platform, SolutionConfig, Success);
			}
		}

		private List<uint> ChildrenOf(IVsHierarchy hierarchy, uint rootID)
		{
			List<uint> nums = new List<uint>();
			for (uint i = BuildManager.FirstChild(hierarchy, rootID); i != -1; i = BuildManager.NextSibling(hierarchy, i))
			{
				nums.Add(i);
				nums.AddRange(this.ChildrenOf(hierarchy, i));
			}
			return nums;
		}

		private static ProjectPropertyElement FindProperty(ProjectRootElement msbuildProject, string name)
		{
			ProjectPropertyElement projectPropertyElement;
			using (IEnumerator<ProjectPropertyGroupElement> enumerator = msbuildProject.PropertyGroups.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					using (IEnumerator<ProjectPropertyElement> enumerator1 = enumerator.Current.Properties.GetEnumerator())
					{
						while (enumerator1.MoveNext())
						{
							ProjectPropertyElement current = enumerator1.Current;
							if (current.Name != name)
							{
								continue;
							}
							projectPropertyElement = current;
							return projectPropertyElement;
						}
					}
				}
				return null;
			}
			return projectPropertyElement;
		}

		private static uint FirstChild(IVsHierarchy hierarchy, uint rootID)
		{
			object obj = null;
			hierarchy.GetProperty(rootID, -1001, out obj);
			if (obj == null)
			{
				return (uint)-1;
			}
			return (uint)obj;
		}

		private BuildEvents GetBuildEvents()
		{
			if (this._buildEvents == null)
			{
				DTE service = this._serviceProvider.GetService(typeof(DTE)) as DTE;
				this._buildEvents = service.Events.BuildEvents;
			}
			return this._buildEvents;
		}

		private static ProjectTaskElement GetNamedTask(ProjectTargetElement target, string taskName)
		{
			ProjectTaskElement projectTaskElement;
			if (target != null)
			{
				using (IEnumerator<ProjectTaskElement> enumerator = target.Tasks.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						ProjectTaskElement current = enumerator.Current;
						if (current.Name != taskName)
						{
							continue;
						}
						projectTaskElement = current;
						return projectTaskElement;
					}
					return null;
				}
				return projectTaskElement;
			}
			return null;
		}

		public string GetProperty(IVsProject project, string name)
		{
			if (project == null)
			{
				throw new ArgumentNullException("project");
			}
			if (name == null)
			{
				throw new ArgumentNullException("name");
			}
			if (name.Length == 0)
			{
				throw new ArgumentException(Resources.EmptyProperty, "name");
			}
			ProjectPropertyElement projectPropertyElement = BuildManager.FindProperty(BuildManager.MSBuildProjectFromIVsProject(project).Xml, name);
			if (projectPropertyElement == null)
			{
				return null;
			}
			return projectPropertyElement.Value;
		}

		private static Microsoft.Build.Evaluation.Project MSBuildProjectFromIVsProject(IVsProject project)
		{
			return ProjectCollection.GlobalProjectCollection.GetLoadedProjects(ProjectUtilities.GetProjectFilePath(project)).FirstOrDefault<Microsoft.Build.Evaluation.Project>();
		}

		private static uint NextSibling(IVsHierarchy hierarchy, uint firstID)
		{
			object obj = null;
			hierarchy.GetProperty(firstID, -1002, out obj);
			if (obj == null)
			{
				return (uint)-1;
			}
			return (uint)obj;
		}

		public void SetProperty(IVsProject project, string name, string value)
		{
			if (project == null)
			{
				throw new ArgumentNullException("project");
			}
			if (name == null)
			{
				throw new ArgumentNullException("name");
			}
			if (value == null)
			{
				throw new ArgumentNullException("value");
			}
			if (name.Length == 0)
			{
				throw new ArgumentException(Resources.EmptyProperty, "name");
			}
			Microsoft.Build.Evaluation.Project project1 = BuildManager.MSBuildProjectFromIVsProject(project);
			ProjectPropertyElement projectPropertyElement = BuildManager.FindProperty(project1.Xml, name);
			if (projectPropertyElement != null)
			{
				projectPropertyElement.Value = value;
				return;
			}
			project1.Xml.AddPropertyGroup().AddProperty(name, value);
		}

		public event BuildProjConfigBegin_EventHandler BuildProjConfigBegin;

		public event BuildProjConfigDone_EventHandler BuildProjConfigDone;

		public event EmptyEvent BuildStarted;

		public event EmptyEvent BuildStopped;
	}
}