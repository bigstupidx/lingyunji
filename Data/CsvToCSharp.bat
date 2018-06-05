set root=%~dp0

"%root%/CsvToCSharp.exe" CsvLoad "%root%/Config" "Auto/Game" "Auto-GCode"
"%root%/CsvToCSharp.exe" WorldCsvLoad "%root%/Config" "Auto/World" "Auto-WCode" "%root%/../Assets/Scripts/Share/AutoCsv/World"

rd /s /q "%root%/../Assets/Scripts/Share/AutoCsv/Game/*.cs"
rd /s /q "%root%/../Assets/Scripts/Share/AutoCsv/World/*.cs"

xcopy "%root%/Config/Auto-GCode" "%root%/../Assets/Scripts/Share/AutoCsv/Game"  /s /e /y
xcopy "%root%/Config/Auto-WCode" "%root%/../Assets/Scripts/Share/AutoCsv/World" /s /e /y

rd /s /q "%root%/Config/Auto-GCode"
rd /s /q "%root%/Config/Auto-WCode"