#tool dotnet:?package=GitVersion.Tool&version=5.12.0
#tool nuget:?package=ReportGenerator&version=5.1.24
#tool nuget:?package=NuGet.CommandLine&version=6.6.1

#addin nuget:?package=Cake.Docker&version=1.2.0
#addin nuget:?package=Cake.FileHelpers&version=6.0.0

#load "config.cake"

using System.Text.RegularExpressions;
using System.Runtime.InteropServices;

var target = Argument("target", "Pack");

var dockerRepositoryProxy = EnvironmentVariable("DOCKER_REPOSITORY_PROXY") ?? $"mcr.microsoft.com";
var dockerRepository = EnvironmentVariable("DOCKER_REPOSITORY") ?? string.Empty;
var dockerRepositoryUsername = EnvironmentVariable("DOCKER_REPOSITORY_USERNAME") ?? string.Empty;
var dockerRepositoryPassword = EnvironmentVariable("DOCKER_REPOSITORY_PASSWORD") ?? string.Empty;

var nugetRepositoryProxy = EnvironmentVariable("NUGET_REPOSITORY_PROXY") ?? $"https://api.nuget.org/v3/index.json";
var nugetRepository = EnvironmentVariable("NUGET_REPOSITORY");
var nugetApiKey = EnvironmentVariable("NUGET_API_KEY");

var DockerRepositoryPrefix = string.IsNullOrWhiteSpace(dockerRepository) ? string.Empty : dockerRepository + "/";

var sonarToken = EnvironmentVariable("SONAR_TOKEN");
var sonarUrl = EnvironmentVariable("SONAR_URL") ?? "https://sonarcloud.io/";

string GetCoverageFilePath()
{
  var testProject = TestProjects.FirstOrDefault();
  if (string.IsNullOrWhiteSpace(testProject))
  {
    return string.Empty;
  }

  var testDirectoryPath = System.IO.Path.GetDirectoryName(testProject);
  return System.IO.Path.GetFullPath($"{testDirectoryPath}/coverage.opencover.xml");
}

string GetCoverageDirectoryPath()
{
  var testProject = TestProjects.FirstOrDefault();
  if (string.IsNullOrWhiteSpace(testProject))
  {
    return string.Empty;
  }

  return System.IO.Path.GetDirectoryName(testProject);
}

string GetTestResultFilePath()
{
  var testProject = TestProjects.FirstOrDefault();
  if (string.IsNullOrWhiteSpace(testProject))
  {
    return string.Empty;
  }

  var testDirectoryPath = System.IO.Path.GetDirectoryName(testProject);
  return System.IO.Path.GetFullPath($"{testDirectoryPath}/TestResults.trx");
}

string GetTestResultFilePath(string testProject, string category)
{
  var testDirectoryPath = System.IO.Path.GetDirectoryName(testProject);
  return System.IO.Path.GetFullPath($"{testDirectoryPath}/TestResults.{category}.trx");
}

Task("UpdateVersion")
  .Does(() =>
  {
    var settings = new GitVersionSettings
    {
      NoFetch = true,
      UpdateAssemblyInfo = false,
      ToolPath = Context.Tools.Resolve("dotnet-gitversion") ?? Context.Tools.Resolve("dotnet-gitversion.exe")
    };

    var result = GitVersion(settings);
    NuGetVersionV2 = result.NuGetVersionV2;

    settings.OutputType = GitVersionOutput.BuildServer;
    GitVersion(settings);
  });

Task("Restore")
  .Does(() =>
  {
    Information("Restoring Solution Packages");
    DotNetRestore(SolutionFileName, new DotNetRestoreSettings()
    {
      Sources = new[] { nugetRepositoryProxy },
      NoCache = true
    });
  });

Task("Build")
  .IsDependentOn("UpdateVersion")
  .IsDependentOn("Restore")
  .Does(() =>
  {
    DotNetBuild(
                SolutionFileName,
                new DotNetBuildSettings()
                {
                  Configuration = buildConfiguration,
                  ArgumentCustomization = args => args
                        .Append($"/p:Version={NuGetVersionV2}")
                        .Append($"/p:PackageVersion={NuGetVersionV2}")
                });
  });

Task("UnitTest")
  .IsDependentOn("Build")
  .ContinueOnError()
  .Does(() =>
  {
    foreach (var testProject in TestProjects)
    {
      var settings = new DotNetTestSettings
      {
        NoWorkingDirectory = true,
        Configuration = buildConfiguration,
        Filter = "Category=\"Unit\"",
        ArgumentCustomization = args => args
          .Append("/p:CollectCoverage=true")
          .Append("/p:CoverletOutput=./coverage.unit.xml")
          .Append("/p:CoverletOutputFormat=opencover")
      };

      settings.Loggers.Add($"trx;LogFileName={GetTestResultFilePath(testProject, "Unit")}");
      settings.Collectors.Add("XPlat Code Coverage");

      DotNetTest(testProject, settings);
    }
  });

