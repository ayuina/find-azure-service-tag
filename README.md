# Find Azure Network Service Tag

This tool checks an IP address is Azure public ip address, and which service is using.

# How to use

When build this project, the `servicetags.json` is generated on output directory.
Then run exe, 1st argument is IP address which you want to check.

```powershell
❯  .\find-azure-service-tag.exe  20.38.116.38 
20.38.116.38 is in 20.38.116.0/23 (AzureCloud)
20.38.116.38 is in 20.38.116.0/23 (AzureCloud.japaneast)
20.38.116.38 is in 20.38.96.0/19 (Storage)
20.38.116.38 is in 20.38.116.0/23 (Storage.JapanEast)
```

Or, simply run on project root directory like below.

```powershell
❯ dotnet run 52.239.211.104
52.239.211.104 is in 52.239.210.0/23 (AzureCloud)        
52.239.211.104 is in 52.239.210.0/23 (AzureCloud.westus2)
52.239.211.104 is in 52.239.208.0/20 (Storage)
52.239.211.104 is in 52.239.210.0/23 (Storage.WestUS2)
```