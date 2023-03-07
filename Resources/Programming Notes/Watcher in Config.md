If you don't want to write the "dotnet watch run" command every time, you can create a new profile in the launchSettings.json file. You can find this file in your Blazor project (Organize.WASM) in the Properties folder:
￼

Open the launchSettings.json file. Add a new profile in the profiles section. Use the following code:
* 		"Watch": {
* 		"commandName": "Executable",
* 		"launchBrowser": true,
* 		"launchUrl": "http://localhost:5000/",
* 		"commandLineArgs": "watch run",
* 		"workingDirectory": "$(ProjectDir)",
* 		"executablePath": "dotnet.exe",
* 		"environmentVariables": {
* 		"ASPNETCORE_ENVIRONMENT": "Development"
* 		}
* 		}

After that change, your launchSettings.json file should look like the following image:
￼

Now you are able to start the app together with the watcher. Pay attention that you choose the right launch config:
￼

Take Watch here (the name of the profile in the launchSettings.json file). After the start of the app, you can change your code and the app will be refreshed, and you see the new state of your Blazor application.
