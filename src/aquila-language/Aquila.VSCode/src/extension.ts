'use strict';
// The module 'vscode' contains the VS Code extensibility API
// Import the module and reference it with the alias vscode in your code below
import * as vscode from 'vscode';

import * as path from 'path';
import * as fs from 'fs';
import * as cp from 'child_process';

import { defaultTasksJson, defaultLaunchJson } from './defaults';
import {
    LanguageClient,
    LanguageClientOptions,
    SettingMonitor,
    ServerOptions,
    TransportKind,
    InitializeParams,
    StreamInfo,
    createServerPipeTransport,
} from "vscode-languageclient/node";
import { workspace } from "vscode";

import { XMLHttpRequest, Document } from 'xmlhttprequest';

let channel: vscode.OutputChannel;

// this method is called when your extension is activated
// your extension is activated the very first time the command is executed
export async function activate(context: vscode.ExtensionContext) {
    channel = vscode.window.createOutputChannel("AqLang");
    channel.appendLine("AqLang extension was activated.");

    context.subscriptions.push(
        channel,

         vscode.commands.registerCommand('aqlang.createproject', async () => {
            await createTemplate("project");
        }),

        startLanguageServer(context)
    );
}

function startLanguageServer(context: vscode.ExtensionContext): vscode.Disposable {
    // TODO: Handle the proper publishing of the executable
    let serverPath = context.asAbsolutePath("out/server/Aquila.LanguageServer.dll");
    let serverOptions: ServerOptions = {
        run: { command: "dotnet", args: [serverPath] },
        debug: { command: "dotnet", args: [serverPath, "--debug"] }
    }

    // Options to control the language client
    let clientOptions: LanguageClientOptions = {
        // Register the server for Aquila documents
        documentSelector: [
            {
                pattern: "**/*.aq",
            },
            {
                pattern: "**/*.yaml",
            },
            {
                pattern: "**/*.md",
            },
        ],
        progressOnInitialization: true,
        synchronize: {
            configurationSection: "aqLanguageServer",
            // Notify the server when running dotnet restore on a project in the workspace
            fileEvents: [
                workspace.createFileSystemWatcher("**/*.aq"),
                workspace.createFileSystemWatcher('**/*.aqproj')
            ]
        }
    }

    // Create the language client and start the server
    return new LanguageClient("Aquila Language Server", serverOptions, clientOptions).start();
}

function showInfo(message: string, doShowWindow = false) {
    channel.appendLine(message);
    if (doShowWindow) {
        vscode.window.showInformationMessage(message);
    }
}

function showError(message: string, doShowWindow = true) {
    channel.appendLine(message);
    if (doShowWindow) {
        vscode.window.showErrorMessage(message);
    }
}

