# Tempore

A comprehensive time and attendance system that provides accurate tracking of employee hours and attendance. 

## Supported Devices

| Devices                             |   Brand   |     Model     |
|-------------------------------------|:---------:|:-------------:|
| Fingerprint Access Control Terminal | Hikvision | DSK-K1T804AEF |
| Value Series Face Access Terminal   | Hikvision | DS-K1T341AMF  |

## Third Party Software Integration

| Software |            Import             |        Export         |
|----------|:-----------------------------:|:---------------------:|
| Payday   | Employee Data in Excel format | Employee hours in CSV |


## Developmenmt

### Build Status

| Branch  |                                                                                                           Status                                                                                                          |
| ------- |:-------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------:|
| master  |                                                                                                           [N/A]                                                                                                           |
| develop | [![Build Status](https://dev.azure.com/Port-Hope-Investment/Tempore/_apis/build/status%2FTempore?branchName=develop)](https://dev.azure.com/Port-Hope-Investment/Tempore/_build/latest?definitionId=3&branchName=develop) |


### Deployment Status

| Deployment               |                                                                   Status                                                                  |                                  Link                                 |
| ------------------------ |:-----------------------------------------------------------------------------------------------------------------------------------------:|:---------------------------------------------------------------------:|
| Tempore Agent            | ![Deployment Status](https://vsrm.dev.azure.com/Port-Hope-Investment/_apis/public/Release/badge/45ea6a94-54c3-49c5-bbaa-f561a0dce01b/2/2) | [Open](https://nexus.tempore.io/#browse/browse:tempore:Tempore.Agent) |
| Herramientas y Tornillos | ![Deployment Status](https://vsrm.dev.azure.com/Port-Hope-Investment/_apis/public/Release/badge/45ea6a94-54c3-49c5-bbaa-f561a0dce01b/2/3) |                      [Open](http://ht.tempore.io)                     |
| Port Hope Investment     | ![Deployment Status](https://vsrm.dev.azure.com/Port-Hope-Investment/_apis/public/Release/badge/45ea6a94-54c3-49c5-bbaa-f561a0dce01b/2/4) |                      [Open](http://phi.tempore.io)                    |

## Development

### Configuration

| Environment Variable                      |                Description                 |
|-------------------------------------------|:------------------------------------------:|
| TMP_CONNECTIONSTRINGS_APPLICATIONDATABASE | The application database connection string |

### Migrations

To start with generating the database migrations, folow the next steps:

1) Open a terminal, move to deployment\tye directory and run the following command

        > tye run .\backend-%DB_VENDOR%.yaml

  > **NOTE**: This runs a database engine from the specific database vendor.

2) Open the Package Manager console on Visual Studio and type according with your requirements:

        > Add-Migration %MIGRATION_NAME% -StartupProject src\storage\Tempore.Storage.%DB_VENDOR% -Project src\storage\Tempore.Storage.%DB_VENDOR% -Context %DB_VENDOR%ApplicationDbContext -Args '--environment tye'

        > Remove-Migration -StartupProject src\storage\Tempore.Storage.%DB_VENDOR% -Project src\storage\Tempore.Storage.%DB_VENDOR% -Context %DB_VENDOR%ApplicationDbContext -Args '--environment tye'

  > **NOTE**: Replace the placeholders with real names. For instance %MIGRATION_NAME% with Update_Something or Remove_SomethingElse, and %DB_VENDOR% with PostgreSQL.

### Force Coding Style

1) Temporally modify the file 'src\global.json' with this content:

        {
            "msbuild-sdks": {
                "MSBuild.Sdk.Extras": "3.0.44"
            },
            "sdk": {
                "version": "7.0.0",
                "rollForward": "latestMinor",
                "allowPrerelease": false
            }
        }

     > **NOTE**: It you already installed the tool skip this step.     

2) Install or update the dotnet-format using the following install or update commands:

        > dotnet tool install -g dotnet-format --version "7.*" --add-source https://pkgs.dev.azure.com/dnceng/public/_packaging/dotnet7/nuget/v3/index.json

    or

        > dotnet tool update -g dotnet-format --version "7.*" --add-source https://pkgs.dev.azure.com/dnceng/public/_packaging/dotnet7/nuget/v3/index.json

3) Revert the file 'src\global.json'. 

        {
            "msbuild-sdks": {
                "MSBuild.Sdk.Extras": "3.0.44"
            },
            "sdk": {
                "version": "6.0.0",
                "rollForward": "latestMinor",
                "allowPrerelease": false
            }
        }

     > **NOTE**: It is recommended discard the changes of this file using the version control system.     

4) Before opening a command prompt and run the following commands:

        > cd %CLONE_DIRECTORY%\src
        > dotnet-format