#if __UAP__

using System;
using System.ComponentModel;
using System.Threading.Tasks;
using Windows.Services.Store;

namespace Wokhan.Shared.Store
{
    public partial class LicenceManager
    {
        private static LicenceManager Instance;

        private StoreContext storeContext;
        private StoreAppLicense licenceInformation;

        private static async Task _initInstance()
        {
            Instance = new LicenceManager
            {
                storeContext = StoreContext.GetDefault()
            };

            Instance.licenceInformation = await Instance.storeContext.GetAppLicenseAsync();
            /*PropertyChanged?.Invoke(Instance, new PropertyChangedEventArgs(nameof(RemainingDays)));
            PropertyChanged?.Invoke(Instance, new PropertyChangedEventArgs(nameof(IsExpired)));
            PropertyChanged?.Invoke(Instance, new PropertyChangedEventArgs(nameof(IsTrial)));*/
        }

        private static bool _isValid() => Instance.licenceInformation.IsActive;


        private static bool _isTrial() =>
#if TRIAL
            true;
#else
            Instance.licenceInformation.IsActive && Instance.licenceInformation.IsTrial;
#endif

        private static int _remainingDays() => (int)Math.Floor(Instance.licenceInformation.ExpirationDate.Subtract(DateTime.Now).TotalDays);

        private static async Task _goToPurchase(Action<bool> callback, string id = null, bool isFree = false)
        {
            StoreProduct prd;
            if (id == null)
            {
                StoreProductResult storeProduct = await Instance.storeContext.GetStoreProductForCurrentAppAsync();
                prd = storeProduct.Product;
            }
            else
            {
                StoreProductQueryResult storeProductQuery = await Instance.storeContext.GetStoreProductsAsync(new[] { "Application" }, new[] { id });
                prd = storeProductQuery.Products[id];
            }
            if (isFree)
            {
                StorePackageUpdateResult packageUpdateResult = await Instance.storeContext.RequestDownloadAndInstallStorePackagesAsync(new[] { id });
                callback(packageUpdateResult.OverallState == StorePackageUpdateState.Downloading);
            }
            else
            {
                StorePurchaseResult purchaseResult = await prd.RequestPurchaseAsync();
                callback(purchaseResult.Status == StorePurchaseStatus.Succeeded || purchaseResult.Status == StorePurchaseStatus.AlreadyPurchased);
            }
        }

        private static async Task _goToAddonPurchase(string addOnId, Action<bool> callback)
        {
            StorePurchaseResult purchaseResult = null;
            StoreProductQueryResult storeProduct = await Instance.storeContext.GetStoreProductsAsync(new[] { "Consumable" }, new[] { addOnId });
            if (storeProduct != null && storeProduct.Products.ContainsKey(addOnId))
            {
                purchaseResult = await storeProduct.Products[addOnId].RequestPurchaseAsync();
            }
            callback(purchaseResult.Status == StorePurchaseStatus.Succeeded || purchaseResult.Status == StorePurchaseStatus.AlreadyPurchased);
        }
    }
}

#endif