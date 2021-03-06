﻿using System.Collections.Generic;
using System.IO;
using Functional;

namespace TagCloudApp
{
    internal class WhitespaceTextReader : ITextReader
    {
        public Result<IEnumerable<string>> ReadWords(string path)
        {
            try
            {
                return File.ReadAllText(path)
                           .Split(null);
            }
            catch (IOException ex)
            {
                return Result.Fail<IEnumerable<string>>(ex.Message);
            }
        }

        public string Extension => ".txt";
    }
}
