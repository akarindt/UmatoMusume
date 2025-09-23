@echo off

SET "WORKDIR=%~1"
SET "EXEPATH=%~2"

timeout /t 1 >nul

start "" "%EXEPATH%"
