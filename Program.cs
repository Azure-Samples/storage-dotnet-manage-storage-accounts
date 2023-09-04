// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using Azure;
using Azure.Core;
using Azure.Identity;
using Azure.ResourceManager;
using Azure.ResourceManager.Resources;
using Azure.ResourceManager.Samples.Common;
using Azure.ResourceManager.Storage;
using Azure.ResourceManager.Storage.Models;

namespace ManageStorageAccount
{
    public class Program
    {
        /**
         * Azure Storage sample for managing storage accounts -
         *  - Create a storage account
         *  - Get | regenerate storage account access keys
         *  - Create another storage account
         *  - List storage accounts
         *  - Delete a storage account.
         */
        private static ResourceIdentifier? _resourceGroupId = null;
        public static async Task RunSample(ArmClient client)
        {
            try
            {
                // ============================================================
                
                // Get default subscription
                SubscriptionResource subscription = await client.GetDefaultSubscriptionAsync();
                var rgName = Utilities.CreateRandomName("StorageAccountRG");
                Utilities.Log($"creating a resource group with name : {rgName}...");
                var rgLro = await subscription.GetResourceGroups().CreateOrUpdateAsync(WaitUntil.Completed, rgName, new ResourceGroupData(AzureLocation.EastUS));
                var resourceGroup = rgLro.Value;
                _resourceGroupId = resourceGroup.Id;
                Utilities.Log("Created a resource group with name: " + resourceGroup.Data.Name);

                //Create a storage account
                Utilities.Log("Creating a storage account...");
                var storageCollection = resourceGroup.GetStorageAccounts();
                var accountName = Utilities.CreateRandomName("saname");
                var sku = new StorageSku(StorageSkuName.StandardGrs);
                var kind = StorageKind.StorageV2;
                var content = new StorageAccountCreateOrUpdateContent(sku, kind, AzureLocation.EastUS);
                var storageAccountLro = await storageCollection.CreateOrUpdateAsync(WaitUntil.Completed, accountName, content);
                var storageAccount = storageAccountLro.Value;
                Utilities.Log("Created a storage account with name : " + storageAccount.Data.Name);

                // ============================================================

                // Get | regenerate storage account access keys
                Utilities.Log("Getting storage account access keys...");
                var storageAccountKeys = storageAccount.GetKeys();
                Utilities.Log("Got storage account access keys");
                Utilities.Log("Regenerating first storage account access key...");
                var i = 0;
                foreach (var key in storageAccountKeys)
                {
                    if (i == 0)
                    {
                        var keycontent = new StorageAccountRegenerateKeyContent(key.KeyName);
                        var regeneratekey = storageAccount.RegenerateKey(keycontent);
                    }
                    i++;
                }
                Utilities.Log("Regenerated first storage account access key");

                // ============================================================

                // Create another storage account
                Utilities.Log("Creating a 2nd Storage Account...");
                var storageCollection2 = resourceGroup.GetStorageAccounts();
                var accountName2 = Utilities.CreateRandomName("saname2");
                var sku2 = new StorageSku(StorageSkuName.StandardGrs);
                var kind2 = StorageKind.StorageV2;
                var content2 = new StorageAccountCreateOrUpdateContent(sku2, kind2, AzureLocation.EastUS);
                var storageAccountLro2 = await storageCollection2.CreateOrUpdateAsync(WaitUntil.Completed, accountName2, content2);
                var storageAccount2 = storageAccountLro2.Value;
                Utilities.Log("Created a 2nd Storage Accountt with name : " + storageAccount2.Data.Name);

                // ============================================================

                // Update storage account by enabling encryption
                Utilities.Log($"Enabling encryption for the storage account2: {storageAccount2.Data.Name}...");
                var patch = new StorageAccountPatch()
                {
                    Encryption = new StorageAccountEncryption()
                    {
                       Services = new StorageAccountEncryptionServices()
                       {
                           Blob = new StorageEncryptionService()
                           {
                               IsEnabled = true,
                           }
                       }
                    }
                };
                _ = await storageAccount2.UpdateAsync(patch);
                Utilities.Log($"Enabled encryption for the storage account2...");
                await foreach (var encryptionStatus in storageCollection2.GetAllAsync())
                {
                    String status = encryptionStatus.Data.Encryption.Services.Blob.IsEnabled.Equals(true) ? "Enabled" : "Not enabled";
                    Utilities.Log($"Encryption status of the service : {status}");
                }

                // ============================================================

                // List storage accounts
                Utilities.Log("Listing storage accounts...");
                var accounts = resourceGroup.GetStorageAccounts();
                if (resourceGroup != null)
                {
                    await foreach (var account in accounts)
                    {
                        Utilities.Log($"Storage Account {account.Data.Name} created @ {account.Data.KeyCreationTime.Key1}");
                    }
                }

                // ============================================================

                // Delete a storage account
                Utilities.Log($"Deleting a storage account - {accounts.ElementAt(0).Data.Name} created @ {accounts.ElementAt(0).Data.KeyCreationTime.Key1}...");
                accounts.ElementAt(0).DeleteAsync(WaitUntil.Completed).Wait();
                Utilities.Log($"Deleted a storage account");
            }
            finally
            {
                try
                {
                    if (_resourceGroupId is not null)
                    {
                        Utilities.Log($"Deleting Resource Group: {_resourceGroupId}");
                        await client.GetResourceGroupResource(_resourceGroupId).DeleteAsync(WaitUntil.Completed);
                        Utilities.Log($"Deleted Resource Group: {_resourceGroupId}");
                    }
                }
                catch (NullReferenceException)
                {
                    Utilities.Log("Did not create any resources in Azure. No clean up is necessary");
                }
                catch (Exception g)
                {
                    Utilities.Log(g);
                }
            }
        }
        public static async Task Main(string[] args)
        {
            try
            {
                var clientId = Environment.GetEnvironmentVariable("CLIENT_ID");
                var clientSecret = Environment.GetEnvironmentVariable("CLIENT_SECRET");
                var tenantId = Environment.GetEnvironmentVariable("TENANT_ID");
                var subscription = Environment.GetEnvironmentVariable("SUBSCRIPTION_ID");
                ClientSecretCredential credential = new ClientSecretCredential(tenantId, clientId, clientSecret);
                ArmClient client = new ArmClient(credential, subscription);
                await RunSample(client);
            }
            catch (Exception e)
            {
                Utilities.Log(e);
            }
        }
    }
}