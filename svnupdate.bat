set svn_home=C:/Program Files/TortoiseSVN/bin 
set UnityProject=%~dp0

"%svn_home%"\TortoiseProc.exe/command:update /path:%UnityProject% /notempfile /closeonend:2
"%svn_home%"\TortoiseProc.exe/command:update /path:%UnityProject%Assets/Protobuf /notempfile /closeonend:2

if "%1"=="" pause