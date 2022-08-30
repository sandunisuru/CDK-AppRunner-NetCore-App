#Get Base SDK for Dotnet Core 6.0
FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /app
EXPOSE 80

#Download Nuget Search Service 
RUN curl https://api.nuget.org/v3/index.json -k

#Copy solution file
COPY *.sln ./

#Copy CSPROJ file and restore dependecies
COPY */*.csproj ./
RUN for file in $(ls *.csproj); do mkdir -p ${file%.*} && mv $file ${file%.*}; done
RUN dotnet restore

#Copy the project files and publish
COPY . ./
FROM build AS publish
RUN dotnet publish -c Release -o out

# Generate Runtime image
FROM mcr.microsoft.com/dotnet/aspnet:6.0
WORKDIR /app
EXPOSE 80
COPY --from=publish /app/out .
ENTRYPOINT [ "dotnet", "AppRunnerTestAPI.dll" ]