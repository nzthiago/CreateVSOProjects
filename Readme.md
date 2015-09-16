# Create Visual Studio Online Projects
This code is a sample that connects to the Visual Studio Online REST API to create projects.

It was written fairly quickly so pull requests with improvements and fixes welcome!

## Running the sample
From Visual Studio create a local AppSettingsSecrets.config file (not included in this code) with the app settings one folder up from the solution folder. Here's an example content for the AppSettingsSecrets.config file:

```xml
<?xml version="1.0" encoding="utf-8" ?>
  <appSettings>
    <add key="user" value="<your VSO alternate login>" />
    <add key="password" value="<your VSO alternate password>" />
    <add key="url" value="<your VSO account>.visualstudio.com" />
</appSettings>
```

Modify the values in that file with your own VSO account and credentials. Steps for setting up the credentails can be found here:
VSO Alternate Authentication Credential 
https://www.visualstudio.com/integrate/get-started/auth/overview

Modify the _projectNames list at the top of the Program class with the list of projects you want to create.

It defaults to Scrum as the template and Git as the repository type, but you can change that in the code.