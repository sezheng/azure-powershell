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
        public static string GetLast(string source)
        {
            string[] tokens = StringExtensions.GetTokens(source);
            int length = tokens.Length;
            return tokens[length - 1];
        }
        public static string[] SplitRemoveEmpty(string source, params char[] separator)
        {
            return source.Split(separator, StringSplitOptions.RemoveEmptyEntries);
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