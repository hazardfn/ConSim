#! /bin/bash

## Build
bash build.sh
clear

## Test
command -v nuget > /dev/null 2>&1 || { echo >&2 "I require nuget but it's not installed.  Aborting."; exit 1; }

if [[ -z ${IS_TRAVIS} ]]; then
nuget restore ConSim.sln
nuget install NUnit.Runners -Version 2.6.4 -OutputDirectory testrunner
fi

xbuild /p:Configuration=Release ./ConSim.NUnit/ConSim.NUnit.csproj
clear
mono ./testrunner/NUnit.Runners.2.6.4/tools/nunit-console.exe ./ConSim.NUnit/bin/Release/ConSim.NUnit.dll