//------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//------------------------------------------------------------

namespace Microsoft.Azure.Commands.Resources.Models
{
   
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

    public class StringExtensions
    {
        /// <summary>
        /// Concatenates the strings.
        /// </summary>
        /// <param name="strings">The strings.</param>
        /// <param name="separator">The separator.</param>
        public static string ConcatStrings(IEnumerable<string> strings, string separator = "")
        {
            return string.Join(separator, strings);
        }
        public static string[] SplitRemoveEmpty(string source, params char[] separator)
        {
            return source.Split(separator, StringSplitOptions.RemoveEmptyEntries);
        }

        public static int Find(string[] tokens, string target)
        {
            int length = tokens.Length;
            int index = -1;
            for (int i = 0; i < length; i++)
            {
                if (target == tokens.GetValue(i).ToString())
                    index = i;
            }
            return index;
        }
        public static string[] GetTokens(string source)
        {
            return StringExtensions.SplitRemoveEmpty(source, '/');
        }

        public static int GetTokenSize(string source)
        {
            string[] tokens = StringExtensions.SplitRemoveEmpty(source, '/');
            return tokens.Length;
        }


        public static string GetValue(string source, int index)
        {
            return GetTokens(source).GetValue(index).ToString();
        }

    }
    
}