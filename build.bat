@echo off
REM Usage: build.bat [runtime]
REM Example: build.bat win-x64

setlocal

set DOTNET_VER=net9.0-windows
set DELETE_TARGET=win-x32

set RUNTIME=%1
if "%RUNTIME%"=="" set RUNTIME=win-x64

if "%RUNTIME%"=="win-x86" set DELETE_TARGET=win-x64
if "%RUNTIME%"=="win-x64" set DELETE_TARGET=win-x86

dotnet publish -c Release -r %RUNTIME% --self-contained true -o "bin\Release\%DOTNET_VER%\%RUNTIME%\publish\%RUNTIME%"

echo.
echo ========================================
echo Cleaning up unnecessary files...
echo ========================================

rmdir /s /q "bin\Release\%DOTNET_VER%\%RUNTIME%\publish\%RUNTIME%\Extras\chrome-driver-%DELETE_TARGET%\"
rmdir /s /q "bin\Release\%DOTNET_VER%\%RUNTIME%\publish\%RUNTIME%\Extras\chrome-%DELETE_TARGET%\"

echo.
echo ========================================
echo Cleaning up success!
echo ========================================


echo.
echo ========================================
echo Zipping up files
echo ========================================

set PUBLISH_DIR=bin\Release\%DOTNET_VER%\%RUNTIME%\publish
set ZIP_PATH=bin\Release\%DOTNET_VER%\umato-musume-%RUNTIME%.zip
if exist "%PUBLISH_DIR%" (
    powershell -Command "Compress-Archive -Path '%PUBLISH_DIR%\*' -DestinationPath '%ZIP_PATH%' -Force"
)

echo.
echo ========================================
echo Zipping complete!
echo ========================================

echo.
echo ========================================
echo Self-contained build complete for runtime: %RUNTIME%
echo Output: bin\Release\%DOTNET_VER%\%RUNTIME%\publish
echo ========================================