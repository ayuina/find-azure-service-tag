# Find Azure Network Service Tag

This tool checks an IP address is Azure public ip address, and which service is using.

# Prerequisite

To build this tool, you need .NET sdk, you can download from [here](https://dotnet.microsoft.com/download/dotnet/).
The specific version is written in [global.json](./global.json) file.

To run this tool, you need active Azure subscription.
If you don't have any subscripton, you can use [free trial](https://azure.microsoft.com/free/). 

# How to use

## Step 1 : Download Service Tags definitions.

Move to project root directory, run with your subscription id. 
The parameter `subscriptionid` can be specified in `appsettings.json` file.
If you don't have any cached credential when required, login your own credential which can access the Azure subscription specified.

```bash
$ dotnet run --subscriptionid "your-subscription-guid-goes-here"
```

This tool use [DefaultAzureCredential](https://docs.microsoft.com/en-us/dotnet/api/azure.identity.defaultazurecredential?view=azure-dotnet) class for authorization to access your Azure subscription.
Check the detail of DefaultAzureCredential behavior in [this Document](https://docs.microsoft.com/en-us/dotnet/api/overview/azure/identity-readme?view=azure-dotnet).

If successfully authenticated and call [Service Tags REST api](https://docs.microsoft.com/ja-jp/rest/api/virtualnetwork/servicetags/list), 
the file named `servicetags.json` is written in current directory. 
This file contains all Azure ServiceTag definition, and used in followoing operation.

## Step 2 : Check an IP Address 

Run with `ip` parameter, which you want to check.
This operation doesn't need authorization , but `servicetags.json` file.

```bash
$ dotnet run --ip 20.38.116.38

20.38.116.38 is in 20.38.116.0/23 (AzureCloud)
20.38.116.38 is in 20.38.116.0/23 (AzureCloud.japaneast)
20.38.116.38 is in 20.38.96.0/19 (Storage)
20.38.116.38 is in 20.38.116.0/23 (Storage.JapanEast)
```

This ip address is Azure Storqage Account in JapanEast region.

# How to pubish for MacOS

Publish executable file.

```bash
$ dotnet publish --runtime osx-x64 --output ./publish/osx-x64 \
    -p:PublishSingleFile=true -p:IncludeNativeLibrariesForSelfExtract=true
```

Then run this tool like below and check the output file `servicetags.json` exists.

```bash
$ cd ./publish/osx-x64
$ ./find-azure-service-tag --subscriptionid "your-subscription-guid-goes-here"
```

Check the ip address you are interested in.

```bash
$ ./find-azure-service-tag --ip 40.79.192.5

40.79.192.5 is in 40.79.192.0/21 (AzureCloud)
40.79.192.5 is in 40.79.192.0/21 (AzureCloud.japaneast)
40.79.192.5 is in 40.79.192.0/27 (Sql)
40.79.192.5 is in 40.79.192.0/27 (Sql.JapanEast)
```

This ip address is Azure SQL Database in JapanEast region.

# How to publish for Windows

Publish executable file.

```powershell
> dotnet publish --runtime win-x64 --output ./publish/win-x64 /p:PublishSingleFile=true /p:IncludeNativeLibrariesForSelfExtract=true
```

Then run this tool like below and check the output file `servicetags.json` exists.

```powershell
$ cd .\publish\win-x64
$ .\find-azure-service-tag.exe --subscriptionid "your-subscription-guid-goes-here"
```

Check the ip address you are interested in.

```powershell
> .\find-azure-service-tag.exe --ip 13.66.146.12 

13.66.146.12 is in 13.66.128.0/17 (AzureCloud)
13.66.146.12 is in 13.66.128.0/17 (AzureCloud.westus2)
13.66.146.12 is in 13.66.146.0/24 (AzureContainerRegistry)
13.66.146.12 is in 13.66.146.0/24 (AzureContainerRegistry.WestUS2)
```

This ip address is Azure Container Registry in West US 2 region.

