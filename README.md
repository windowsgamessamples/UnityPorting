# Unity Porting Code Samples

## Read the Whitepapers

This repo has been created as reference code samples for a set of whitepapers produced to help developers in 
porting their Unity games to Windows 8.1 and Windows Phone 8.

It is recommended that you read these whitepapers before delving into the code.

- [Getting Started on Windows Store with Unity](http://aka.ms/unityWinStoreStart)
- [Porting tips for Windows Store with Unity](http://aka.ms/unityWinStoreTips)
- [Getting Started on Windows Phone with Unity](http://aka.ms/unitywpstart)
- [Porting tips for Windows Phone with Unity](http://aka.ms/unityWPTips)


The prerequisites for working with these samples are in either of the Getting Started documents above. 

## What's in the repo?

Small demonstrations and code snippets for most of the approaches outlined in the papers.  

This repo contains the following folders:

-  **Platformer** - Unity 4.3 2D game freely available on the Unity Store. We use it as a base to add concepts. 
-  **PlatformerApps** - Vs.net 2013 solution folder with Windows Store and Windows Phone Apps along with plugin example
-  **Resources** -  Packages to highlight current shader issues on Windows


There are currently the following tasks in the game for both Windows 8.1 and Windows Phone 8

- **Graceful loading** is an extended splash and progress bar
- **Sharing** support using sample plugin
- **Live tile** updates with latest score
- **Complete Plugin Development Sample** with shared code across both Windows 8.1 and Windows Phone 8

For Windows 8.1, there are a couple of extra snippets 

- **WACK fixers** has working implementations of missing classes in .NET Core. Today, it has Collections, IO, Sockets, Threading, and other useful class extensions. There is more coming soon. 
- **Facebook Integration** (Login, Logout, Friend Request) (Windows 8.1 only)

- **Window Resizing sample**, pause/resume at 500px (Windows 8.1 only)

- **Orientation Change Support** Via the sample plugin, Unity can respond to orientation changes
- **Xaml Textbox Overlay** Via the sample plugin, Unity can show a XAML textbox as an overlay to get keyboard input from soft (aka touch) keyboard.  


# Building the solution #

The first time you build, you need to follow specific order:

1. Build to Windows Store and Windows Phone in Unity 4.3 (over top of projects in the Windows solution folder)
2. Build the Windows Solution (which will automatically add Nuget dependencies)
3. Run the Windows Store or Windows Phone App!

See below for tips on each step:  

## Building the Windows Store Player in Unity 

In Unity, build the Player 

- File > Build Settings > Windows Store. 
- Select Xaml/C# and 8.1
- Build out to PlatformerApps\PlatformerWindowsStore folder

Note: If you make changes to Unity scripts and game, you can just keep building to PlatformerWindowsStore project and it won't override your Visual studio project. 
If you  add new plugins or change the player preferences, you will need to merge these manually.    

## Building Windows Phone App from Unity

- File > Build Settings > Windows Phone
- Select Xaml/C# 
- Build out on top of \PlatformerApps\PlatformerWindowsPhone folder

Note: If you make changes to Unity scripts and game, you can just keep building to PlatformerWindowsPhone project and it won't override your Visual studio project. 
If you  add new plugins or change the player preferences, you will need to merge these manually.  


## Building and running for Windows Store

- Open \UnityPorting\PlatformerApps\Platformer.sln with Visual Studio 2013. 
- You must have the [NuGet](http://www.nuget.org/) packet manager installed.<br/>
- Build the solution to ensure all NuGet packages are available (e.g. Facebook) as the solution is set to automatically restore NuGet packages.
- Ensure PlatfomerWindows is set as the startup project
- Ensure that configuration for the build matches your target device (e.g. Master | x86)
- Then simply F5 the solution and it will run!

## Building and running for Windows Phone


- Open \UnityPorting\PlatformerApps\Platformer.sln with Visual Studio 2013. 
- You must have the [NuGet](http://www.nuget.org/) packet manager installed.<br/>
- Build the solution to ensure all NuGet packages are available (e.g. Facebook) as the solution is set to automatically restore NuGet packages.
- Make sure you build out from Unity as above "Building to Windows Phone App from Unity"
- Ensure PlatfomerWP8 is set as the startup project
- Then simply F5 the solution and it should run!

## Updating the Sample Plugin

The plugin binary outputs are included in the Unity project automatically, but you can update and easily have Unity get the updated plugins.

To build the plugin, open the \UnityPorting\PlatformerApps\Platformer.sln with Visual Studio 2013 
and set the build configuration for "Release" and "Any CPU", there is a post build event that will automatically copy the resulting MyPlugin.dll (and any adjacent dlls in the target directory) to the correct locations in the Unity project automatically as follows:

- MyPluginUnity Project > /Assets/Plugins/MyPlugin.dll 
- MyPluginWindows Project > /Assets/Plugins/Metro/MyPlugin.dll
- MyPluginWP8 Project > /Assets/Plugins/WP8/MyPlugin.dll

Note: If you make changes to the plugins later, every time you update the plugin, you should rebuild in Unity and Visual Studio. 

### Upcoming features  ###
We are just getting started. Today, it is a sample and lots of useful snippets you can copy and paste into your projects.  We need  to: 
- Refactor into something more reusable outside of the sample; we will also provide more explanations.   
- Create guidance papers on getting ready for certification, performance and troubleshooting.  

### Known issues ###

With Unity 4.3, we are seeing a windowing focus problem. If you have multiple monitors and the game is not getting focus, drag it as if you were going to move the window or close the game so focus is restored.  
