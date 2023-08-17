$DLL_LOCATION = "Assets/UnityEditor/Dep/External"
New-Item -ItemType Directory -Force -Path $DLL_LOCATION

$SDK_DLL_LOCATION = "Packages/com.figglewatts.lsdr.sdk/Vendor"
New-Item -ItemType Directory -Force -Path $SDK_DLL_LOCATION

Write-Information "Copying packages to LSDR project..."
foreach ($packagePath in @( `
    'nuget-packages/Newtonsoft.Json.12.0.1/lib/net45/Newtonsoft.Json.dll', `
    'nuget-packages/NVorbis.0.9.0/lib/net45/NVorbis.dll', `
    'nuget-packages/protobuf-net.2.4.0/lib/net40/protobuf-net.dll', `
    'nuget-packages/SharpZipLib.1.1.0/lib/net45/ICSharpCode.SharpZipLib.dll' `
)) {
    Copy-Item -Force -Path $packagePath -Destination $DLL_LOCATION
}

Write-Information "Copying packages to LSDR SDK..."
foreach ($packagePath in @( `
    'nuget-packages/MoonSharp.2.0.0.0/lib/net40-client/MoonSharp.Interpreter.dll' `
)) {
    Copy-Item -Force -Path $packagePath -Destination $SDK_DLL_LOCATION
}

Write-Information "Packages moved!"