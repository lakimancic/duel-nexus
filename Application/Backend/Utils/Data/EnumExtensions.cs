using System.ComponentModel;
using System.Reflection;

namespace Backend.Utils.Data;

public static class EnumExtensions
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

    public static string? GetDescription(this Enum current)
    {
        var field = current.GetType().GetField(current.ToString());
        var attribute = field?.GetCustomAttribute<DescriptionAttribute>();
        return attribute?.Description;
    }

    public static List<EnumDto> GetNameValues<T>() where T : Enum
    {
        return [.. Enum.GetValues(typeof(T))
            .Cast<T>()
            .Select(e => new EnumDto
            {
                Name = e.ToString(),
                Value = Convert.ToInt32(e),
                Description = e.GetDescription()
            })];
    }
}
