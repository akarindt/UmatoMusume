@echo off

SET "WORKDIR=%~1"
SET "EXEPATH=%~2"

timeout /t 1 >nul

del /q "%WORKDIR%\*_OLD.*"

for /d %%i in ("%WORKDIR%\*_OLD") do rd /s /q "%%i"

start "" "%EXEPATH%"