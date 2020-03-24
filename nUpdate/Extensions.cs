// Extensions.cs, 14.11.2019
// Copyright (C) Dominic Beger 24.03.2020

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace nUpdate
{
    public static class Extensions
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
    }
}