﻿using System;
using System.Collections.Generic;

namespace nnmnist.Data
{
    internal static class DataUtil
    {
        // helper functions that operate on array-like data structures
        // all extension functions

        // partition the lists into sub-lists
        // if there are m sub-lists
        // the ith example is in the ith % m list ("strip")
        public static List<List<T>> PartitionIntoBatches<T>(this List<T> storage, int batchCount)
        {
            if (batchCount == 0)
                return new List<List<T>> {storage};
            var batches = new List<List<T>>();
            var count = storage.Count;
            var batchSize = count / batchCount;
            var remainder = count % batchCount;
            var start = 0;
            for (var batchId = 0; batchId < batchCount; batchId++)
            {
                var size = batchSize;
                if (batchId < remainder)
                    size++;
                batches.Add(storage.GetRange(start, size));
                start += size;
            }
            return batches;
        }

        // elementwise add
        public static void ListAdd(this float[] a, float[] b)
        {
            for (var i = 0; i < a.Length; i++)
                a[i] += b[i];
        }

        // elementwise divide
        public static void ListDivide(this float[] a, float[] b)
        {
            for (var i = 0; i < a.Length; i++)
                a[i] /= b[i];
        }

        // divide by a constant
        public static void ListDivide(this float[] a, float b)
        {
            for (var i = 0; i < a.Length; i++)
                a[i] /= b;
        }

        // argmax
        public static int ArgMax<T>(this T[] a) where T : IComparable<T>
        {
            var max = a[0];
            var maxi = 0;
            for (var i = 1; i < a.Length; i++)
            {
                if (a[i].CompareTo(max) > 0)
                {
                    max = a[i];
                    maxi = i;
                }
            }
            return maxi;
        }

        // partition the list into sublists
        // first mbsize elements in the first minibatch ("consecutive")
        public static IEnumerable<T[]> GetMiniBatches<T>(this IEnumerable<T> source, int mbsize)
        {
            var list = new List<T>();

            foreach (var ex in source)
            {
                list.Add(ex);
                if (list.Count == mbsize)
                {
                    yield return list.ToArray();
                    list.Clear();
                }
            }
            if (list.Count > 0)
                yield return list.ToArray();
        }
    }
}