@echo off

SET CUR_DIR=%~dp0
set TOOL_PATH=%CUR_DIR%exportExcel\ExportExcel.dll
set DATA_PATH=%CUR_DIR%
set UNITY_DATA=%CUR_DIR%..\Assets\Resources\database
set COMMON_CONFIG=common.txt

REM get common config
if exist %COMMON_CONFIG% (
    for /f "delims=" %%x in (%COMMON_CONFIG%) do (set "%%x")
) else (
    echo !!! missing %COMMON_CONFIG%
    goto :eof
)

pushd %DATA_PATH%

call dotnet %TOOL_PATH% %INPUT_FILE%

popd

echo.
echo Moving files to %UNITY_DATA%
if not exist %UNITY_DATA% (
    mkdir %UNITY_DATA%
)
move *.bytes %UNITY_DATA%

:eof