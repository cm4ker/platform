'use strict';

export var defaultTasksJson =
{
  // See https://go.microsoft.com/fwlink/?LinkId=733558
  // for the documentation about the tasks.json format
  "version": "2.0.0",
  "tasks": [
    {
      "label": "build",
            "command": "dotnet",
            "type": "process",
            "args": [
                "build"               
                ],
            "problemMatcher": "$msCompile"
    },
    {
      "label" : "deploy",
      "command" : "aq",
      "type" : "process",
      "args" : ["deploy", "-pkg \"${workspaceRoot}/bin/Debug/net6.0/Package/${workspaceFolderBasename}.aqpk\"", "-e 127.0.0.1:5000" , "-i ${workspaceFolderBasename}"],
      "dependsOn": ["build"],
    },
    {
      "label" : "migrate",
      "command" : "aq migrate",
      "type" : "process",
      "args" : ["-e 127.0.0.1:5000" , "-i ${workspaceFolderBasename}"] ,
      "dependsOn": ["build"],
    }
  ]
};

export var defaultLaunchJson =
{
  "version": "0.2.0",
  "configurations": [
    {
      "name": ".NET Core Launch (console)",
      "type": "coreclr",
      "request": "launch",
      "preLaunchTask": "build",
      "justMyCode": false,
      "program": "${workspaceRoot}/bin/Debug/net6.0/${workspaceFolderBasename}.dll",
      "args": [],
      "cwd": "${workspaceRoot}",
      "externalConsole": false,
      "stopAtEntry": false,
      "internalConsoleOptions": "openOnSessionStart"
    },
    {
      "name": ".NET Core Attach",
      "type": "coreclr",
      "request": "attach",
      "processId": "${command.pickProcess}"
    }
  ]
};