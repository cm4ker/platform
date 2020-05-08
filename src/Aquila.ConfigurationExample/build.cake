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
    NuGetRestore("./Aquila.ConfigurationExample.csproj");
});


Task("Build")
    .IsDependentOn("Restore-NuGet-Packages")
    .Does(() =>
{
    //if(IsRunningOnWindows())
    //{
      // Use MSBuild
      MSBuild("./Aquila.ConfigurationExample.csproj", settings =>
        settings.SetConfiguration(configuration));
    //}
    //else
    //{
      // Use XBuild
    //  XBuild("./Aquila.ConfigurationExample.csproj", settings =>
    //    settings.SetConfiguration(configuration));
    //}
});

Task("Default").IsDependentOn("Build")
  .Does(() =>
{
  CreateDirectory(confDir + Directory("XCComponent"));
  CopyFile(buildDir + File("Aquila.DataComponent.dll"), confDir + Directory("XCComponent") + File("Aquila.DataComponent.dll"));
  CopyFile(buildDir + File("Aquila.EntityComponent.dll"), confDir + Directory("XCComponent") + File("Aquila.EntityComponent.dll"));
  CopyFile(buildDir + File("Aquila.Configuration.dll"), confDir + Directory("XCComponent") + File("Aquila.Configuration.dll"));

  //Copy pdb files for debug
  CopyFile(buildDir + File("Aquila.DataComponent.pdb"), confDir + Directory("XCComponent") + File("Aquila.DataComponent.pdb"));
  CopyFile(buildDir + File("Aquila.EntityComponent.pdb"), confDir + Directory("XCComponent") + File("Aquila.EntityComponent.pdb"));
  CopyFile(buildDir + File("Aquila.Configuration.pdb"), confDir + Directory("XCComponent") + File("Aquila.Configuration.pdb"));

  
  Information("Build done");
});

RunTarget(target);