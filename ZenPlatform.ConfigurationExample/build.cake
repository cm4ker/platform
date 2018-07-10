var target = Argument("target", "Default");

var configuration = Argument("configuration", "Debug");


//////////////////////////////////////////////////////////////////////
// PREPARATION
//////////////////////////////////////////////////////////////////////

// Define directories.
var buildDir = Directory("../Build/") + Directory(configuration) + Directory("ExampleConfiguration");
//var targetDir = Directory("netcoreapp2.1");
var confDir = buildDir + Directory("Configuration");

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
    //if(IsRunningOnWindows())
    //{
      // Use MSBuild
      MSBuild("./ZenPlatform.ConfigurationExample.csproj", settings =>
        settings.SetConfiguration(configuration));
    //}
    //else
    //{
      // Use XBuild
    //  XBuild("./ZenPlatform.ConfigurationExample.csproj", settings =>
    //    settings.SetConfiguration(configuration));
    //}
});

Task("Default").IsDependentOn("Build")
  .Does(() =>
{
  CreateDirectory(confDir + Directory("Components"));
  CopyFile(buildDir + File("ZenPlatform.DataComponent.dll"), confDir + Directory("Components") + File("ZenPlatform.DataComponent.dll"));
  CopyFile(buildDir + File("ZenPlatform.EntityComponent.dll"), confDir + Directory("Components") + File("ZenPlatform.EntityComponent.dll"));
  CopyFile(buildDir + File("ZenPlatform.Configuration.dll"), confDir + Directory("Components") + File("ZenPlatform.Configuration.dll"));

  //Copy pdb files for debug
  CopyFile(buildDir + File("ZenPlatform.DataComponent.pdb"), confDir + Directory("Components") + File("ZenPlatform.DataComponent.pdb"));
  CopyFile(buildDir + File("ZenPlatform.EntityComponent.pdb"), confDir + Directory("Components") + File("ZenPlatform.EntityComponent.pdb"));
  CopyFile(buildDir + File("ZenPlatform.Configuration.pdb"), confDir + Directory("Components") + File("ZenPlatform.Configuration.pdb"));

  
  Information("Build done");
});

RunTarget(target);