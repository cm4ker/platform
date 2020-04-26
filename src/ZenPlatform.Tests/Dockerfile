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

COPY ZenPlatform.Tests/ZenPlatform.Tests.csproj ZenPlatform.Tests/
COPY ZenPlatform.EntityComponent/ZenPlatform.EntityComponent.csproj ZenPlatform.EntityComponent/
COPY ZenPlatform.UIGeneration2/ZenPlatform.UIBuilder.csproj ZenPlatform.UIGeneration2/
COPY ZenPlatform.Controls.Avalonia/ZenPlatform.Controls.Avalonia.csproj ZenPlatform.Controls.Avalonia/
COPY DataGrid/src/Avalonia.DataGrid/Avalonia.DataGrid.csproj DataGrid/src/Avalonia.DataGrid/
COPY ZenPlatform.Shared/ZenPlatform.Shared.csproj ZenPlatform.Shared/
COPY DataGrid/src/Avalonia.DataGrid.Themes.Default/Avalonia.DataGrid.Themes.Default.csproj DataGrid/src/Avalonia.DataGrid.Themes.Default/
COPY ZenPlatform.ConfigurationDataComponent/ZenPlatform.DataComponent.csproj ZenPlatform.ConfigurationDataComponent/
COPY ZenPlatform.Core/ZenPlatform.Core.csproj ZenPlatform.Core/
COPY ZenPlatform.Data/ZenPlatform.Data.csproj ZenPlatform.Data/
COPY ZenPlatform.QueryBuilder2/ZenPlatform.QueryBuilder.csproj ZenPlatform.QueryBuilder2/
COPY ZenPlatform.Configuration/ZenPlatform.Configuration.csproj ZenPlatform.Configuration/
COPY ZenPlatform.Initializer/ZenPlatform.Initializer.csproj ZenPlatform.Initializer/
COPY ZenPlatform.System/ZenPlatform.System.csproj ZenPlatform.System/
COPY ZenPlatform.Builder/ZenPlatform.Cli.csproj ZenPlatform.Builder/
COPY ZenPlatform.Pidl/ZenPlatform.Pidl.csproj ZenPlatform.Pidl/
COPY ZenPlatform.ConfigurationExample/ZenPlatform.ConfigurationExample.csproj ZenPlatform.ConfigurationExample/ 
COPY NuGet.Config ./

#RUN dotnet restore ZenPlatform.Tests/ZenPlatform.Tests.csproj
COPY . .

WORKDIR /src/ZenPlatform.ConfigurationExample
RUN dotnet restore
RUN ./build.sh
#RUN dotnet build ZenPlatform.Tests.csproj -c Release -o /app

#FROM build AS publish
#RUN dotnet publish ZenPlatform.Tests.csproj -c Release -o /app

WORKDIR /src/ZenPlatform.Tests
RUN dotnet test ZenPlatform.Tests.csproj

#FROM base AS final
#WORKDIR /app
#COPY --from=publish /app .
#ENTRYPOINT ["dotnet", "ZenPlatform.Tests.dll"]
