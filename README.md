# LSD Revamped
A remake of the cult-classic game LSD: Dream Emulator.

## Prerequisites
- Git LFS (https://git-lfs.github.com/)
- Unity 2017.4.17f1 LTS
- Visual Studio
- Windows (this may change soon... watch this space)

## Quick start
1. Ensure you've prepared the prerequisites as above.
2. Clone this repo.
3. Open the folder "LSDR" in Unity.
4. Click on `Assets > Open C# Project`.
5. Right click on the solution 'LSDR' in Visual Studio and click on 'Restore NuGet Packages'.
6. A folder called 'packages' in the LSDR folder has been created. Inside are the required NuGet packages. Copy `packages/Newtonsoft.Json.X/lib/net45/Newtonsoft.Json.dll` and `packages/protobuf-net.X/lib/net40/protobuf-net.dll` to `LSDR/Assets/UnityEditor/Dep/External/`.
