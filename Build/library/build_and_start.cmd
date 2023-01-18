call build.cmd
start cmd /C "cd ..\..\src\Aquila.Runner\ && dotnet run"
timeout 10
aq deploy -pkg bin\Debug\net6.0\Package\library.aqpk -e localhost:5000 -i library
timeout 5
aq migrate -e localhost:5000 -i library
