FROM microsoft/dotnet:2.1-sdk

# Install mono for Cake
ENV MONO_VERSION 5.4.1.6

RUN apt-key adv --keyserver hkp://keyserver.ubuntu.com:80 --recv-keys 3FA7E0328081BFF6A14DA29AA6A19B38D3D831EF

RUN echo "deb http://download.mono-project.com/repo/debian stretch/snapshots/$MONO_VERSION main" > /etc/apt/sources.list.d/mono-official.list \
  && apt-get update \
  && apt-get install -y mono-runtime \
  && rm -rf /var/lib/apt/lists/* /tmp/*

RUN apt-get update \
  && apt-get install -y binutils curl mono-devel ca-certificates-mono fsharp mono-vbnc nuget referenceassemblies-pcl \
  && rm -rf /var/lib/apt/lists/* /tmp/*


WORKDIR /src

COPY Aquila.Tests/Aquila.Tests.csproj Aquila.Tests/
COPY Aquila.EntityComponent/Aquila.EntityComponent.csproj Aquila.EntityComponent/
COPY Aquila.UIGeneration2/Aquila.UIBuilder.csproj Aquila.UIGeneration2/
COPY Aquila.Controls.Avalonia/Aquila.Controls.Avalonia.csproj Aquila.Controls.Avalonia/
COPY DataGrid/src/Avalonia.DataGrid/Avalonia.DataGrid.csproj DataGrid/src/Avalonia.DataGrid/
COPY Aquila.Shared/Aquila.Shared.csproj Aquila.Shared/
COPY DataGrid/src/Avalonia.DataGrid.Themes.Default/Avalonia.DataGrid.Themes.Default.csproj DataGrid/src/Avalonia.DataGrid.Themes.Default/
COPY Aquila.ConfigurationDataComponent/Aquila.DataComponent.csproj Aquila.ConfigurationDataComponent/
COPY Aquila.Core/Aquila.Core.csproj Aquila.Core/
COPY Aquila.Data/Aquila.Data.csproj Aquila.Data/
COPY Aquila.QueryBuilder2/Aquila.QueryBuilder.csproj Aquila.QueryBuilder2/
COPY Aquila.Configuration/Aquila.Configuration.csproj Aquila.Configuration/
COPY Aquila.Initializer/Aquila.Initializer.csproj Aquila.Initializer/
COPY Aquila.System/Aquila.System.csproj Aquila.System/
COPY Aquila.Builder/Aquila.Cli.csproj Aquila.Builder/
COPY Aquila.Pidl/Aquila.Pidl.csproj Aquila.Pidl/
COPY Aquila.ConfigurationExample/Aquila.ConfigurationExample.csproj Aquila.ConfigurationExample/ 
COPY NuGet.Config ./

#RUN dotnet restore Aquila.Tests/Aquila.Tests.csproj
COPY . .

WORKDIR /src/Aquila.ConfigurationExample
RUN dotnet restore
RUN ./build.sh
#RUN dotnet build Aquila.Tests.csproj -c Release -o /app

#FROM build AS publish
#RUN dotnet publish Aquila.Tests.csproj -c Release -o /app

WORKDIR /src/Aquila.Tests
RUN dotnet test Aquila.Tests.csproj

#FROM base AS final
#WORKDIR /app
#COPY --from=publish /app .
#ENTRYPOINT ["dotnet", "Aquila.Tests.dll"]