Task("IntegrationTest")
  .IsDependentOn("Build")
  .ContinueOnError()
  .Does(() =>
  {
    foreach (var testProject in TestProjects)
    {
      var settings = new DotNetTestSettings
      {
        NoWorkingDirectory = true,
        Configuration = buildConfiguration,
        Filter = "Category=\"Integration\"",
        ArgumentCustomization = args => args
          .Append("/p:CollectCoverage=true")
          .Append("/p:CoverletOutput=./coverage.integration.xml")
          .Append("/p:CoverletOutputFormat=opencover")
      };

      settings.Loggers.Add($"trx;LogFileName={GetTestResultFilePath(testProject, "Integration")}");
      settings.Collectors.Add("XPlat Code Coverage");

      DotNetTest(testProject, settings);
    }
  });

Task("Test")
  .IsDependentOn("UnitTest")
  .IsDependentOn("IntegrationTest")
  .ContinueOnError()
  .Does(() =>
  {
  });

Task("ReportGenerator")
  .IsDependentOn("Test")
  .Does(() =>
  {
    var reports = string.Empty;
    var coverageFiles = new string[] { "coverage.unit.xml", "coverage.integration.xml" };
    foreach (var testProject in TestProjects)
    {
      foreach (var coverageFile in coverageFiles)
      {
        var testProjectDirectory = System.IO.Path.GetDirectoryName(testProject);
        var testProjectCoverageFile = System.IO.Path.Combine(testProjectDirectory, coverageFile);
        if (FileExists(testProjectCoverageFile))
        {
          reports += testProjectCoverageFile + ";";
        }
      }
    }

    FilePath reportGeneratorPath = Context.Tools.Resolve("ReportGenerator.exe");
    StartProcess(reportGeneratorPath, new ProcessSettings
    {
      Arguments = new ProcessArgumentBuilder()
            .Append($"-reports:\"{reports}\"")
            .Append($"-targetdir:output/coverage/coverlet")
            .Append($"-sourcedirs:src")
            .Append($"-reporttypes:Cobertura")
    });
  });

Task("Sonar-Begin")
  .IsDependentOn("UpdateVersion")
  .Does(() =>
  {
    StartProcess("dotnet", new ProcessSettings
    {
      Arguments = new ProcessArgumentBuilder()
          .Append("sonarscanner")
          .Append("begin")
          .Append($"/k:{SonarProjectKey}")
          .Append($"/o:{SonarOrganization}")
          .Append($"/v:{NuGetVersionV2}")
          .Append($"/d:sonar.cs.opencover.reportsPaths={GetCoverageFilePath()}")
          .Append($"/d:sonar.cs.vstest.reportsPaths={GetTestResultFilePath()}")
          .Append($"/d:sonar.host.url={sonarUrl}")
          .Append($"/d:sonar.login={sonarToken}")
    });
  });

Task("Sonar-End")
  .Does(() =>
  {
    StartProcess("dotnet", new ProcessSettings
    {
      Arguments = new ProcessArgumentBuilder()
          .Append("sonarscanner")
          .Append("end")
          .Append($"/d:sonar.login={sonarToken}")
    });
  });

Task("Sonar")
  .IsDependentOn("Sonar-Begin")
  .IsDependentOn("Test")
  .IsDependentOn("Sonar-End");

Task("ExecPack")
  .IsDependentOn("Build")
  .Does(() =>
  {
    Information("Publish Exec Projects");
    foreach (var execProject in ExecProjects)
    {
      var directoryName = new System.IO.FileInfo(execProject).Directory.Name;
      var outputDirectory = System.IO.Path.GetFullPath($"output/exec/{directoryName}");
      DotNetPublish(execProject, new DotNetPublishSettings()
      {
        Configuration = buildConfiguration,
        Framework = "net6.0",
        Runtime = "win-x64",
        PublishSingleFile = true,
        SelfContained = true,
        OutputDirectory = outputDirectory,
      });
    }

    Information("Publishing extra files");
    foreach (var file in ExtraFiles)
    {
      var sourceFilePath = System.IO.Path.GetFullPath(file.Key);
      if (FileExists(sourceFilePath))
      {
        Information($"Copying file '{file.Key}' => '{file.Value}'....");

        var destinationFilePath = System.IO.Path.GetFullPath(file.Value);
        var destinationDirectory = System.IO.Path.GetDirectoryName(destinationFilePath);

        EnsureDirectoryExists(destinationDirectory);
        CopyFile(sourceFilePath, destinationFilePath);
      }
      else
      {
        Warning($"File '{sourceFilePath}' not found");
      }
    }
  });

