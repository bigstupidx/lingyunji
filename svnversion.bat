set svn_home=C:/Program Files/TortoiseSVN/bin
set UnityProject=%~dp0
set VersionFile=svnversion.txt

@echo off
(
"%svn_home%\SubWCRev.exe" %UnityProject%
)>%VersionFile% 2>&1<nul

"%svn_home%\SubWCRev.exe" %UnityProject%

if "%1"=="" pause