# LSD Revamped
A remake of the cult-classic game LSD: Dream Emulator.

## Prerequisites
- Git LFS (https://git-lfs.github.com/)
- Unity 2017.4.30f1 installed via Unity Hub
- C# IDE (I use JetBrains Rider, but Visual Studio will work too...)
- Windows (this may change soon... watch this space)
- InControl (Unity Asset Store, paid asset)
- Python 3.7

## Quick start
1. Ensure you've prepared the prerequisites as above.
2. Clone and initialise this repo:
   ```terminal
   $ git clone https://github.com/Figglewatts/   LSDRevamped.git
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
1. Install the `toriicli` utility:
   ```terminal
   $ pip install toriicli
   ```
2. Make sure `toriicli` can find your Unity installation by running:
   ```terminal
   $ toriicli find
   ```
   It should print out the path to the Unity 2017.4.30f1 executable.
3. Run `toriicli build` in the root of the repo to build the project.
   Make sure Unity isn't open. This will take a while, so wait for it to finish.
4. Once it's finished, the build will be in the 'builds' folder as a zip. You
   can extract it and play it.