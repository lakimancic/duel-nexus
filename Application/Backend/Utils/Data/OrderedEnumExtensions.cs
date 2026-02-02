namespace Backend.Utils.Data;

public static class OrderedEnumExtensions
{
    public static T Next<T>(this T current) where T : Enum
    {
        T[] values = (T[])Enum.GetValues(typeof(T));
        int currentIndex = Array.IndexOf(values, current);
        return values[currentIndex + 1];
    }

    public static T Previous<T>(this T current) where T : Enum
    {
        T[] values = (T[])Enum.GetValues(typeof(T));
        int currentIndex = Array.IndexOf(values, current);
        return values[(currentIndex - 1 + values.Length) % values.Length];
    }

    public static bool HasNext<T>(this T current) where T : Enum
    {
        T[] values = (T[])Enum.GetValues(typeof(T));
        int currentIndex = Array.IndexOf(values, current);
        return currentIndex < values.Length - 1;
    }
}
