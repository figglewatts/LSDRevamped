#!/usr/bin/env bash
set -e

DLL_LOCATION="Assets/UnityEditor/Dep/External"
mkdir -p $DLL_LOCATION

for packagePath in \
    'nuget-packages/MoonSharp.2.0.0.0/lib/net40-client/MoonSharp.Interpreter.dll' \
    'nuget-packages/Newtonsoft.Json.12.0.1/lib/net45/Newtonsoft.Json.dll' \
    'nuget-packages/NVorbis.0.9.0/lib/net45/NVorbis.dll' \
    'nuget-packages/protobuf-net.2.4.0/lib/net40/protobuf-net.dll' \
    'nuget-packages/SharpZipLib.1.1.0/lib/net45/ICSharpCode.SharpZipLib.dll'
do
    cp $packagePath $DLL_LOCATION
done

echo 'Packages moved!'