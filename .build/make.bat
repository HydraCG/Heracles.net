@ECHO OFF
SET nobuild=
SET config=Release
IF "%1"=="nobuild" (SET nobuild=yes) ELSE (SET config=%1)
IF "%2"=="nobuild" (SET nobuild=yes)
IF "%config%"=="" (SET config=Release)
@ECHO %nobuild%
IF "%nobuild%"=="" (
    ECHO Cleaning up...
    IF EXIST Heracles.net\bin RD /s /q Heracles.net\bin
    IF EXIST Heracles.net\obj RD /s /q Heracles.net\obj
    IF EXIST Heracles.net.Tests\bin RD /s /q Heracles.net.Tests\bin
    IF EXIST Heracles.net.Tests\obj RD /s /q Heracles.net.Tests\obj
)

IF EXIST NugetBuild RD /s /q NugetBuild
MD NugetBuild

ECHO Setting up a version...
@ECHO OFF
CALL ".build\version"

IF "%nobuild%"=="" (
    ECHO Building...
    @ECHO OFF
    msbuild Heracles.net.sln /t:Restore
    msbuild Heracles.net.sln /p:Configuration=%config%
)

CALL ".build\pack" Heracles.net %tag% %version% %release%

:COMPLETED
