@echo off
setlocal ENABLEEXTENSIONS
pushd %~dp0

FOR /F "usebackq skip=2 tokens=1-3" %%A IN (`reg.exe query "HKLM\SOFTWARE\Microsoft\MSBuild\ToolsVersions\4.0" /v MSBuildToolsPath 2^>nul`) DO (
    set ValueName=%%A
    set ValueType=%%B
    set ValueValue=%%C
)
if not defined ValueValue (
	echo Need framework 4.5! Ei!
	goto qwer
)

%ValueValue%\msbuild.exe /m SomeSecretProjectCore.sln /property:Configuration=Release

echo Run SomeSecretProject/bin/release/play_icfp2015.exe

popd

:qwer
pause