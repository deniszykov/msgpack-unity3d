SET OUTPUT_DIR=%1
IF "%OUTPUT_DIR%"=="" SET OUTPUT_DIR=%CD%
nuget pack GameDevWare.Serialization.csproj -Prop Configuration=Release -Prop Platform=AnyCPU -IncludeReferencedProjects -Build -OutputDirectory %OUTPUT_DIR%