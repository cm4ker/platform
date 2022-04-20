FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /app

RUN mkdir "/app/.nugs"

# copy sln and csproj files into the image
COPY *.sln .
COPY NuGet.Config .
COPY Directory.Build.props .
COPY Build ./Build/ 
COPY src/ ./src/
#COPY src/Aquila.CodeAnalysis/*.csproj ./src/Aquila.CodeAnalysis/
#COPY src/Aquila.Syntax.Test/*.csproj ./src/Aquila.Syntax.Test/
# restore package dependencies for the solution
RUN dotnet restore
RUN dotnet build

# create a new build target called testrunner
FROM build AS testrunner
# navigate to the unit test directory
WORKDIR /app
# when you run this build target it will run the unit tests
CMD ["dotnet", "test", "--logger:trx"]