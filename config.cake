var buildConfiguration = Argument("Configuration", "Release");

string NuGetVersionV2 = "";
string SolutionFileName = "src/Tempore.sln";

string[] DockerFiles = new[]
{
	"./deployment/docker/Tempore.Server/Dockerfile",
};

string[] OutputImages = new[]
{
	"tempore-server",
};

string[] ComponentProjects = new string[]
{
	// "./src/Tempore.Configuration/Tempore.Configuration.csproj",
	// "./src/Tempore.Hosting/Tempore.Hosting.csproj",
	// "./src/Tempore.Storage/Tempore.Storage.csproj",
	// "./src/Tempore.Storage.PostgreSQL/Tempore.Storage.PostgreSQL.csproj",
};

string[] ExecProjects = new[]
{
	"./src/Tempore.Agent/Tempore.Agent.csproj",
	"./src/Tempore.Agent.Setup/Tempore.Agent.Setup.csproj",
};

var ExtraFiles = new System.Collections.Generic.Dictionary<string, string>();

// Extra file for Tempore.Agent
ExtraFiles.Add($"src/Tempore.Agent/bin/{buildConfiguration}/net6.0/Serilog.Enrichers.Thread.dll", $"output/exec/Tempore.Agent/Serilog.Enrichers.Thread.dll");
ExtraFiles.Add($"src/Tempore.Agent/bin/{buildConfiguration}/net6.0/Serilog.Enrichers.ShortTypeName.dll", $"output/exec/Tempore.Agent/Serilog.Enrichers.ShortTypeName.dll");
ExtraFiles.Add($"src/Tempore.Agent/bin/{buildConfiguration}/net6.0/Serilog.Enrichers.Environment.dll", $"output/exec/Tempore.Agent/Serilog.Enrichers.Environment.dll");
ExtraFiles.Add($"deployment/config/Tempore.Agent/appsettings.json", $"output/exec/Tempore.Agent/appsettings.json");

// Extra file for Tempore.Agent.Setup
ExtraFiles.Add($"src/Tempore.Agent.Setup/bin/{buildConfiguration}/net6.0/Serilog.Enrichers.Thread.dll", $"output/exec/Tempore.Agent.Setup/Serilog.Enrichers.Thread.dll");
ExtraFiles.Add($"src/Tempore.Agent.Setup/bin/{buildConfiguration}/net6.0/Serilog.Enrichers.ShortTypeName.dll", $"output/exec/Tempore.Agent.Setup/Serilog.Enrichers.ShortTypeName.dll");
ExtraFiles.Add($"src/Tempore.Agent.Setup/bin/{buildConfiguration}/net6.0/Serilog.Enrichers.Environment.dll", $"output/exec/Tempore.Agent.Setup/Serilog.Enrichers.Environment.dll");
ExtraFiles.Add($"deployment/config/Tempore.Agent.Setup/appsettings.json", $"output/exec/Tempore.Agent.Setup/appsettings.json");

var ZipFiles = new System.Collections.Generic.Dictionary<string, (string Path, string Pattern, string[] ExclusionPatterns)>();

// Extra file for Tempore.Agent
ZipFiles.Add("Tempore.Agent", ($"output/exec/Tempore.Agent", "**/*.*", new string[] { ".+\\.pdb\\b", ".+\\.xml\\b", "web.config", "log.+\\.txt\\b" }));
ZipFiles.Add("Tempore.Agent.Setup", ($"output/exec/Tempore.Agent.Setup", "**/*.*", new string[] { ".+\\.pdb\\b", ".+\\.xml\\b", "web.config", "log.+\\.txt\\b" }));

string[] TestProjects =
{
	"./src/Tempore.Tests/Tempore.Tests.csproj"
};

var NuGetFiles = new System.Collections.Generic.Dictionary<string, string>();
NuGetFiles.Add("Tempore.Agent", "./deployment/nuget/Tempore.Agent/Tempore.Agent.nuspec");
NuGetFiles.Add("Tempore.Agent.Setup", "./deployment/nuget/Tempore.Agent.Setup/Tempore.Agent.Setup.nuspec");

string SonarProjectKey = "Tempore";
string SonarOrganization = "PHI";