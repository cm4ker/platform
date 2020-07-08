#rem remove from cache
del /q "C:\Users\cmake\.nuget\packages\aquila.net.sdk\*"
FOR /D %%p IN ("C:\Users\cmake\.nuget\packages\aquila.net.sdk\*.*") DO rmdir "%%p" /s /q

cd "..\..\src\Aquila.NET.Sdk"
dotnet build
cd "..\..\build\dummy"
dotnet build 
#rem -p:Test=SomeValue
