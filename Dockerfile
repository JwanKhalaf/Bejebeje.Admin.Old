# set base image as the dotnet 6.0 SDK.
FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build-env

# set the working directory for any RUN, CMD, ENTRYPOINT, COPY and ADD
# instructions that follows the WORKDIR instruction.
WORKDIR /app

# our current working directory within the container is /app
# we now copy all the files (from local machine) to /app (in the container).
COPY . ./

# again, on the container (we are in /app folder)
# we now publish the project into a folder called 'out'.
RUN dotnet publish Bejebeje.Admin/Bejebeje.Admin.csproj -c Release -o out

# set base image as the dotnet 6.0 runtime.
FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS runtime

# telling the application what port to run on.
ENV ASPNETCORE_URLS=http://*:5000

# set the working directory for any RUN, CMD, ENTRYPOINT, COPY and ADD
# instructions that follows the WORKDIR instruction.
WORKDIR /app

# copy the contents of /app/out in the `build-env` and paste it in the
# `/app` directory of the new runtime container.
COPY --from=build-env /app/out .

# set the entry point into the application.
ENTRYPOINT ["dotnet", "Bejebeje.Admin.dll"]
