FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app

# Create the user and dictionary directory
RUN adduser --disabled-password appuser \
    && mkdir -p /app/dictionary \
    && chown -R appuser:appuser /app/dictionary
# Switch to non-root user
USER appuser
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

# Copy and restore dependencies
COPY ["VocabularyRepositoryFromPDF.csproj", "."]
RUN dotnet restore "VocabularyRepositoryFromPDF.csproj"

# Copy the rest of the application code
COPY . .
WORKDIR "/src"
RUN dotnet build "VocabularyRepositoryFromPDF.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "VocabularyRepositoryFromPDF.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
# Copy published application to the final image
COPY --from=publish /app/publish .
# Ensure directory permissions are correct for runtime
USER appuser
ENTRYPOINT ["dotnet", "VocabularyRepositoryFromPDF.dll"]
