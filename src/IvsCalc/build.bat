@echo off

setlocal EnableDelayedExpansion

IF "%1"=="" (
    ECHO Usage: build.bat "<SolutionDirectory>" "<LogVerbocity>"
    EXIT /B 1
) ELSE (
    SET SOLUTION_DIR=%1
)

IF "%2"=="" (
    ECHO Usage: build.bat "<SolutionDirectory>" "<LogVerbocity>"
    EXIT /B 1
) ELSE (
    SET VERBOCITY=%2
)

IF "%MSBUILD%"=="" (
    ECHO Locating MSBuild...

    FOR /F "tokens=* USEBACKQ" %%F IN (`"%ProgramFiles(x86)%\Microsoft Visual Studio\Installer\vswhere.exe" -latest -prerelease -products * -requires Microsoft.Component.MSBuild -find MSBuild\**\Bin\MSBuild.exe`) do (
        SET MSBUILD=%%F
    )
)

IF NOT EXIST "%MSBUILD%" (
    ECHO MSBuild not found!
    EXIT /B 1
)

ECHO MSBuild: %MSBUILD%

IF EXIST "IvsCalc\IvsCalc\Libraries" (
    FOR /F %%i IN ('dir /b /a "IvsCalc\IvsCalc\Libraries\*"') DO (
        ECHO Library already built, skipping...
        GOTO END
    )
)

:VCXPROJ
CALL :BUILD "IvsCalc\IvsCalcMathLib\IvsCalcMathLib.vcxproj", "Win32", "Debug"
CALL :BUILD "IvsCalc\IvsCalcMathLib\IvsCalcMathLib.vcxproj", "Win32", "Release"
CALL :BUILD "IvsCalc\IvsCalcMathLib\IvsCalcMathLib.vcxproj", "x64", "Debug"
CALL :BUILD "IvsCalc\IvsCalcMathLib\IvsCalcMathLib.vcxproj", "x64", "Release"

:END
ECHO Build finished

EXIT /B 0

:BUILD
ECHO Building %~1 (ARCH: %~2; CONFIG: %~3)
"%MSBUILD%" "%~1" -p:Platform="%~2" -p:Configuration="%~3" -t:restore -t:Build -v:%VERBOCITY% -p:SolutionDir=%~dp0%SOLUTION_DIR%
ECHO Build done
EXIT /B 0