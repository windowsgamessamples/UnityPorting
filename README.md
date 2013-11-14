Unity Porting Code Samples
==========================

-  Platformer - Sample Unity Game
-  PlatformerApps - Sample Windows Store and Windows Phone Apps along with Plugin Examples
-  Resources -  Classes to help overcome .Net Core compilation errors in Unity for Windows Store, along with a package to highlight shader issues on Windows

**Sample Unity App - Platformer**

This is the sample Unity game project you can simply open with Unity 4.3.

Here's some guidance as to the most interestin features that have been added

Windows Store and Windows Phone specific scripts are kept here:

/Assets/Scripts/Windows

- Windows specific handlers and direct interop code (no plugins)
/Assets/Scripts/Windows/WindowsGateway.cs

WACK overrides (with some plugin usage)
/Assets/Scripts/Windows/WACK

Facebook Management (using plugin):
/Assets/Scripts/FacebookManager.cs

Sharing (using plugin):
/Assets/Scripts/ShareManager.cs

This approach means you don't have to change ANY game code which relies on these unsupported APIs.

**Building to Windows Store App from Unity**

You must do this before the Windows Store app will work.

- File > Build Settings > Windows Store. 
- Select Xaml/C# and 8.1
- Build out on top of \PlatformerApps\PlatformerWindowsStore folder

**Building Windows Phone App from Unity**

You must do this before the Windows Phone app will work.

- File > Build Settings > Windows Phone
- Select Xaml/C# 
- Build out on top of \PlatformerApps\PlatformerWindowsPhone folder

**The Windows Solution**

Open \UnityPorting\PlatformerApps\Platformer.sln with Visual Studio 2013. 

There are 2 app projects for the Windows Store and WP8 apps:

- PlatfomerWindows - The Windows 8.1 App
- PlatfomerWP8 - The Windows Phone 8 App
 
In addition, there are 3 projets for the sample "MyPlugin" plugin.

- MyPluginUnity - Unity Editor plugin project
- MyPluginWindows - Windows 8.1 plugin project
- MyPluginWP8 - WP8 plugin project

**Running the Windows Store App**

- Make sure you build out from Unity as above "Building to Windows Store App from Unity"
- Ensure PlatfomerWindows is set as the startup project
- Then simply F5 the solution and it shoudl run!

**Running the Windows Phone App**

- Make sure you build out from Unity as above "Building to Windows Phone App from Unity"
- Ensure PlatfomerWP8 is set as the startup project
- Then simply F5 the solution and it should run!

**Updating the Sample Windows Plugin**

The MyPlugin plugin is already include in the Unity project source. 

When updating the source of any of the plugin projects (MyPluginUnity, MyPluginWindows or MyPluginWP8), if you ensure 
that you set the build configuration for "Release" and "Any CPU", there is a post build event that will automatically 
copy the resulting MyPlugin.dll to the correct locations in the Unity project automatically as follows:

- MyPluginUnity Project > /Assets/Plugins/MyPlugin.dll 
- MyPluginWindows Project > /Assets/Plugins/Metro/MyPlugin.dll
- MyPluginWP8 Project > /Assets/Plugins/WP8/MyPlugin.dll



