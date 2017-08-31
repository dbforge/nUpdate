// Copyright © Dominic Beger 2017

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using nUpdate.Win32;

namespace nUpdate
{
    internal static class Extensions
    {
        public static Task ForEachAsync<T>(this IEnumerable<T> source, Func<T, Task> body, int degreeOfParallelism = 4)
        {
            return Task.WhenAll(
                from partition in Partitioner.Create(source).GetPartitions(degreeOfParallelism)
                select Task.Run(async () =>
                {
                    using (partition)
                    {
                        while (partition.MoveNext())
                            await body(partition.Current);
                    }
                }));
        }

        public static string ToAdequateSizeString(this long fileSize)
        {
            var sb = new StringBuilder(20);
            NativeMethods.StrFormatByteSize(fileSize, sb, 20);
            return sb.ToString();
        }
    }
}