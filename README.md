# LSD Revamped

A remake of the cult-classic game LSD: Dream Emulator.

## Prerequisites

- [Git LFS](https://git-lfs.github.com/)
- Unity 2021.3.35f1 installed via Unity Hub
- [nuget.exe](https://learn.microsoft.com/en-us/nuget/install-nuget-client-tools#nugetexe-cli)
  - If you are using Mac or Linux, there are instructions above for running nuget.exe with mono.

## Quick start

1. Ensure you've prepared the prerequisites as above.
2. Clone this repo:
   ```terminal
   $ git clone https://github.com/Figglewatts/LSDRevamped.git
   ```
3. Run `nuget.exe restore -PackagesDir LSDR/nuget-packages LSDR/packages.config` to download and install the NuGet packages.
4. Run the `move-packages` shell script in `LSDR/`. There is a Bash version and a PowerShell version.
5. Open `LSDR/` in Unity.
6. All done!
