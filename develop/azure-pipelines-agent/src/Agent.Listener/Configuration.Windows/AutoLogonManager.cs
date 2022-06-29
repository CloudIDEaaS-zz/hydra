// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.TeamFoundation.DistributedTask.WebApi;
using Microsoft.VisualStudio.Services.Agent.Util;

namespace Microsoft.VisualStudio.Services.Agent.Listener.Configuration
{
    [ServiceLocator(Default = typeof(AutoLogonManager))]
    public interface IAutoLogonManager : IAgentService
    {
        Task ConfigureAsync(CommandSettings command);
        void Unconfigure();
    }

    public class AutoLogonManager : AgentService, IAutoLogonManager
    {
        private ITerminal _terminal;
        private INativeWindowsServiceHelper _windowsServiceHelper;
        private IAutoLogonRegistryManager _autoLogonRegManager;
        private IConfigurationStore _store;

        public override void Initialize(IHostContext hostContext)
        {
            base.Initialize(hostContext);
            _terminal = hostContext.GetService<ITerminal>();
            _windowsServiceHelper = hostContext.GetService<INativeWindowsServiceHelper>();
            _autoLogonRegManager = HostContext.GetService<IAutoLogonRegistryManager>();
            _store = hostContext.GetService<IConfigurationStore>();
        }

        public async Task ConfigureAsync(CommandSettings command)
        {
            if (!_windowsServiceHelper.IsRunningInElevatedMode())
            {
                Trace.Error("Needs Administrator privileges to configure agent with AutoLogon capability.");
                throw new SecurityException(StringUtil.Loc("NeedAdminForAutologonCapability"));
            }

            string domainName;
            string userName;
            string logonAccount;
            string logonPassword;

            while (true)
            {
                logonAccount = command.GetWindowsLogonAccount(defaultValue: string.Empty, descriptionMsg: StringUtil.Loc("AutoLogonAccountNameDescription"));
                GetAccountSegments(logonAccount, out domainName, out userName);

                if ((string.IsNullOrEmpty(domainName) || domainName.Equals(".", StringComparison.CurrentCultureIgnoreCase)) && !logonAccount.Contains("@"))
                {
                    logonAccount = String.Format("{0}\\{1}", Environment.MachineName, userName);
                    domainName = Environment.MachineName;
                }
                Trace.Info("LogonAccount after transforming: {0}, user: {1}, domain: {2}", logonAccount, userName, domainName);

                logonPassword = command.GetWindowsLogonPassword(logonAccount);
                if (_windowsServiceHelper.IsValidAutoLogonCredential(domainName, userName, logonPassword))
                {
                    Trace.Info("Credential validation succeeded");
                    break;
                }

                if (command.Unattended)
                {
                    throw new SecurityException(StringUtil.Loc("InvalidAutoLogonCredential"));
                }

                Trace.Error("Invalid credential entered.");
                _terminal.WriteError(StringUtil.Loc("InvalidAutoLogonCredential"));
            }

            _autoLogonRegManager.GetAutoLogonUserDetails(out string currentAutoLogonUserDomainName, out string currentAutoLogonUserName);

            if (currentAutoLogonUserName != null
                    && !userName.Equals(currentAutoLogonUserName, StringComparison.CurrentCultureIgnoreCase)
                    && !domainName.Equals(currentAutoLogonUserDomainName, StringComparison.CurrentCultureIgnoreCase))
            {
                string currentAutoLogonAccount = String.Format("{0}\\{1}", currentAutoLogonUserDomainName, currentAutoLogonUserName);
                if (string.IsNullOrEmpty(currentAutoLogonUserDomainName) || currentAutoLogonUserDomainName.Equals(".", StringComparison.CurrentCultureIgnoreCase))
                {
                    currentAutoLogonAccount = String.Format("{0}\\{1}", Environment.MachineName, currentAutoLogonUserName);
                }

                Trace.Warning($"AutoLogon already enabled for {currentAutoLogonAccount}.");
                if (!command.GetOverwriteAutoLogon(currentAutoLogonAccount))
                {
                    Trace.Error("Marking the agent configuration as failed due to the denial of autologon setting overwriting by the user.");
                    throw new Exception(StringUtil.Loc("AutoLogonOverwriteDeniedError", currentAutoLogonAccount));
                }
                Trace.Info($"Continuing with the autologon configuration.");
            }

            // grant permission for agent root folder and work folder
            Trace.Info("Create local group and grant folder permission to logon account.");
            string agentRoot = HostContext.GetDirectory(WellKnownDirectory.Root);
            string workFolder = HostContext.GetDirectory(WellKnownDirectory.Work);
            Directory.CreateDirectory(workFolder);
            _windowsServiceHelper.GrantDirectoryPermissionForAccount(logonAccount, new[] { agentRoot, workFolder });

            _autoLogonRegManager.UpdateRegistrySettings(command, domainName, userName, logonPassword);
            _windowsServiceHelper.SetAutoLogonPassword(logonPassword);

            await ConfigurePowerOptions();

            SaveAutoLogonSettings(domainName, userName);
            RestartBasedOnUserInput(command);
        }

