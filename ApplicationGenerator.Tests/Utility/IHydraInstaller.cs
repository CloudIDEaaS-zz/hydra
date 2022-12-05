namespace Hydra.ReleaseManagement
{
    public interface IHydraInstaller
    {
        string MsiPath { get; set; }
        string GetProductVersion();
        string Extract(string workingDirectory);
    }
}