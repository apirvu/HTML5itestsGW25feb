@REM
@REM Copyright (c) 2007-2013, Kaazing Corporation. All rights reserved.
@REM

@echo off
if "%OS%" == "Windows_NT" SETLOCAL EnableDelayedExpansion
rem ---------------------------------------------------------------------------
rem Windows start script for Kaazing Gateway
rem ---------------------------------------------------------------------------

rem A temporary variable for the location of the gateway installation,
rem to allow determining the conf and lib subdirectories (assumed to 
rem be siblings to this script's 'bin' directory).
set GW_HOME=%~dp0..

if "%GATEWAY_DEMO_OPTS%" == "" (
    set GATEWAY_DEMO_OPTS=-Xmx512m "-DGATEWAY_LOG_DIRECTORY=%GW_HOME%\log" "-DLOG4J_CONFIG=%GW_HOME%\conf\log4j-config.xml"
)

for /F %%a in ('dir /b "%GW_HOME%\lib\*.jar"') do if [!CP!]==[] (set CP=%GW_HOME%\lib\%%a) else (set CP=!CP!;%GW_HOME%\lib\%%a)

java %GATEWAY_DEMO_OPTS% -XX:+HeapDumpOnOutOfMemoryError -cp "%CP%" com.kaazing.gateway.server.core.demo.data.udp.UdpDataSource %*