        public void Unconfigure()
        {
            if (!_windowsServiceHelper.IsRunningInElevatedMode())
            {
                Trace.Error("Needs Administrator privileges to unconfigure an agent running with AutoLogon capability.");
                throw new SecurityException(StringUtil.Loc("NeedAdminForAutologonRemoval"));
            }

            var autoLogonSettings = _store.GetAutoLogonSettings();
            _autoLogonRegManager.ResetRegistrySettings(autoLogonSettings.UserDomainName, autoLogonSettings.UserName);
            _windowsServiceHelper.ResetAutoLogonPassword();

            // Delete local group we created during configure.
            string agentRoot = HostContext.GetDirectory(WellKnownDirectory.Root);
            string workFolder = HostContext.GetDirectory(WellKnownDirectory.Work);
            _windowsServiceHelper.RevokeDirectoryPermissionForAccount(new[] { agentRoot, workFolder });

            Trace.Info("Deleting the autologon settings now.");
            _store.DeleteAutoLogonSettings();
            Trace.Info("Successfully deleted the autologon settings.");
        }

        private void SaveAutoLogonSettings(string domainName, string userName)
        {
            Trace.Entering();
            var settings = new AutoLogonSettings()
            {
                UserDomainName = domainName,
                UserName = userName
            };
            _store.SaveAutoLogonSettings(settings);
            Trace.Info("Saved the autologon settings");
        }

        private async Task ConfigurePowerOptions()
        {
            var filePath = WhichUtil.Which("powercfg.exe", require: true, trace: Trace);
            string[] commands = new string[] { "/Change monitor-timeout-ac 0", "/Change monitor-timeout-dc 0" };

            foreach (var command in commands)
            {
                try
                {
                    Trace.Info($"Running powercfg.exe with {command}");
                    using (var processInvoker = HostContext.CreateService<IProcessInvoker>())
                    {
                        processInvoker.OutputDataReceived += delegate (object sender, ProcessDataReceivedEventArgs message)
                        {
                            Trace.Info(message.Data);
                        };

                        processInvoker.ErrorDataReceived += delegate (object sender, ProcessDataReceivedEventArgs message)
                        {
                            Trace.Error(message.Data);
                            _terminal.WriteError(message.Data);
                        };

                        await processInvoker.ExecuteAsync(
                                workingDirectory: string.Empty,
                                fileName: filePath,
                                arguments: command,
                                environment: null,
                                cancellationToken: CancellationToken.None);
                    }
                }
                catch (Exception ex)
                {
                    //we will not stop the configuration. just show the warning and continue
                    _terminal.WriteError(StringUtil.Loc("PowerOptionsConfigError"));
                    Trace.Error(ex);
                }
            }
        }

        private void RestartBasedOnUserInput(CommandSettings command)
        {
            Trace.Info("Asking the user to restart the machine to launch agent and for autologon settings to take effect.");
            _terminal.WriteLine(StringUtil.Loc("RestartMessage"));

            var noRestart = command.GetNoRestart();
            if (!noRestart)
            {
                var shutdownExePath = WhichUtil.Which("shutdown.exe", trace: Trace);

                Trace.Info("Restarting the machine in 15 seconds");
                _terminal.WriteLine(StringUtil.Loc("RestartIn15SecMessage"));
                string msg = StringUtil.Loc("ShutdownMessage");
                //we are not using ProcessInvoker here as today it is not designed for 'fire and forget' pattern
                //ExecuteAsync API of ProcessInvoker waits for the process to exit
                var args = $@"-r -t 15 -c ""{msg}""";
                Trace.Info($"Shutdown.exe path: {shutdownExePath}. Arguments: {args}");
                Process.Start(shutdownExePath, $@"{args}");
            }
            else
            {
                _terminal.WriteLine(StringUtil.Loc("NoRestartSuggestion"));
            }
        }

        //todo: move it to a utility class so that at other places it can be re-used
        private void GetAccountSegments(string account, out string domain, out string user)
        {
            string[] segments = account.Split('\\');
            domain = string.Empty;
            user = account;
            if (segments.Length == 2)
            {
                domain = segments[0];
                user = segments[1];
            }
        }
    }
}
