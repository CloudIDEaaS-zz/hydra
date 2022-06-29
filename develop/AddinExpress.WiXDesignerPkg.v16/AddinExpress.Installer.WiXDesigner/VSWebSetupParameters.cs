using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace AddinExpress.Installer.WiXDesigner
{
	internal class VSWebSetupParameters
	{
		private List<IISWebSite> _sites;

		private List<IISWebAppPool> _pools;

		internal List<IISWebAppPool> AppPools
		{
			get
			{
				return this._pools;
			}
		}

		internal List<IISWebSite> WebSites
		{
			get
			{
				return this._sites;
			}
		}

		internal VSWebSetupParameters()
		{
			this._sites = new List<IISWebSite>();
			this._pools = new List<IISWebAppPool>();
		}

		internal void Clear()
		{
			this._sites.Clear();
			this._pools.Clear();
		}

		internal void Parse(ref List<WiXEntity> list)
		{
			List<IISWebSite> iSWebSites = list.FindAll((WiXEntity e) => e is IISWebSite).ConvertAll<IISWebSite>((WiXEntity e) => e as IISWebSite);
			if (iSWebSites != null)
			{
				foreach (IISWebSite iSWebSite in iSWebSites)
				{
					this._sites.Add(iSWebSite);
					list.Remove(iSWebSite);
				}
			}
			List<IISWebAppPool> iSWebAppPools = list.FindAll((WiXEntity e) => e is IISWebAppPool).ConvertAll<IISWebAppPool>((WiXEntity e) => e as IISWebAppPool);
			if (iSWebAppPools != null)
			{
				foreach (IISWebAppPool iSWebAppPool in iSWebAppPools)
				{
					this._pools.Add(iSWebAppPool);
					list.Remove(iSWebAppPool);
				}
			}
		}
	}
}