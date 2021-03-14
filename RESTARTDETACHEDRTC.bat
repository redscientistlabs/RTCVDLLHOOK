@echo off

taskkill /F /IM xemu.exe > nul 2>&1
taskkill /F /IM WerFault.exe > nul 2>&1

start xemu
exit