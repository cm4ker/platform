#rem remove from cache
del /q "C:\Users\cmake\.nuget\packages\aquila.net.sdk\*"
FOR /D %%p IN ("C:\Users\cmake\.nuget\packages\aquila.net.sdk\*.*") DO rmdir "%%p" /s /q

cd "C:\GitHub\Aquila.CodeAnalysis\Aquila.NET.Sdk"
dotnet build
cd "C:\GitHub\Aquila.CodeAnalysis\build\dummy"
dotnet build -p:Test=SomeValue
