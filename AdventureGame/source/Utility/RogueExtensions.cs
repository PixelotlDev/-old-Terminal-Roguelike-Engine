namespace AdventureGame
{
    public static class RogueExtensions
    {
        // Enum
        // modified version of function from: https://stackoverflow.com/a/643438
        public static T Next<T>(this T src) where T : Enum
        {
            T[] arr = (T[])Enum.GetValues(src.GetType());
            int j = Array.IndexOf(arr, src) + 1;
            return (j == arr.Length) ? arr[0] : arr[j];
        }

        // modified version of function from: https://stackoverflow.com/a/643438
        public static T Prev<T>(this T src) where T : Enum
        {
            T[] arr = (T[])Enum.GetValues(src.GetType());
            int j = Array.IndexOf(arr, src) - 1;
            return (j == -1) ? arr[^1] : arr[j];
        }

        public static int Length<T>(this T src) where T : Enum
        {
            return Enum.GetValues(src.GetType()).Length;
        }

        // String
        public static string Truncate(this string src, int len, int dots = 3)
        {
            return (src.Length <= len) ? src : src[..(len - dots)].TrimEnd() + new string('.', dots);
        }
    }
}