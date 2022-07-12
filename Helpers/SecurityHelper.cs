namespace Rio.Common.Helpers;
public static class SecurityHelper
{
    [ThreadStatic]
    private static Random? _random;

    public static Random Random
    {
        get
        {
            if (_random == null)
            {
                _random = new();
            }
            return _random;
        }
    }
}
