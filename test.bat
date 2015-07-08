REM BUILD FIRST
call build.bat

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
echo NuGet
echo -----------------

WHERE nuget.exe 
IF %ERRORLEVEL% NEQ 0 ECHO nuget.exe wasn't found, please download and place in this directory or in PATH & goto :eof

nuget restore ConSim.sln
nuget install NUnit.Runners -Version 2.6.4 -OutputDirectory testrunner

echo.

echo Build
echo -----------------
@echo on
"%msbuild.exe%" /p:Configuration=Release "%cd%\ConSim.NUnit\ConSim.NUnit.csproj"
@echo off
clear
@echo on
"%cd%\testrunner\NUnit.Runners.2.6.4\tools\nunit-console.exe" "%cd%\ConSim.NUnit\bin\Release\ConSim.NUnit.dll"
