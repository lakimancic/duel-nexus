namespace Backend.Utils.Rand;

using System.Security.Cryptography;

public static class RandomExtensions
{
    public static List<int> GenerateUniqueRandomIndices(int count)
    {
        var indices = Enumerable.Range(0, count).ToList();
        Shuffle(indices);
        return indices;
    }

    public static void Shuffle<T>(this IList<T> list)
    {
        int n = list.Count;
        while (n > 1)
        {
            n--;
            int k = RandomNumberGenerator.GetInt32(n + 1);
            (list[k], list[n]) = (list[n], list[k]);
        }
    }
}
