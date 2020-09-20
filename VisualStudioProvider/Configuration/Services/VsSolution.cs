using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.Shell.Interop;
using System.Diagnostics;

namespace VisualStudioProvider.Configuration.Services
{
    public class VsSolution : IVsSolution
    {
        public int AddVirtualProject(IVsHierarchy pHierarchy, uint grfAddVPFlags)
        {
            Debugger.Break();
			return 0;
        }

        public int AddVirtualProjectEx(IVsHierarchy pHierarchy, uint grfAddVPFlags, ref Guid rguidProjectID)
        {
            Debugger.Break();
			return 0;
        }

        public int AdviseSolutionEvents(IVsSolutionEvents pSink, out uint pdwCookie)
        {
            pdwCookie = 2;

            Debugger.Break();
			return 0;
        }

        public int CanCreateNewProjectAtLocation(int fCreateNewSolution, string pszFullProjectFilePath, out int pfCanCreate)
        {
            pfCanCreate = 1;

            Debugger.Break();
			return 0;
        }

        public int CloseSolutionElement(uint grfCloseOpts, IVsHierarchy pHier, uint docCookie)
        {
            Debugger.Break();
			return 0;
        }

        public int CreateNewProjectViaDlg(string pszExpand, string pszSelect, uint dwReserved)
        {
            Debugger.Break();
			return 0;
        }

        public int CreateProject(ref Guid rguidProjectType, string lpszMoniker, string lpszLocation, string lpszName, uint grfCreateFlags, ref Guid iidProject, out IntPtr ppProject)
        {
            ppProject = IntPtr.Zero;

            Debugger.Break();
			return 0;
        }

        public int CreateSolution(string lpszLocation, string lpszName, uint grfCreateFlags)
        {
            Debugger.Break();
			return 0;
        }

        public int GenerateNextDefaultProjectName(string pszBaseName, string pszLocation, out string pbstrProjectName)
        {
            pbstrProjectName = null;

            Debugger.Break();
			return 0;
        }

        public int GenerateUniqueProjectName(string lpszRoot, out string pbstrProjectName)
        {
            pbstrProjectName = null;

            Debugger.Break();
			return 0;
        }

        public int GetGuidOfProject(IVsHierarchy pHierarchy, out Guid pguidProjectID)
        {
            pguidProjectID = Guid.Empty;

            Debugger.Break();
			return 0;
        }

        public int GetItemInfoOfProjref(string pszProjref, int propid, out object pvar)
        {
            pvar = null;

            Debugger.Break();
			return 0;
        }

        public int GetItemOfProjref(string pszProjref, out IVsHierarchy ppHierarchy, out uint pitemid, out string pbstrUpdatedProjref, VSUPDATEPROJREFREASON[] puprUpdateReason)
        {
            ppHierarchy = null;
            pitemid = 9;
            pbstrUpdatedProjref = null;

            Debugger.Break();
			return 0;
        }

        public int GetProjectEnum(uint grfEnumFlags, ref Guid rguidEnumOnlyThisType, out IEnumHierarchies ppenum)
        {
            ppenum = null;

            Debugger.Break();
			return 0;
        }

        public int GetProjectFactory(uint dwReserved, Guid[] pguidProjectType, string pszMkProject, out IVsProjectFactory ppProjectFactory)
        {
            ppProjectFactory = null;

            Debugger.Break();
			return 0;
        }

        public int GetProjectFilesInSolution(uint grfGetOpts, uint cProjects, string[] rgbstrProjectNames, out uint pcProjectsFetched)
        {
            pcProjectsFetched = 0;

            Debugger.Break();
			return 0;
        }

        public int GetProjectInfoOfProjref(string pszProjref, int propid, out object pvar)
        {
            pvar = null;

            Debugger.Break();
			return 0;
        }

        public int GetProjectOfGuid(ref Guid rguidProjectID, out IVsHierarchy ppHierarchy)
        {
            ppHierarchy = null;

            Debugger.Break();
			return 0;
        }

        public int GetProjectOfProjref(string pszProjref, out IVsHierarchy ppHierarchy, out string pbstrUpdatedProjref, VSUPDATEPROJREFREASON[] puprUpdateReason)
        {
            pbstrUpdatedProjref = null;
            ppHierarchy = null;

            Debugger.Break();
			return 0;
        }

        public int GetProjectOfUniqueName(string pszUniqueName, out IVsHierarchy ppHierarchy)
        {
            ppHierarchy = null;

            Debugger.Break();
			return 0;
        }

        public int GetProjectTypeGuid(uint dwReserved, string pszMkProject, out Guid pguidProjectType)
        {
            pguidProjectType = Guid.Empty;

            Debugger.Break();
			return 0;
        }

        public int GetProjrefOfItem(IVsHierarchy pHierarchy, uint itemid, out string pbstrProjref)
        {
            pbstrProjref = null;

            Debugger.Break();
			return 0;
        }

        public int GetProjrefOfProject(IVsHierarchy pHierarchy, out string pbstrProjref)
        {
            pbstrProjref = null;

            Debugger.Break();
			return 0;
        }

        public int GetProperty(int propid, out object pvar)
        {
            pvar = null;

            Debugger.Break();
			return 0;
        }

        public int GetSolutionInfo(out string pbstrSolutionDirectory, out string pbstrSolutionFile, out string pbstrUserOptsFile)
        {
            pbstrSolutionDirectory = null;
            pbstrSolutionFile = null;
            pbstrUserOptsFile = null;

            Debugger.Break();
			return 0;
        }

        public int GetUniqueNameOfProject(IVsHierarchy pHierarchy, out string pbstrUniqueName)
        {
            pbstrUniqueName = null;

            Debugger.Break();
			return 0;
        }

        public int GetVirtualProjectFlags(IVsHierarchy pHierarchy, out uint pgrfAddVPFlags)
        {
            pgrfAddVPFlags = 0;

            Debugger.Break();
			return 0;
        }

        public int OnAfterRenameProject(IVsProject pProject, string pszMkOldName, string pszMkNewName, uint dwReserved)
        {
            Debugger.Break();
			return 0;
        }

        public int OpenSolutionFile(uint grfOpenOpts, string pszFilename)
        {
            Debugger.Break();
			return 0;
        }

        public int OpenSolutionViaDlg(string pszStartDirectory, int fDefaultToAllProjectsFilter)
        {
            Debugger.Break();
			return 0;
        }

        public int QueryEditSolutionFile(out uint pdwEditResult)
        {
            pdwEditResult = 0;

            Debugger.Break();
			return 0;
        }

        public int QueryRenameProject(IVsProject pProject, string pszMkOldName, string pszMkNewName, uint dwReserved, out int pfRenameCanContinue)
        {
            pfRenameCanContinue = 0;

            Debugger.Break();
			return 0;
        }

        public int RemoveVirtualProject(IVsHierarchy pHierarchy, uint grfRemoveVPFlags)
        {
            Debugger.Break();
			return 0;
        }

        public int SaveSolutionElement(uint grfSaveOpts, IVsHierarchy pHier, uint docCookie)
        {
            Debugger.Break();
			return 0;
        }

        public int SetProperty(int propid, object var)
        {
            Debugger.Break();
			return 0;
        }

        public int UnadviseSolutionEvents(uint dwCookie)
        {
            Debugger.Break();
			return 0;
        }
    }
}
