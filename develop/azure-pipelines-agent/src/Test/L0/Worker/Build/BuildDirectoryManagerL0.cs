// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Microsoft.TeamFoundation.DistributedTask.WebApi;
using Pipelines = Microsoft.TeamFoundation.DistributedTask.Pipelines;
using Microsoft.VisualStudio.Services.Agent.Worker;
using Microsoft.VisualStudio.Services.Agent.Worker.Build;
using Moq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;
using Xunit;

namespace Microsoft.VisualStudio.Services.Agent.Tests.Worker.Build
{
    public sealed class BuildDirectoryManagerL0
    {
        private const string CollectionId = "31ffacb8-b468-4e60-b2f9-c50ce437da92";
        private const string DefinitionId = "1234";
        private BuildDirectoryManager _buildDirectoryManager;
        private Mock<IExecutionContext> _ec;
        private Pipelines.RepositoryResource _repository;
        private IList<Pipelines.RepositoryResource> _repositories;
        private Pipelines.WorkspaceOptions _workspaceOptions;
        private TrackingConfig _existingConfig;
        private TrackingConfig _newConfig;
        private string _trackingFile;
        private Mock<ITrackingManager> _trackingManager;
        private Variables _variables;
        private string _workFolder;

        [Fact]
        [Trait("Level", "L0")]
        [Trait("Category", "Worker")]
        public void CreatesBuildDirectories()
        {
            // Arrange.
            using (TestHostContext hc = Setup())
            {
                // Act.
                _buildDirectoryManager.PrepareDirectory(_ec.Object, _repositories, _workspaceOptions);

                // Assert.
                Assert.True(Directory.Exists(Path.Combine(_workFolder, _newConfig.BuildDirectory, Constants.Build.Path.ArtifactsDirectory)));
                Assert.True(Directory.Exists(Path.Combine(_workFolder, _newConfig.BuildDirectory, Constants.Build.Path.BinariesDirectory)));
                Assert.True(Directory.Exists(Path.Combine(_workFolder, _newConfig.BuildDirectory, Constants.Build.Path.TestResultsDirectory)));
            }
        }

        [Fact]
        [Trait("Level", "L0")]
        [Trait("Category", "Worker")]
        public void CreatesNewConfig()
        {
            // Arrange.
            using (TestHostContext hc = Setup())
            {
                // Act.
                var repos = new[] { _repository };
                _buildDirectoryManager.PrepareDirectory(_ec.Object, repos, _workspaceOptions);

                // Assert.
                _trackingManager.Verify(x => x.Create(_ec.Object, repos, false));
            }
        }

        [Fact]
        [Trait("Level", "L0")]
        [Trait("Category", "Worker")]
        public void CreatesNewConfigWhenHashKeyIsDifferent()
        {
            // Arrange.
            using (TestHostContext hc = Setup(existingConfigKind: ExistingConfigKind.Nonmatching))
            {
                // Act.
                var repos = new[] { _repository };
                _buildDirectoryManager.PrepareDirectory(_ec.Object, repos, _workspaceOptions);

                // Assert.
                _trackingManager.Verify(x => x.LoadExistingTrackingConfig(_ec.Object));
                _trackingManager.Verify(x => x.Create(_ec.Object, repos, false));
                _trackingManager.Verify(x => x.MarkForGarbageCollection(_ec.Object, _existingConfig));
            }
        }

        [Fact]
        [Trait("Level", "L0")]
        [Trait("Category", "Worker")]
        public void DeletesSourcesDirectoryWhenCleanIsSources()
        {
            // Arrange.
            using (TestHostContext hc = Setup(cleanOption: BuildCleanOption.Source))
            {
                string sourcesDirectory = Path.Combine(_workFolder, _newConfig.BuildDirectory, Constants.Build.Path.SourcesDirectory);
                string sourceFile = Path.Combine(sourcesDirectory, "some subdirectory", "some source file");
                Directory.CreateDirectory(Path.GetDirectoryName(sourceFile));
                File.WriteAllText(path: sourceFile, contents: "some source contents");

                // Act.
                _buildDirectoryManager.PrepareDirectory(_ec.Object, _repositories, _workspaceOptions);

                // Assert.
                Assert.True(Directory.Exists(sourcesDirectory));
                Assert.Equal(0, Directory.GetFileSystemEntries(sourcesDirectory, "*", SearchOption.AllDirectories).Length);
            }
        }

