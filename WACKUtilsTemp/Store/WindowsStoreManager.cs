using System;
using System.Collections.Generic;
using System.Linq;

public static class WindowsStoreManager
{
    public static Action<Action<List<StoreProduct>>> DoRetrieveProductList;
    public static Action<string, Action<StoreResult>> DoPurchaseProduct;
    public static Func<bool> DoGetIsAppOwned;
    public static Action<Action<List<StoreResult>>> DoRestorePurchases;

    public static List<StoreProduct> ProductList { get; private set; }

    static WindowsStoreManager()
    {
        ProductList = new List<StoreProduct>();
    }

    public static void RetrieveProductList(Action<List<StoreProduct>> callback)
    {
        if (DoRetrieveProductList != null)
        {
            DoRetrieveProductList(
                list =>
                {
                    ProductList = list;
                    if (callback != null)
                        callback(list);
                });
        }
    }

    public static void PurchaseProduct(StoreProduct product, Action<StoreResult> callback)
    {
        PurchaseProduct(product.ID, callback);
    }

    public static void PurchaseProduct(string id, Action<StoreResult> callback)
    {
        if (DoPurchaseProduct != null)
            DoPurchaseProduct(id, callback);
    }

    public static void PurchaseProductConsumable(string baseId, Action<StoreResult> callback, string idFormatString = "{0}-{1}")
    {
        RetrieveProductList(
            products =>
            {
                bool done = false;
                int count = 1;
                while (!done)
                {
                    var id = string.Format(idFormatString, baseId, count);
                    var product = products.SingleOrDefault(p => p.ID == id);
                    if (product != null)
                    {
                        if (!product.Owned)
                        {
                            PurchaseProduct(id,
                                result =>
                                {
                                    if (callback != null)
                                    {
                                        result.ItemID = baseId; // Overwrite to hide the store consumable "hack"
                                        callback(result);
                                    }
                                });
                            return;
                        }
                        count++;
                    }
                    else
                        done = true; // Product doesn't exist
                }

                if (callback != null)
                    callback(StoreResult.CreateFailed(baseId, "Product does not exist"));
            });
    }

    public static bool HasApplicationBeenPurchased()
    {
        if (DoGetIsAppOwned != null)
            return DoGetIsAppOwned();
        else
            throw new NotImplementedException();
    }

    public static void RestorePurchases(Action<List<StoreResult>> onCompleted)
    {
        if (DoRestorePurchases != null)
            DoRestorePurchases(onCompleted);
    }
}