async function createTemplate(templatename: string) {
    // We will write successes to the output channel. In case of an error, we will display it also
    // in the window and skip the remaining operations.
    channel.show(true);

    // Check the opened folder
    let rootPath = vscode.workspace.rootPath;
    if (rootPath != null) {
        showInfo(`Creating Aquila project in '${rootPath}' ...\n`);
    } else {
        showError("A folder must be opened in the Explorer panel.\n");
        return;
    }

    // const templatename = "console"
    const templatefilename = templatename + ".msbuildproj"

    // Create .msbuildproj project file:
    let projectPath = path.join(rootPath, templatefilename);
    showInfo(`Creating '${templatename}' ...`);
    if (fs.existsSync(projectPath)) {
        showInfo(`Warning: project file already exists, won't be created.`);
    } else {
        if (await createProjectFile(projectPath, templatefilename)) {
            showInfo("Project file created successfully.");
        } else {
            showError("Error in creating project file.\n");
            return;
        }
    }

    // Create or update .tasks.json and .launch.json
    showInfo("Configuring build and debugging in 'tasks.json' and 'launch.json' ...");
    let isTasksSuccess =
        await configureTasks() &&
        (templatename != "console" || await configureLaunch());
    
    if (isTasksSuccess) {
        showInfo("Build tasks successfully configured.\n");
    } else {
        showError("Error in configuring the build tasks.\n");
        return;
    }

    // Run dotnet restore
    let isError = false;
    showInfo("Running dotnet restore to install Aquila Sdk ...");
    await execChildProcess("dotnet restore", rootPath)
        .then((data: string) => {
            showInfo(data);
            if (data.includes("Restore completed in")) {
                showInfo("Project dependencies were successfully installed.\n");
            } else {
                showError("Error in installing project dependencies.\n");
                isError = true;
            }
        })
        .catch((error) => {
            showError("For building and executing, Aquila needs .NET Core CLI tools to be available on the path. Make sure they are installed properly.\n");
            isError = true;
        });
    if (isError) {
        return;
    }

    // Activate Omnisharp C# extension for debugging
    let csharpExtension = vscode.extensions.getExtension("ms-vscode.csharp");
    if (csharpExtension == null) {
        showError("Install OmniSharp C# extension in order to enable the debugging of Aquila projects.\n");
        return;
    } else {
        if (csharpExtension.isActive) {
            showInfo("OmniSharp C# extension is already active.\n");
        } else {
            showInfo("Activating OmniSharp C# extension to take care of the project structure and debugging ...\n");
            await csharpExtension.activate();
        }
        showInfo("Aquila project was successfully configured.", true);
    }
}

// Create project file in the opened root folder
async function createProjectFile(filePath: string, templateFile: string): Promise<boolean> {
    let projectUri = vscode.Uri.parse(`untitled:${filePath}`);
    let projectDocument = await vscode.workspace.openTextDocument(projectUri);
    let extensionDir = vscode.extensions.getExtension("aqlang-vscode").extensionPath;
    let projectContent = fs.readFileSync(extensionDir + "/templates/" + templateFile).toString();
    let projectEdit = vscode.TextEdit.insert(new vscode.Position(0, 0), projectContent);

    let wsEdit = new vscode.WorkspaceEdit();
    wsEdit.set(projectUri, [projectEdit]);
    let isSuccess = await vscode.workspace.applyEdit(wsEdit);

    if (isSuccess) {
        isSuccess = await vscode.workspace.saveAll(true);
    }

    return isSuccess;
}

// Overwrite tasks configuration, resulting in adding or replacing .vscode/tasks.json
async function configureTasks(): Promise<boolean> {
    return overwriteConfiguration("tasks", defaultTasksJson);
}

// Overwrite launch configuration, resulting in adding or replacing .vscode/tasks.json
async function configureLaunch(): Promise<boolean> {
    return overwriteConfiguration("launch", defaultLaunchJson);
}

async function overwriteConfiguration(section: string, configuration: any): Promise<boolean> {
    let tasksConfig = vscode.workspace.getConfiguration(section);
    if (tasksConfig == null) {
        channel.appendLine(`Unable to load ${section} configuration`);
        return false;
    }

    try {
        for (var key in configuration) {
            if (configuration.hasOwnProperty(key)) {
                var element = configuration[key];

                // Not defined in the Typescript interface, therefore called this way
                await tasksConfig['update'].call(tasksConfig, key, element);
            }
        }
    } catch (error) {
        channel.appendLine("Error in configuring the build tasks: " + (<Error>error).message);
        return false;
    }

    return true;
}

// Taken from omnisharp-vscode
function execChildProcess(command: string, workingDirectory: string): Promise<string> {
    return new Promise<string>((resolve, reject) => {
        cp.exec(command, { cwd: workingDirectory, maxBuffer: 500 * 1024 }, (error, stdout, stderr) => {
            if (error) {
                reject(error);
            }
            else if (stderr && stderr.length > 0) {
                reject(new Error(stderr));
            }
            else {
                resolve(stdout);
            }
        });
    });
}

// this method is called when your extension is deactivated
export function deactivate() {
}