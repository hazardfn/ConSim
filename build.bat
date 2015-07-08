@echo off
echo Locate MS Build
echo -----------------
set build.msbuild=
for /D %%D in (%SYSTEMROOT%\Microsoft.NET\Framework\v4.0.30319) do set msbuild.exe=%%D\MSBuild.exe
echo MSBuild: %msbuild.exe%
echo.
echo Sanity Checks
echo -----------------
if not defined msbuild.exe echo error: can't find MSBuild.exe & goto :eof
if not exist "%msbuild.exe%" echo error: %msbuild.exe%: not found & goto :eof
echo %msbuild.exe% exists!
echo.
echo Build
echo -----------------
set platform=
@echo on
"%msbuild.exe%" /p:Configuration=Release "%cd%\ConSim.Lib\ConSim.Lib.csproj"
"%msbuild.exe%" /p:Configuration=Release "%cd%\ConSim.Windows.Module\ConSim.Windows.Module.csproj"
"%msbuild.exe%" /p:Configuration=Release "%cd%\ConSim.Shell\ConSim.Shell.csproj"
"%msbuild.exe%" /p:Configuration=Release "%cd%\ConSim.Bash.Module\ConSim.Bash.Module.csproj"
