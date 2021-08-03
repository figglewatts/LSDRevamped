# LSD Revamped
A remake of the cult-classic game LSD: Dream Emulator.

## Prerequisites
* [Git LFS](https://git-lfs.github.com/)
* Unity 2019.4.20f1 installed via Unity Hub
* C# IDE (I use JetBrains Rider, but Visual Studio will work too...)
* Windows (this may change soon... watch this space)
* [InControl](https://assetstore.unity.com/packages/tools/input-management/incontrol-14695) (Unity Asset Store, paid asset)
* Python 3.7
* [Toriicli](https://github.com/Figglewatts/toriicli)

## Quick start
1. Ensure you've prepared the prerequisites as above.
2. Clone this repo:
   ```terminal
   $ git clone https://github.com/Figglewatts/LSDRevamped.git
   ```
3. Run `toriicli nuget restore` to download and install the NuGet packages.
4. Open the folder "LSDR" in Unity.
5. Go to the Asset Store and add 'InControl' to the project.
6. All done!

## Building the game
1. Make sure `toriicli` can find your Unity installation by running:
   ```terminal
   $ toriicli find
   ```
   It should print out the path to the Unity 2019.4.20f1 executable.
2. Run `toriicli build` in the root of the repo to build the project.
   Make sure Unity isn't open. This will take a while, so wait for it to finish.
3. Once it's finished, the build will be in the 'builds' folder as a zip. You
   can extract it and play it.
