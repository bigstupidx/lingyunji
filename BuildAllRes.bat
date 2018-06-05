set UnityExe="D:/Program Files/Unity/Editor/Unity.exe"
set UnityProject=%~dp0

%UnityExe% -nographics -batchmode -projectPath %UnityProject% -executeMethod PackTool.New.AutoExportAll.BuildAutoExportTest
echo Íê³É!
pause