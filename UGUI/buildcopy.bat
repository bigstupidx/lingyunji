set dstpath=D:\Program Files (x86)\Unity\Editor\

set dstpath=%dstpath%Data/UnityExtensions/Unity/GUISystem
set copypath=%dstpath%_back
if not exist "%copypath%" (
md "%copypath%"
xcopy "%dstpath%" "%copypath%" /y /e)

set project=%~dp0
xcopy "%project%Output" "%dstpath%" /y /e

cd %~dp0
pdb2mdb "%dstpath%/UnityEngine.UI.dll"
pdb2mdb "%dstpath%/Editor/UnityEditor.UI.dll"
pdb2mdb "%dstpath%/Standalone/UnityEngine.UI.dll"