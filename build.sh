#! /bin/bash

## Simply run xbuild
echo "Build Projects"
echo "------------------"
xbuild /p:Configuration=Release ./ConSim.Lib/ConSim.Lib.csproj
xbuild /p:Configuration=Release ./ConSim.Shell/ConSim.Shell.csproj
xbuild /p:Configuration=Release ./ConSim.Windows.Module/ConSim.Windows.Module.csproj
xbuild /p:Configuration=Release ./ConSim.Bash.Module/ConSim.Bash.Module.csproj