Task("Publish")
  .IsDependentOn("ExecPack")
  // .IsDependentOn("DockerPack")
  .Does(() =>
  {
    Information("Publishing Keycloak themes");
    CopyDirectory("./deployment/keycloack-themes", "./output/deployment/keycloack-themes");

    Information("Publishing zip files");
    var swarmTemplateDeployFile = $"./deployment/swarm/deploy.ps1";
    var swarmDeployFile = $"./output/deployment/swarm/deploy.ps1";
    if (FileExists(swarmTemplateDeployFile))
    {
      EnsureDirectoryExists($"./output/deployment/swarm");
      CopyFile(swarmTemplateDeployFile, swarmDeployFile);
      ReplaceTextInFiles(swarmDeployFile, "${VERSION_NUMBER}", NuGetVersionV2);

      CopyDirectory("./deployment/config", "./output/deployment/swarm/config");
    }
  });

Task("DockerBuild")
  .IsDependentOn("UpdateVersion")
  .Does(() =>
  {
    if (DockerFiles.Length != OutputImages.Length)
    {
      Error("DockerFiles.Length != OutputImages.Length");
    }

    var tarFileName = "dotnet.csproj.tar.gz";

    using (var process = StartAndReturnProcess("tar", new ProcessSettings { Arguments = $"-cf {tarFileName} -C src {System.IO.Path.GetFileName(SolutionFileName)}" }))
    {
      process.WaitForExit();
    }

    var srcFilePath = GetDirectories("src").FirstOrDefault();
    var files = GetFiles("./src/**/*.csproj");
    foreach (var file in files)
    {
      var relativeFilePath = srcFilePath.GetRelativePath(file);
      using (var process = StartAndReturnProcess("tar", new ProcessSettings { Arguments = $"-rf {tarFileName} -C src {relativeFilePath}" }))
      {
        process.WaitForExit();
      }
    }

    var swarmTemplateDeployFile = $"./deployment/swarm/deploy.ps1";
    var swarmDeployFile = $"./output/deployment/swarm/deploy.ps1";
    if (FileExists(swarmTemplateDeployFile))
    {
      EnsureDirectoryExists($"./output/deployment/swarm");
      CopyFile(swarmTemplateDeployFile, swarmDeployFile);
      ReplaceTextInFiles(swarmDeployFile, "${VERSION_NUMBER}", NuGetVersionV2);

      CopyDirectory("./deployment/config", "./output/deployment/swarm/config");
    }

    for (int i = 0; i < DockerFiles.Length; i++)
    {
      var outputImage = OutputImages[i];
      var dockerFile = DockerFiles[i];
      var settings = new DockerImageBuildSettings()
      {
        File = dockerFile,
        BuildArg = new[]
          {
                $"DOCKER_REPOSITORY_PROXY={dockerRepositoryProxy}",
                $"NUGET_REPOSITORY_PROXY={nugetRepositoryProxy}",
                $"PACKAGE_VERSION={NuGetVersionV2}"
              },
        Tag = new[]
          {
                $"{DockerRepositoryPrefix}{outputImage}:{NuGetVersionV2}",
                $"{DockerRepositoryPrefix}{outputImage}:latest"
              }
      };

      DockerBuild(settings, "./");
    }
  });

Task("DockerPack")
  .IsDependentOn("DockerBuild")
  .Does(() =>
  {
    if (DockerFiles.Length != OutputImages.Length)
    {
      Error("DockerFiles.Length != OutputImages.Length");
    }

    var dockerImagesOutputDirectory = $"./output/docker";
    EnsureDirectoryExists(dockerImagesOutputDirectory);

    for (int i = 0; i < DockerFiles.Length; i++)
    {
      var outputImage = OutputImages[i];
      var settings = new DockerImageSaveSettings()
      {
        Output = $"{dockerImagesOutputDirectory}/{outputImage}-v{NuGetVersionV2}.tar"
      };

      DockerSave(settings, new[] { $"{DockerRepositoryPrefix}{outputImage}:{NuGetVersionV2}" });
    }
  });


