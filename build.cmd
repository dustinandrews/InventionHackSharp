@echo off
if "" equ "%MSBuildPath%" (goto error)
"%MSBuildPath%" -p:Configuration=Debug;Platform=x64 -t:Clean,Build InventionBase.sln

:error
echo Must set MSBuildPath to MSBuild.exe to use.