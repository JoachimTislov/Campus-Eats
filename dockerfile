# Use official .NET 8 runtime image
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

RUN apt-get update && apt-get install -y openssl && apt-get clean

# Use the .NET 8 SDK image for building the application
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy the project files and restore dependencies
COPY ["Mor_Qui_Sun_Tis_Lau/Mor_Qui_Sun_Tis_Lau.csproj", "Mor_Qui_Sun_Tis_Lau/"]
RUN dotnet restore "Mor_Qui_Sun_Tis_Lau/Mor_Qui_Sun_Tis_Lau.csproj"

# Copy the rest of the app's source code and build the application
COPY . .
WORKDIR "/src/Mor_Qui_Sun_Tis_Lau"
RUN dotnet build "Mor_Qui_Sun_Tis_Lau.csproj" -c Release -o /app/build

# Publish the app as a self-contained deployment
RUN dotnet publish "Mor_Qui_Sun_Tis_Lau.csproj" -c Release -o /app/publish

# Final image with the runtime environment
FROM base AS final
WORKDIR /app

# Copy published application from the build stage
COPY --from=build /app/publish .

COPY "Mor_Qui_Sun_Tis_Lau/Infrastructure/Data/ShopContext.db" /app/Infrastructure/Data/

# Set environment variables for ASP.NET Core
# ENV ASPNETCORE_URLS="https://+:7004;http://+:80"
ENV ASPNETCORE_URLS="http://+:5001"
ENV DOTNET_RUNNING_IN_CONTAINER=true

# Set the entrypoint for the Docker container
ENTRYPOINT ["dotnet", "Mor_Qui_Sun_Tis_Lau.dll"]
