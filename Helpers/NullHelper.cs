internal static class NullHelper
{

    public static void Patch(string[] args)
    {
        if (args is null)
        {
            throw new ArgumentNullException(nameof(args));
        }
    }
}