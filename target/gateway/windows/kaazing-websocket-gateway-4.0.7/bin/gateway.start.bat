@REM
@REM Copyright (c) 2007-2013, Kaazing Corporation. All rights reserved.
@REM

@echo off

if "%OS%" == "Windows_NT" SETLOCAL EnableDelayedExpansion
rem ---------------------------------------------------------------------------
rem Windows start script for Kaazing Gateway
rem ---------------------------------------------------------------------------

cd %~dp0

rem A temporary variable for the location of the gateway installation,
rem to allow determining the conf and lib subdirectories (assumed to 
rem be siblings to this script's 'bin' directory).
set GW_HOME=..

rem You can define various Java system properties by setting the value
rem of the GATEWAY_OPTS environment variable before calling this script.
rem The script itself should not be changed. For example, the setting
rem below sets the Java maximum memory to 512MB.
if "%GATEWAY_OPTS%" == "" (
    set GATEWAY_OPTS=-Xmx512m
)

rem Create the classpath.

for /F %%a in ('dir /b "%GW_HOME%\lib\*.jar"') do if [!CP!]==[] (set CP=%GW_HOME%\lib\%%a) else (set CP=!CP!;%GW_HOME%\lib\%%a)

rem Add a directory for management support
set JAVA_LIBRARY_PATH=%GW_HOME%\lib\sigar

java %GATEWAY_OPTS% -Djava.library.path="%JAVA_LIBRARY_PATH%" -XX:+HeapDumpOnOutOfMemoryError -cp "%CP%" com.kaazing.gateway.server.WindowsMain %*
