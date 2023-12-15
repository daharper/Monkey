using System.Text;

namespace Monkey.Utils;

public static class CollectionsExt
{
    /// <summary>
    /// Skips a specified number of elements from the end of an enumerable sequence.
    /// </summary>
    /// <typeparam name="T">The type of the elements in the sequence.</typeparam>
    /// <param name="source">The enumerable sequence to skip elements from.</param>
    /// <param name="count">The number of elements to skip from the end of the sequence.</param>
    /// <returns>
    /// An enumerable sequence that contains the elements from the source sequence without
    /// the specified number of elements from the end.
    /// </returns>
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

    /// <summary>
    /// Performs the specified action on each element of the source collection,
    /// passing the index and the element to the action.
    /// </summary>
    /// <typeparam name="T">The type of the elements in the source collection.</typeparam>
    /// <param name="source">The source collection to perform the action on.</param>
    /// <param name="action">
    /// The action to perform on each element. The action accepts two parameters:
    /// the index of the element and the element itself.
    /// </param>
    public static void ForEach<T>(this IEnumerable<T> source, Action<int, T> action)
    {
        var index = 0;

        foreach (var item in source)
            action(index++, item);
    }

    /// <summary>
    /// Executes the specified <paramref name="action"/> for each element in the <paramref name="source"/> sequence.
    /// </summary>
    /// <typeparam name="T">The type of elements in the sequence.</typeparam>
    /// <param name="source">The sequence of elements.</param>
    /// <param name="action">The action to be executed for each element.</param>
    public static void ForEach<T>(this IEnumerable<T> source, Action<T> action)
    {
        foreach (var item in source)
            action(item);
    }

    /// <summary
    public static (int index, string value) TakeFrom(this string source, int startIndex, Func<char, bool> predicate)
    {
        ArgumentOutOfRangeException.ThrowIfGreaterThan(startIndex, source.Length);

        var index = startIndex;
        var sb = new StringBuilder();
        var ch = source[index];

        while (index < source.Length && predicate(ch))
        {
            sb.Append(ch);

            ++index;

            if (index < source.Length)
            {
                ch = source[index];
            }
        }

        return (index, sb.ToString());
    }
}