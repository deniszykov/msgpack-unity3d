SET OUTPUT_DIR=%1
IF "%OUTPUT_DIR%"=="" SET OUTPUT_DIR=%CD%/Temp
IF EXIST "%OUTPUT_DIR%/NUL" RD "%OUTPUT_DIR%" /s /q
MKDIR %OUTPUT_DIR%
nuget pack GameDevWare.Serialization.csproj -Prop Configuration=Release -Prop Platform=AnyCPU -IncludeReferencedProjects -Build -OutputDirectory %OUTPUT_DIR%
nuget push "%CD%\Temp\*.nupkg" d481a3ec-f2be-449b-bb02-8a6d27d07c9e -NonInteractive
PAUSE