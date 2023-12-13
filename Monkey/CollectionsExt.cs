using System.Text;

namespace Monkey;

public static class CollectionsExt
{
    public static IEnumerable<T> SkipLast<T>(this IEnumerable<T> source, int count)
    {
        var queue = new Queue<T>(count + 1);

        foreach (var x in source)
        {
            queue.Enqueue(x);

            if (queue.Count > count)
                yield return queue.Dequeue();
        }
    }
    
    public static void ForEach<T>(this IEnumerable<T> source, Action<int, T> action)
    {
        var index = 0;
        
        foreach (var item in source)
            action(index++, item);
    }
    
    public static (int position, string value) TakeFrom(this string source, int startIndex, Func<char,bool> predicate)
    {
        ArgumentOutOfRangeException.ThrowIfGreaterThan(startIndex, source.Length);

        var position = startIndex;
        var sb = new StringBuilder();
        var ch = source[position];

        while (position < source.Length && predicate(ch))
        {
            sb.Append(ch);
            ch = source[++position];
        }
        
        return (position, sb.ToString());
    }
}