        [Fact]
        [Trait("Level", "L0")]
        [Trait("Category", "Worker")]
        public void RecreatesArtifactsAndTestResultsDirectory()
        {
            // Arrange.
            using (TestHostContext hc = Setup())
            {
                string artifactsDirectory = Path.Combine(_workFolder, _newConfig.BuildDirectory, Constants.Build.Path.ArtifactsDirectory);
                string artifactFile = Path.Combine(artifactsDirectory, "some subdirectory", "some artifact file");
                Directory.CreateDirectory(Path.GetDirectoryName(artifactFile));
                File.WriteAllText(path: artifactFile, contents: "some artifact contents");

                string testResultsDirectory = Path.Combine(_workFolder, _newConfig.BuildDirectory, Constants.Build.Path.TestResultsDirectory);
                string testResultsFile = Path.Combine(testResultsDirectory, "some subdirectory", "some test results file");
                Directory.CreateDirectory(Path.GetDirectoryName(testResultsFile));
                File.WriteAllText(path: testResultsFile, contents: "some test result contents");

                // Act.
                _buildDirectoryManager.PrepareDirectory(_ec.Object, _repositories, _workspaceOptions);

                // Assert.
                Assert.True(Directory.Exists(artifactsDirectory));
                Assert.Equal(0, Directory.GetFileSystemEntries(artifactsDirectory).Length);
                Assert.True(Directory.Exists(testResultsDirectory));
                Assert.Equal(0, Directory.GetFileSystemEntries(testResultsDirectory).Length);
            }
        }

        // Recreates build directory when clean is all.
        [Fact]
        [Trait("Level", "L0")]
        [Trait("Category", "Worker")]
        public void RecreatesBuildDirectoryWhenCleanIsAll()
        {
            // Arrange.
            using (TestHostContext hc = Setup(cleanOption: BuildCleanOption.All))
            {
                string buildDirectory = Path.Combine(_workFolder, _newConfig.BuildDirectory);
                string looseFile = Path.Combine(buildDirectory, "some loose directory", "some loose file");
                Directory.CreateDirectory(Path.GetDirectoryName(looseFile));
                File.WriteAllText(path: looseFile, contents: "some loose file contents");

                // Act.
                _buildDirectoryManager.PrepareDirectory(_ec.Object, _repositories, _workspaceOptions);

                // Assert.
                Assert.Equal(4, Directory.GetFileSystemEntries(buildDirectory, "*", SearchOption.AllDirectories).Length);
                Assert.True(Directory.Exists(Path.Combine(_workFolder, _newConfig.BuildDirectory, Constants.Build.Path.ArtifactsDirectory)));
                Assert.True(Directory.Exists(Path.Combine(_workFolder, _newConfig.BuildDirectory, Constants.Build.Path.BinariesDirectory)));
                Assert.True(Directory.Exists(Path.Combine(_workFolder, _newConfig.BuildDirectory, Constants.Build.Path.SourcesDirectory)));
                Assert.True(Directory.Exists(Path.Combine(_workFolder, _newConfig.BuildDirectory, Constants.Build.Path.TestResultsDirectory)));
            }
        }

