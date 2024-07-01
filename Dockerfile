FROM node:22 AS verify-format
WORKDIR /src
COPY package.json yarn.lock ./
RUN yarn
COPY . .
RUN yarn verify-format

FROM koalaman/shellcheck-alpine:v0.10.0 as verify-sh
WORKDIR /src
COPY ./*.sh ./
RUN shellcheck -e SC1091,SC1090 ./*.sh

FROM mcr.microsoft.com/dotnet/sdk:7.0.410-bullseye-slim AS restore
WORKDIR /src
COPY ./*.sln ./
COPY */*.csproj ./
# Take into account using the same name for the folder and the .csproj and only one folder level
RUN for file in $(ls *.csproj); do mkdir -p ${file%.*}/ && mv $file ${file%.*}/; done
RUN dotnet restore

FROM restore AS build
COPY . .
RUN dotnet format --verify-no-changes
RUN dotnet build -c Release

FROM build AS test
ARG EncryptionSettings__InitVectorAsAsciiString
ENV EncryptionSettings__InitVectorAsAsciiString=$EncryptionSettings__InitVectorAsAsciiString
ARG EncryptionSettings__SaltValueAsAsciiString
ENV EncryptionSettings__SaltValueAsAsciiString=$EncryptionSettings__SaltValueAsAsciiString
ARG EncryptionSettings__Password
ENV EncryptionSettings__Password=$EncryptionSettings__Password
RUN dotnet test

FROM build AS publish
RUN dotnet publish "./Doppler.UsersApi/Doppler.UsersApi.csproj" -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:6.0.31-bullseye-slim AS final
WORKDIR /app
EXPOSE 80
COPY --from=publish /app/publish .
ARG version=unknown
RUN echo $version > /app/wwwroot/version.txt
ENTRYPOINT ["dotnet", "Doppler.UsersApi.dll"]
