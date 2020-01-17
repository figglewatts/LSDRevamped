# LSD Revamped
A remake of the cult-classic game LSD: Dream Emulator.

## Screenshots
![Ingame](/Screenshots/Game-1.png)
![Menu](/Screenshots/Menu-1.png)

## Prerequisites
- Git LFS (https://git-lfs.github.com/)
- Unity 2017.4.31f1 LTS
- C# IDE (I use JetBrains Rider, but Visual Studio will work too...)
- Windows (this may change soon... watch this space)
- InControl (Unity Asset Store, paid asset)
- Python 3.7
- Pipenv

## Quick start
1. Ensure you've prepared the prerequisites as above.
2. Clone and initialise this repo:

```terminal
$ git clone https://github.com/Figglewatts/LSDRevamped.git
$ git submodule init
$ git submodule update
```

3. Open the folder "LSDR" in Unity.
4. Click on `Assets > Open C# Project`.
5. Restore the NuGet packages. This can be done from within your IDE -- I use JetBrains Rider, it's under Tools > NuGet > NuGet Restore (on Visual Studio you can restore NuGet packages by right clicking on the 'LSDR' solution and clicking 'Restore NuGet packages').
6. A folder called 'packages' in the LSDR folder has been created. Inside are the required NuGet packages. Execute the script `copy-nuget-packages.ps1` or `copy-nuget-packages.sh` depending on your shell.
7. Back in Unity, go to the Asset Store and add 'InControl' to the project.
8. All done!

## Building the game
1. Get a pipenv shell in the root of the repo:
```terminal
$ pipenv shell
```
2. Ensure you modify `buildconf.yaml` to have the correct values for `unity_location` and `project_path`.
3. Run the build script
```terminal
$ python build.py
```
