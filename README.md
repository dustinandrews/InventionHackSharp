# InventionHack
The C# Roguelike built on top of the [Invention API](https://gitlab.com/hodgskin-callan/Invention)  by Callan Hodgskin and [RogueSharp](https://github.com/FaronBracy/RogueSharp) by FaronBracy.
## The Mission
Use the Invention API to create a basic roguelike with all the features of the [Roguesharp C# tutorial](https://roguesharp.wordpress.com/) and put it on github with an MIT license as a jumping off point for aspiring roguelike creators.

## Features

 - Basic roguelike gameplay
 - Line of site
 - Random floor creation
 - Basic combat
 - Speed system
 - RDBMS style ECS system
 - UI is loosely coupled to the underlying game engine
 - Builds with the dotnet.exe command
 - Not too buggy!
 - Invention is a very good UI engine and the starter game only scratches the surface of whats possible with it.

## Cloning, Building and running
There may be other valid ways to build this.
#### Pre-Reqs

 - Visual Studio 2017/2019 community edition
 - [Dotnet 4.6 developer pack](https://www.microsoft.com/en-us/download/details.aspx?id=53321)
 - Optional: [Visual Studio Code](https://code.visualstudio.com/)

#### Clone

Change to a directory where you like to clone stuff.

    C:\local> git clone https://github.com/dustinandrews/InventionHackSharp.git
    C:\local> cd InventionHackSharp
    
    
#### Build and test
    C:\local\InventionHackSharp>dotnet build
    ...
    C:\local\InventionHackSharp\bin\Debug\EntityComponentSystemCSharp\net4.6.1\EntityComponentSystemCSharp.dll
    Engine -> C:\local\InventionHackSharp\bin\Debug\Engine\net461\Engine.dll
    Portable -> C:\local\InventionHackSharp\bin\Debug\Portable\net461\Portable.dll
    InventionUiWpf -> C:\local\InventionHackSharp\bin\Debug\InventionUiWpf\net461\InventionUiWpf.exe
    
    Build succeeded.
        0 Warning(s)
        0 Error(s)
        
    C:\local\InventionHackSharp>dotnet test
    ...
        Test Run Successful.
    Total tests: 20
         Passed: 20
     Total time: 1.5644 Seconds
    
    Test Run Successful.
    Total tests: 4
         Passed: 4
     Total time: 4.6779 Seconds
     
#### Run the game
    C:\local\InventionHackSharp\bin\Debug\InventionUiWpf\net461\InventionUiWpf.exe

## Tour
If you think this is cool and want to fork the repo to create your own roguelike please help yourself! Here is a brief description of the projects inside.
#### Engine
This is the game logic. It manages the game turns, maps and sits on top of the ECS. It also manages player specific stuff.
#### EntityComponentSystemCSharp
This is the Entity Component System that drives the game. Components are bare-bones C# classes with zero or more data members. The game engine calls every system for every entity each turn and gives it an opportunity to act. Systems filter for appropriate entities and change the data as appropriate.

### FeatureDetector
Reads dungeon maps and flags walls and candidate locations for doors. It relies on [NumSharp](https://github.com/SciSharp/NumSharp) and uses a convolution filter. NumSharp is the C# port of numpy from Python. Things that are a breeze and super fast with numpy can be a downright chore in C#. NumSharp is still in beta and it shows. I plan to extend the convolution filter for use in cellular automata level generation at some point.

### InventionUiWpf
This is the Windows WPF project that makes the UI work on that platform. Right now it's the only supported platform, but the other ones shouldn't be too hard to get working. I want to keep compatibility with dotnet.exe and vscode, so I haven't tried to integrate them.

### lib
This is a custom build of the Invention API for x64 since the current nuget packages were x86 and vscode can only debug x64 binaries.
### Portable (AKA the Portable UI)
This is the platform agnostic UI code that displays the game and ties the user to the engine.

## Pull requests
I am happy to recieve any pull requests and consider them. As the point of this repository is to be a clean starting point, new features are (probably) not going to be added. Any pull requests should:

 - Fix a bug
 - Make the code cleaner and easier to understand
 - Add unit tests that will either help downstream authors, be explanatory code, or both
 - Anything else that makes this a better starting point without making it too complex.

## TODO:

 - Consider changing the MegaDungeon namespaces. (MegaDungeon is my own fork where I will develop my unique game.)

## Backstory
After stumbling on [Pathos](https://pathos.azurewebsites.net/) the Nethack Reboot I learned that the author, Callan Hodgskin, built it on top of a code-first API capable of building for Windows, iOS, Android and Linux. I checked out the API and found it to be excellent. It re-ignited my desire to create a roguelike. I also want to promote Invention as a platform, so I created this repository as a basic starting point for roguelike developers who want to program in C#.
