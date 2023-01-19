dotnet build /flp:v=diag; /bl:binarylogfilename.binlog
aq deploy -pkg bin\Debug\net6.0\Package\library.aqpk -e localhost:5000 -i library
timeout 2
aq migrate -e localhost:5000 -i library