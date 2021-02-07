# Get Base Image (Full .NET Core SDK)
FROM mcr.microsoft.com/dotnet/sdk:5.0-alpine AS build-env
WORKDIR /app

# Copy csproj and restore
COPY *.csproj ./
RUN dotnet restore

# Copy everything else and build
COPY . ./
RUN dotnet publish -c Release -o out

# Generate runtime image
FROM mcr.microsoft.com/dotnet/aspnet:5.0-alpine
WORKDIR /app
EXPOSE 70
COPY --from=build-env /app/out .
ENTRYPOINT ["dotnet", "CommandsAPI.dll"]

# to run the build 
# "docker build -t commandslist/commandsapi ."  

# run the built code

# docker run -p 8080:80 commandslist/commandsapi

#postgress docker command
# docker run --name some-postgres -e POSTGRES_PASSWORD=pa55w0rrd -p 5432:5432 -d postgres