var target = Argument("target", "Default");

var configuration = Argument("configuration", "Debug");


//////////////////////////////////////////////////////////////////////
// PREPARATION
//////////////////////////////////////////////////////////////////////

// Define directories.
var buildDir = Directory("./bin") + Directory(configuration);
var targetDir = Directory("netcoreapp2.1");
var confDir = buildDir + targetDir + Directory("Configuration");

Task("Clean")
    .Does(() =>
{
    CleanDirectory(buildDir);
});


Task("Restore-NuGet-Packages")
    .IsDependentOn("Clean")
    .Does(() =>
{
    NuGetRestore("./ZenPlatform.ConfigurationExample.csproj");
});


Task("Build")
    .IsDependentOn("Restore-NuGet-Packages")
    .Does(() =>
{
    if(IsRunningOnWindows())
    {
      // Use MSBuild
      MSBuild("./ZenPlatform.ConfigurationExample.csproj", settings =>
        settings.SetConfiguration(configuration));
    }
    else
    {
      // Use XBuild
      XBuild("./ZenPlatform.ConfigurationExample.csproj", settings =>
        settings.SetConfiguration(configuration));
    }
});

Task("Default").IsDependentOn("Build")
  .Does(() =>
{
  CreateDirectory(confDir + Directory("Components"));
  CopyFile(buildDir + targetDir +File("ZenPlatform.DataComponent.dll"), confDir + Directory("Components") + File("ZenPlatform.DataComponent.dll"));
  CopyFile(buildDir + targetDir +File("ZenPlatform.EntityComponent.dll"), confDir + Directory("Components") + File("ZenPlatform.EntityComponent.dll"));
  
  Information("Build done");
});

RunTarget(target);