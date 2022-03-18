#rem remove from cache
del /q "C:\Users\cmaker\.nuget\packages\aquila.net.sdk\*"
FOR /D %%p IN ("C:\Users\cmaker\.nuget\packages\aquila.net.sdk\*.*") DO rmdir "%%p" /s /q

del /q "C:\Users\cmaker\.nuget\packages\aquila.runtime\*"
FOR /D %%p IN ("C:\Users\cmaker\.nuget\packages\aquila.runtime\*.*") DO rmdir "%%p" /s /q

del /q "C:\Users\cmaker\.nuget\packages\aquila.library\*"
FOR /D %%p IN ("C:\Users\cmaker\.nuget\packages\aquila.library\*.*") DO rmdir "%%p" /s /q


cd "..\..\src\Aquila.NET.Sdk"
dotnet build

cd "..\Aquila.Runtime"
dotnet build

cd "..\Aquila.Library"
dotnet build

cd "..\..\build\library"
dotnet build /flp:v=diag; /bl:binarylogfilename.binlog
#rem -p:Test=SomeValue
