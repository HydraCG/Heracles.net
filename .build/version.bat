@ECHO OFF
SET "tag=0.5"
SET release=0
SET version=0
SET hash=0
FOR /F "tokens=*" %%a IN ('git tag') DO SET tag=%%a
FOR /F "tokens=1" %%a IN ('git show-ref -s %tag%') DO SET hash=%%a
FOR /F "tokens=*" %%a IN ('"git log %tag%..HEAD --pretty=format:%%H"') DO SET /A version+=1
ECHO [assembly: System.Reflection.AssemblyVersion("%tag:~1%.%version%.%release%")] > "%CD%\.build\VersionAssemblyInfo.cs""
ECHO [assembly: System.Reflection.AssemblyFileVersion("%tag:~1%.%version%.%release%")] >> "%CD%\.build\VersionAssemblyInfo.cs""
ECHO [assembly: System.Reflection.AssemblyInformationalVersion("%tag:~1%.%version%.%release%-%hash%")] >> "%CD%\.build\VersionAssemblyInfo.cs""
ECHO [assembly: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2243:AttributeStringLiteralsShouldParseCorrectly", Justification = "Commit hash added to AssemblyInformationalVersion is on purpose.")] >> "%CD%\.build\VersionAssemblyInfo.cs""
