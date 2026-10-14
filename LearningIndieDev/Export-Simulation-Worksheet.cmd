@echo off
powershell -NoProfile -STA -ExecutionPolicy Bypass -File "%~dp0tools\Export-CellSimWorksheet.ps1" %*
if errorlevel 1 pause
