// <copyright file="MethodExtensionsClass.cs" company="Florian">
// Copyright (c) Florian. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>
//
// Based on
// https://stackoverflow.com/questions/2601477/dictionary-returning-a-default-value-if-the-key-does-not-exist
//
namespace VSCMakeWizards
{
    using System.Collections.Generic;

    /// <summary>
    /// Extension methods must be defined in a static class
    /// </summary>
    public static class MethodExtensionsClass
    {
        /// <summary>
        /// Gets the value or default.
        /// </summary>
        /// <typeparam name="TKey">The type of the key.</typeparam>
        /// <typeparam name="TValue">The type of the value.</typeparam>
        /// <param name="dictionary">The dictionary.</param>
        /// <param name="key">The key.</param>
        /// <param name="defaultValue">The default value.</param>
        /// <returns>The value</returns>
        public static TValue GetValueOrDefault<TKey, TValue>(
            this IDictionary<TKey, TValue> dictionary,
            TKey key,
            TValue defaultValue)
        {
            TValue value;
            return dictionary.TryGetValue(key, out value) ? value : defaultValue;
        }
    }
}
