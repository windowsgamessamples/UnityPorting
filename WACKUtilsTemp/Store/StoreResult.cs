
public class StoreResult
{
    public string ItemID { get; internal set; }

    public bool Success
    {
        get
        {
            return !Cancelled && Receipt != null;
        }
    }

    public bool Cancelled { get; private set; }
    public string Reason { get; private set; }
    public string Receipt { get; private set; }

    private StoreResult(string id)
    {
        Cancelled = false;
        Reason = null;
        Receipt = null;
        ItemID = id;
    }

    public static StoreResult CreateSuccess(string id, string receipt)
    {
        return new StoreResult(id) { Receipt = receipt };
    }

    public static StoreResult CreateFailed(string id, string reason)
    {
        return new StoreResult(id) { Reason = reason };
    }

    public static StoreResult CreateCancelled(string id)
    {
        return new StoreResult(id) { Cancelled = true };
    }
}
