SET package=%1
SET tag=%2
SET version=%3
SET release=%4
ECHO Building up Nuget package for %package%
MD NugetBuild\%package%
MD NugetBuild\%package%\lib
MD NugetBuild\%package%\lib\net461
MD NugetBuild\%package%\lib\netstandard2.0
COPY %package%\bin\%config%\net461\%package%.dll NugetBuild\%package%\lib\net461
IF EXIST %package%\bin\%config%\net461\%package%.pdb (COPY %package%\bin\%config%\net461\%package%.PDB NugetBuild\%package%\lib\net461)
COPY %package%\bin\%package%.xml NugetBuild\%package%\lib\net461
COPY %package%\bin\%config%\netstandard2.0\%package%.dll NugetBuild\%package%\lib\netstandard2.0
IF EXIST %package%\bin\%config%\netstandard2.0\%package%.pdb (COPY %package%\bin\%config%\netstandard2.0\%package%.pdb NugetBuild\%package%\lib\netstandard2.0)
COPY %package%\bin\%package%.xml NugetBuild\%package%\lib\netstandard2.0
COPY ".nuget\%package%.nuspec" NugetBuild\%package%
COPY ".nuget\Hydra-CG.png" NugetBuild\%package%
REM COPY "readme.md" NugetBuild\%package%
".build\nuget" pack NugetBuild\%package%\%package%.nuspec -version %tag:~1%.%version%.%release% -outputdirectory NugetBuild