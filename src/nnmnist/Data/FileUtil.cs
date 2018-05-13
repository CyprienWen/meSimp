﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace nnmnist.Data
{
    internal static class FileUtil
    {

        // read the MNIST data (the idx3 format)

        // read a dataset, the image and the label should be paired
        public static List<Example> ReadFromFile(string imgFile, string lbFile)
        {
            var imgBytes = File.ReadAllBytes(imgFile);
            var laBytes = File.ReadAllBytes(lbFile);

            var nImg = GetFromFourBytes(imgBytes, 4);
            var nLabel = GetFromFourBytes(laBytes, 4);
            if (nImg != nLabel)
                throw new Exception("data number mismatch");
            var nRow = GetFromFourBytes(imgBytes, 8);
            var nCol = GetFromFourBytes(imgBytes, 12);
            var set = new List<Example>();
            for (var i = 0; i < nImg; i++)
            {
                var values = new int[nRow * nCol];
                var label = GetFromByte(laBytes, 8 + i);
                for (var j = 0; j < nRow * nCol; j++)
                    values[j] = GetFromByte(imgBytes, 16 + nRow * nCol * i + j);
                set.Add(new Example(values, label));
            }
            return set;
        }

        // read four bytes from the file (for image file)
        private static int GetFromFourBytes(byte[] arr, int idx)
        {
            if (BitConverter.IsLittleEndian)
            {
                var tmp = new byte[4];
                Array.Copy(arr, idx, tmp, 0, 4);
                tmp = tmp.Reverse().ToArray();
                return BitConverter.ToInt32(tmp, 0);
            }
            return BitConverter.ToInt32(arr, idx);
        }

        // read one byte from the file (for label file)
        private static int GetFromByte(byte[] arr, int idx)
        {
            return arr[idx];
        }
    }
}