@echo off
xcopy ..\..\src\UnityTeaProject\Library\ScriptAssemblies\TeaProject.dll .\dll /y
Excel2Json\Excel2Json.exe .\dll\TeaProject.dll Excel\ Define\ txt
xcopy .\Define\ ..\..\src\UnityTeaProject\Assets\StreamingAssets\Define /s /y
pause