        [Fact]
        [Trait("Level", "L0")]
        [Trait("Category", "Worker")]
        public void RecreatesBinariesDirectoryWhenCleanIsBinary()
        {
            // Arrange.
            using (TestHostContext hc = Setup(cleanOption: BuildCleanOption.Binary))
            {
                string binariesDirectory = Path.Combine(_workFolder, _newConfig.BuildDirectory, Constants.Build.Path.BinariesDirectory);
                string binaryFile = Path.Combine(binariesDirectory, "some subdirectory", "some binary file");
                Directory.CreateDirectory(Path.GetDirectoryName(binaryFile));
                File.WriteAllText(path: binaryFile, contents: "some binary contents");

                // Act.
                _buildDirectoryManager.PrepareDirectory(_ec.Object, _repositories, _workspaceOptions);

                // Assert.
                Assert.True(Directory.Exists(binariesDirectory));
                Assert.Equal(0, Directory.GetFileSystemEntries(binariesDirectory).Length);
            }
        }

        [Fact]
        [Trait("Level", "L0")]
        [Trait("Category", "Worker")]
        public void PrepareDirectoryUpdateRepositoryPath()
        {
            // Arrange.
            using (TestHostContext hc = Setup())
            {
                // Act.
                _buildDirectoryManager.PrepareDirectory(_ec.Object, _repositories, _workspaceOptions);

                // Assert.
                Assert.Equal(Path.Combine(_workFolder, _newConfig.SourcesDirectory), _repository.Properties.Get<string>(Pipelines.RepositoryPropertyNames.Path));
            }
        }

        [Fact]
        [Trait("Level", "L0")]
        [Trait("Category", "Worker")]
        public void UpdatesExistingConfig()
        {
            // Arrange.
            using (TestHostContext hc = Setup(existingConfigKind: ExistingConfigKind.Matching))
            {
                // Act.
                _buildDirectoryManager.PrepareDirectory(_ec.Object, _repositories, _workspaceOptions);

                // Assert.
                _trackingManager.Verify(x => x.LoadExistingTrackingConfig(_ec.Object));
                _trackingManager.Verify(x => x.UpdateTrackingConfig(_ec.Object, _existingConfig));
            }
        }

        [Fact]
        [Trait("Level", "L0")]
        [Trait("Category", "Worker")]
        public void UpdateDirectory()
        {
            // Arrange.
            using (TestHostContext hc = Setup(existingConfigKind: ExistingConfigKind.Matching))
            {
                // Act.
                var tracking = _buildDirectoryManager.PrepareDirectory(_ec.Object, _repositories, _workspaceOptions);

                _repository.Properties.Set<string>(Pipelines.RepositoryPropertyNames.Path, Path.Combine(hc.GetDirectory(WellKnownDirectory.Work), tracking.BuildDirectory, $"test{Path.DirectorySeparatorChar}foo"));

                var newTracking = _buildDirectoryManager.UpdateDirectory(_ec.Object, _repository);

                // Assert.
                Assert.Equal(newTracking.SourcesDirectory, $"1{Path.DirectorySeparatorChar}test{Path.DirectorySeparatorChar}foo");
            }
        }

        [Fact]
        [Trait("Level", "L0")]
        [Trait("Category", "Worker")]
        public void UpdateDirectoryFailOnInvalidPath()
        {
            // Arrange.
            using (TestHostContext hc = Setup(existingConfigKind: ExistingConfigKind.Matching))
            {
                // Act.
                var tracking = _buildDirectoryManager.PrepareDirectory(_ec.Object, _repositories, _workspaceOptions);

                _repository.Properties.Set<string>(Pipelines.RepositoryPropertyNames.Path, Path.Combine(hc.GetDirectory(WellKnownDirectory.Work), "test\\foo"));

                var exception = Assert.Throws<ArgumentException>(() => _buildDirectoryManager.UpdateDirectory(_ec.Object, _repository));

                // Assert.
                Assert.True(exception.Message.Contains("should be located under agent's work directory"));
            }
        }
        // TODO: Updates legacy config.