Task("NuGetPack")
  .IsDependentOn("Build")
  .IsDependentOn("ExecPack")
  .Does(() =>
  {
    var packageOutputDirectory = $"./output/nuget";
    EnsureDirectoryExists(packageOutputDirectory);
    CleanDirectory(packageOutputDirectory);

    Information("Packaging components projects");
    for (int i = 0; i < ComponentProjects.Length; i++)
    {
      var componentProject = ComponentProjects[i];
      var settings = new DotNetPackSettings
      {
        Configuration = buildConfiguration,
        OutputDirectory = packageOutputDirectory,
        IncludeSymbols = true,
        ArgumentCustomization = args => args
            .Append($"/p:PackageVersion={NuGetVersionV2}")
            .Append($"/p:Version={NuGetVersionV2}")
      };

      DotNetPack(componentProject, settings);
    }


    Information("Packaging components projects with symbols");
    EnsureDirectoryExists("./output/nuget-symbols");
    CleanDirectory("./output/nuget-symbols");

    MoveFiles($"{packageOutputDirectory}/*.symbols.nupkg", "./output/nuget-symbols");
    var symbolFiles = GetFiles("./output/nuget-symbols/*.symbols.nupkg");
    foreach (var symbolFile in symbolFiles)
    {
      var newFileName = symbolFile.ToString().Replace(".symbols", "");
      MoveFile(symbolFile, newFileName);
    }

    Information("Packaging exec projects as zip");
    var outputZipFileNames = new System.Collections.Generic.Dictionary<string, string>();
    foreach (var file in ZipFiles)
    {
      var zipFileName = $"output/zip/{file.Key}-{NuGetVersionV2}-{DateTime.Now:yyyy-MM-dd-HH-mm-ss}.zip";

      outputZipFileNames[file.Key] = zipFileName;

      Information($"Packaging {file.Key} artifact files into {zipFileName}");
      EnsureDirectoryExists($"output/zip");

      var files = GetFiles(file.Value.Path + "/" + file.Value.Pattern).Cast<FilePath>();
      foreach (var pattern in file.Value.ExclusionPatterns)
      {
        files = files.Where(f => !Regex.IsMatch(f.FullPath, pattern));
      }

      Zip(file.Value.Path, zipFileName, files);
    }

    Information("Packaging exec projects as nuget");
    foreach (var file in NuGetFiles)
    {
      if (!outputZipFileNames.ContainsKey(file.Key))
      {
        Warning($"Zip file for {file.Key} not found. Skipped.");
        continue;
      }

      var outputDirectoryPath = $"output/nuget-pack/{file.Key}";
      var contentDirectoryPath = $"output/nuget-pack/{file.Key}/content";
      EnsureDirectoryExists(contentDirectoryPath);
      CleanDirectory(contentDirectoryPath);

      Unzip(outputZipFileNames[file.Key], contentDirectoryPath);
      var nuspecFile = System.IO.Path.Combine(outputDirectoryPath, System.IO.Path.GetFileName(file.Value));
      CopyFile(file.Value, nuspecFile);
      ReplaceTextInFiles(nuspecFile, "${VERSION_NUMBER}", NuGetVersionV2);

      if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
      {
        NuGetPack(nuspecFile, new NuGetPackSettings()
        {
          OutputDirectory = "./output/nuget",
          ToolPath = Context.Tools.Resolve("nuget.exe")
        });
      }
      else
      {
        StartProcess("nuget", new ProcessSettings
        {
          Arguments = new ProcessArgumentBuilder()
                .Append("pack")
                .Append(nuspecFile)
                .Append("-OutputDirectory")
                .Append("./output/nuget")
        });
      }
    }
  });

Task("NuGetPush")
   .IsDependentOn("NuGetPack")
   .Does(() =>
{
  var nugetFiles = GetFiles("./output/nuget/*.nupkg");
  foreach (var nugetFile in nugetFiles)
  {
    DotNetNuGetPush(nugetFile.ToString(), new DotNetNuGetPushSettings
    {
      Source = nugetRepository,
      ApiKey = nugetApiKey,
      SkipDuplicate = true
    });
  }
});

Task("DockerPush")
  .IsDependentOn("DockerBuild")
  .Does(() =>
  {
    DockerLogin(dockerRepositoryUsername, dockerRepositoryPassword, dockerRepository);
    for (int i = 0; i < DockerFiles.Length; i++)
    {
      var outputImage = OutputImages[i];
      DockerPush($"{DockerRepositoryPrefix}{outputImage}:{NuGetVersionV2}");
    }
  });

Task("Format")
  .Does(() =>
  {
    StartProcess("dotnet-format", new ProcessSettings
    {
      Arguments = new ProcessArgumentBuilder()
            .Append(SolutionFileName)
            .Append("--severity")
            .Append("warn")
            .Append("--verbosity")
            .Append("diagnostic")
    });
  });

RunTarget(target);