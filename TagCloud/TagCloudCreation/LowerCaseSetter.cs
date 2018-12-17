﻿using Functional;

namespace TagCloudCreation
{
    public class LowerCaseSetter : IWordPreparer
    {
        /// <inheritdoc cref="IWordPreparer" />
        public Result<Maybe<string>> PrepareWord(string word, TagCloudCreationOptions _)
        {
            var preparedWord = word.ToLowerInvariant();

            return word == string.Empty ? Maybe<string>.None : preparedWord;
        }
    }
}
