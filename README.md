---
page_type: sample
languages:
- csharp
products:
- azure
- azure-storage
- dotnet
extensions:
- services: Storage
- platforms: dotnet
urlFragment: getting-started-on-managing-storage-accounts-in-c
description: "Azure Storage sample for managing storage accounts."
---

# Get started managing Azure storage accounts (C#)

Azure Storage sample for managing storage accounts.

- Create a storage account
- Get | regenerate storage account access keys
- Create another storage account
- List storage accounts
- Delete a storage account.

## Running this sample

To run this sample:

Set the environment variable `AZURE_AUTH_LOCATION` with the full path for an auth file. See [how to create an auth file](https://github.com/Azure/azure-libraries-for-net/blob/master/AUTH.md).

```bash
git clone https://github.com/Azure-Samples/storage-dotnet-manage-storage-accounts.git
cd storage-dotnet-manage-storage-accounts
dotnet build
bin\Debug\net452\ManageStorageAccount.exe
```

## More information

[Azure Management Libraries for C#](https://github.com/Azure/azure-sdk-for-net/tree/Fluent)
[Azure .Net Developer Center](https://azure.microsoft.com/en-us/develop/net/)
If you don't have a Microsoft Azure subscription you can get a FREE trial account [here](http://go.microsoft.com/fwlink/?LinkId=330212).

This project has adopted the [Microsoft Open Source Code of Conduct](https://opensource.microsoft.com/codeofconduct/). For more information see the [Code of Conduct FAQ](https://opensource.microsoft.com/codeofconduct/faq/) or contact [opencode@microsoft.com](mailto:opencode@microsoft.com) with any additional questions or comments.
