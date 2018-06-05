call "%~dp0/svnupdate.bat" 1
echo 更新完成!

set UnityExe="C:/Program Files/Unity/Editor/Unity.exe"
set UnityProject=%~dp0

%UnityExe% -nographics -batchmode -projectPath %UnityProject% -args %1 -quit -executeMethod PackTool.ResetAllResources.BuildByCommonline
echo 完成!
