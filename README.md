# Unity Porting Code Samples

## Read the Whitepapers

This repo has been created as reference code samples for a set of whitepapers produced to help developers in 
porting their games to Windows 8.1 and Windows Phone 8.

It is recommended that you read these whitepapers before delving into the code so you can unde

- [Getting Started on Windows Store with Unity](http://aka.ms/unityWinStoreStart)
- [Porting tips for Windows Store with Unity](http://aka.ms/unityWinStoreTips)
- [Getting Started on Windows Phone with Unity](http://aka.ms/unitywpstart)
- [Porting tips for Windows Phone with Unity](http://aka.ms/unityWPTips)

## How do I open the Box?

The prerequisites for working with this repo are...

**[Unity 4.3](http://unity3d.com/unity/download)**

Either the Unity free version or Unity Pro will work.  
The add-ons for publishing to the Windows Store and to Windows Phone are free, for basic and Unity Pro users. 

**[Visual Studio .Net 2013](http://www.microsoft.com/visualstudio/eng/downloads)**

You can use any Visual Studio 2013 SKU, including the free Visual Studio Express.   

**Windows 8.1**

If you do not own a Windows 8 license, you can get a [90-day evaluation version](http://msdn.microsoft.com/en-US/evalcenter/jj554510.aspx?wt.mc_id=MEC_132_1_4).  
If you are running Mac OS X or will install on Apple hardware, 
check different options for installing using [Boot Camp](http://msdn.microsoft.com/en-us/library/windows/apps/jj945423.aspx), [VMWare](http://msdn.microsoft.com/en-us/library/windows/apps/jj945426.aspx), or [Parallels](http://msdn.microsoft.com/en-us/library/windows/apps/jj945424.aspx).   

**[Microsoft account](http://signup.live.com/)*
You will need a free Microsoft account to get a developer license.  

To submit your games to the store you will need..

**Windows Store and Windows Phone developer account.**
Get an account at the [Windows store](http://dev.windows.com) or [Windows Phone Marketplace](http://dev.windowsphone.com). 

This registration is shared with Windows Phone (one registration submits to both stores). 

During this process you will register and get verified as an individual or as a business who can submit apps and games to the store.  

Registration is $19 for individuals

## What's in the Box?

This repo contains the following folders:

-  *Platformer* - Sample Unity 4.3 Game freely available on the Unity Store
-  *PlatformerApps* - Vs.net 2013 solution folder with Windows Store and Windows Phone Apps along with Plugin Example
-  *Resources* -  Packages to highlight current shader issues on Windows

In general terms, this repo adds "light up" features which demonstrate many of the approaches outlined in the above 
whitepapers. 

There are currently the following examples in the game for both Windows 8.1 and Windows Phone 8

- *Graceful loading* with extended splash and progress bar
- *Sharing* support using sample plugin
- *Live tile* updates with latest score
- *Complete Plugin Development Sample* with shared code across both Windows 8.1 and Wndows Phone 8

Current the following examples in the game are just for Windows 8.1

- *WACK fixers* (Collections, IO, Sockets, Threading, Missing Extensions, with more coming soon)
- *Facebook Integration* (Login, Logout, Friend Request) (Windows 8.1 only)
- *Window Resizing sample*, pause/resume at 500px (Windows 8.1 only, Windows Phone 8 coming soon)

## Sample Unity App - Platformer

This is the sample Unity game project you can simply open with Unity 4.3.

Here's some guidance as to the most interesting features that have been added

- Windows Store and Windows Phone specific scripts > /Assets/Scripts/Windows
- Windows specific handlers and direct interop code > /Assets/Scripts/Windows/WindowsGateway.cs
- WACK overrides > /Assets/Scripts/Windows/WACK
- Facebook Management > /Assets/Scripts/FacebookManager.cs
- Sharing > /Assets/Scripts/ShareManager.cs

You will find that the sample "MyPlugin" plugin is used a lot within the Unity project, more on this below

## Building to Windows Store App from Unity

You must do this before the Windows Store app will work.

- File > Build Settings > Windows Store. 
- Select Xaml/C# and 8.1
- Build out on top of \PlatformerApps\PlatformerWindowsStore folder

## Building Windows Phone App from Unity

You must do this before the Windows Phone app will work.

- File > Build Settings > Windows Phone
- Select Xaml/C# 
- Build out on top of \PlatformerApps\PlatformerWindowsPhone folder

## The Windows Solution

Open \UnityPorting\PlatformerApps\Platformer.sln with Visual Studio 2013. 

There are 2 app projects for the Windows Store and WP8 apps:

- PlatfomerWindows - The Windows 8.1 App
- PlatfomerWP8 - The Windows Phone 8 App
 
In addition, there are 3 projets for the sample "MyPlugin" plugin.

- MyPluginUnity - Unity Editor plugin project
- MyPluginWindows - Windows 8.1 plugin project
- MyPluginWP8 - WP8 plugin project

## Running the Windows Store App

- Make sure you build out from Unity as above "Building to Windows Store App from Unity"
- Ensure PlatfomerWindows is set as the startup project
- Then simply F5 the solution and it shoudl run!

## Running the Windows Phone App

- Make sure you build out from Unity as above "Building to Windows Phone App from Unity"
- Ensure PlatfomerWP8 is set as the startup project
- Then simply F5 the solution and it should run!

## Updating the Sample Windows Plugin

The MyPlugin plugin is already include in the Unity project source. 

When updating the source of any of the plugin projects (MyPluginUnity, MyPluginWindows or MyPluginWP8), if you ensure 
that you set the build configuration for "Release" and "Any CPU", there is a post build event that will automatically 
copy the resulting MyPlugin.dll to the correct locations in the Unity project automatically as follows:

- MyPluginUnity Project > /Assets/Plugins/MyPlugin.dll 
- MyPluginWindows Project > /Assets/Plugins/Metro/MyPlugin.dll
- MyPluginWP8 Project > /Assets/Plugins/WP8/MyPlugin.dll



