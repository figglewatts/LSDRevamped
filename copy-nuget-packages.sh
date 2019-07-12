#!/usr/bin/env bash

packagesPath="LSDR/nuget-packages/"
packages=( "Newtonsoft.Json.12.0.1/lib/net45/Newtonsoft.Json.dll" 
    "protobuf-net.2.4.0/lib/net40/protobuf-net.dll" 
    "SharpZipLib.1.1.0/lib/net45/ICSharpCode.SharpZipLib.dll" )
outputPath="LSDR/Assets/UnityEditor/Dep/External/"

for package in "${packages[@]}"
do
   cp "$packagesPath/$package" "$outputPath"
done