        private TestHostContext Setup(
            [CallerMemberName] string name = "",
            BuildCleanOption? cleanOption = null,
            ExistingConfigKind existingConfigKind = ExistingConfigKind.None)
        {
            // Setup the host context.
            TestHostContext hc = new TestHostContext(this, name);

            // Create a random work path.
            var configStore = new Mock<IConfigurationStore>();
            _workFolder = hc.GetDirectory(WellKnownDirectory.Work);
            var settings = new AgentSettings() { WorkFolder = _workFolder };
            configStore.Setup(x => x.GetSettings()).Returns(settings);
            hc.SetSingleton<IConfigurationStore>(configStore.Object);

            // Setup the execution context.
            _ec = new Mock<IExecutionContext>();
            List<string> warnings;
            _variables = new Variables(hc, new Dictionary<string, VariableValue>(), out warnings);
            _variables.Set(Constants.Variables.System.CollectionId, CollectionId);
            _variables.Set(Constants.Variables.System.DefinitionId, DefinitionId);
            _variables.Set(Constants.Variables.Build.Clean, $"{cleanOption}");
            _ec.Setup(x => x.Variables).Returns(_variables);

            // Store the expected tracking file path.
            _trackingFile = Path.Combine(
                _workFolder,
                Constants.Build.Path.SourceRootMappingDirectory,
                _ec.Object.Variables.System_CollectionId,
                _ec.Object.Variables.System_DefinitionId,
                Constants.Build.Path.TrackingConfigFile);

            // Setup the endpoint.
            _repository = new Pipelines.RepositoryResource()
            {
                Alias = "self",
                Type = Pipelines.RepositoryTypes.Git,
                Url = new Uri("http://contoso.visualstudio.com"),
            };
            _repository.Properties.Set<String>(Pipelines.RepositoryPropertyNames.Name, "Some endpoint name");
            _repositories = new[] { _repository };

            _workspaceOptions = new Pipelines.WorkspaceOptions();
            // // Setup the source provider.
            // _sourceProvider = new Mock<ISourceProvider>();
            // _sourceProvider
            //     .Setup(x => x.GetBuildDirectoryHashKey(_ec.Object, _repository))
            //     .Returns(HashKey);
            // hc.SetSingleton<ISourceProvider>(_sourceProvider.Object);

            // Store the existing config object.
            switch (existingConfigKind)
            {
                case ExistingConfigKind.Matching:
                    _existingConfig = new TrackingConfig(_ec.Object, _repositories, 1);
                    Assert.Equal("1", _existingConfig.BuildDirectory);
                    break;
                case ExistingConfigKind.Nonmatching:
                    _existingConfig = new TrackingConfig(_ec.Object, _repositories, 2);
                    Assert.Equal("2", _existingConfig.BuildDirectory);
                    break;
                case ExistingConfigKind.None:
                    break;
                default:
                    throw new NotSupportedException();
            }

            // Store the new config object.
            if (existingConfigKind == ExistingConfigKind.Matching)
            {
                _newConfig = _existingConfig;
            }
            else
            {
                _newConfig = new TrackingConfig(_ec.Object, _repositories, 3);
                Assert.Equal("3", _newConfig.BuildDirectory);
            }

            // Setup the tracking manager.
            _trackingManager = new Mock<ITrackingManager>();
            _trackingManager
                .Setup(x => x.LoadExistingTrackingConfig(_ec.Object))
                .Returns(_existingConfig);

            _trackingManager
                .Setup(x => x.Create(_ec.Object, _repositories, false))
                .Returns(_newConfig);
            if (existingConfigKind == ExistingConfigKind.Nonmatching)
            {
                _trackingManager
                    .Setup(x => x.MarkForGarbageCollection(_ec.Object, _existingConfig));
            }
            else if (existingConfigKind == ExistingConfigKind.Matching)
            {
                _trackingManager
                    .Setup(x => x.UpdateTrackingConfig(_ec.Object, _existingConfig));
            }
            else if (existingConfigKind != ExistingConfigKind.None)
            {
                throw new NotSupportedException();
            }

            hc.SetSingleton<ITrackingManager>(_trackingManager.Object);

            // Setup the build directory manager.
            _buildDirectoryManager = new BuildDirectoryManager();
            _buildDirectoryManager.Initialize(hc);
            return hc;
        }

        private enum ExistingConfigKind
        {
            None,
            Matching,
            Nonmatching,
        }
    }
}
