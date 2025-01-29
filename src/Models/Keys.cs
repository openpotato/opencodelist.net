#region OpenCodeList.NET - Copyright (c) STÜBER SYSTEMS GmbH
/*    
 *    OpenCodeList.NET 
 *    
 *    Copyright (c) STÜBER SYSTEMS GmbH
 *
 *    Licensed under the MIT License, Version 2.0. 
 * 
 */
#endregion

using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text.Json;

namespace OpenCodeList
{
    /// <summary>
    /// Keys of a code list.
    /// </summary>
    public class Keys : IEnumerable<Key>
    {
        private readonly CodeListDocument _document;
        private readonly List<Key> _keys = [];

        /// <summary>
        /// Initializes a new instance of the <see cref="Keys"/> class.
        /// </summary>
        /// <param name="document">The owner of the data set</param>
        internal Keys(CodeListDocument document)
        {
            _document = document;
        }

        /// <summary>
        /// Number of keys 
        /// </summary>
        public int Count => _keys.Count;

        /// <summary>
        /// List of keys by ID
        /// </summary>
        public Key this[string keyId]
        {
            get
            {
                var i = IndexOf(x => x.Id == keyId);
                if (i != -1)
                {
                    return _keys[i];
                }
                else
                {
                    throw new Exception($"Key with ID \"{keyId}\" not found");
                }
            }
            set
            {
                var i = IndexOf(x => x.Id == keyId);
                if (i != -1)
                {
                    _keys[i] = value;
                }
                else
                {
                    throw new Exception($"Key with ID \"{keyId}\" not found");
                }
            }
        }

        /// <summary>
        /// List of keys by index
        /// </summary>
        public Key this[int index]
        {
            get
            {
                return _keys[index];
            }
            set
            {
                _keys[index] = value;
            }
        }

        /// <summary>
        /// Creates a new and empty key and adds it to the internal key collection
        /// </summary>
        /// <returns>the new <see cref="Key"/> instance</returns>
        public Key Add()
        {
            var row = new Key(_document);
            _keys.Add(row);
            return row;
        }

        /// <summary>
        /// Removes all keys from the internal key collection
        /// </summary>
        public void Clear()
        {
            _keys.Clear();
        }

        /// <summary>
        /// Does a certain key exist?
        /// </summary>
        /// <param name="predicate">A function to test each element for a condition</param>
        /// <returns>TRue, if found</returns>
        public bool Contains(Func<Key, bool> predicate)
        {
            return _keys.Any(predicate);
        }

        /// <summary>
        /// Find a certain key
        /// </summary>
        /// <param name="predicate">A function to test each element for a condition</param>
        /// <returns>A <see cref="Key"/> instance or null</returns>
        public Key FindOrDefault(Func<Key, bool> predicate)
        {
            return _keys.FirstOrDefault(predicate);
        }

        /// <summary>
        /// Support for iteration over a <see cref="Key"/> collection
        /// </summary>
        /// <returns>Enumerator</returns>
        IEnumerator<Key> IEnumerable<Key>.GetEnumerator()
        {
            return _keys.GetEnumerator();
        }

        /// <summary>
        /// Support for iteration over a non-generic collection.
        /// </summary>
        /// <returns>Enumerator</returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return _keys.GetEnumerator();
        }

        /// <summary>
        /// Index of a key
        /// </summary>
        /// <param name="predicate">A function to test each element for a condition</param>
        /// <returns>Index if found, otherwise -1</returns>
        public int IndexOf(Func<Key, bool> predicate)
        {
            for (int i = 0; i < _keys.Count; i++)
            {
                if (predicate(_keys[i]))
                {
                    return i;
                }
            }
            return -1;
        }

        /// <summary>
        /// Removes a key
        /// </summary>
        /// <param name="key">The key</param>
        /// <returns>TRUE, if successfull</returns>
        public bool Remove(Key key)
        {
            return _keys.Remove(key);
        }

        /// <summary>
        /// Removes all keys with reference to a given column
        /// </summary>
        /// <param name="column">The column</param>
        /// <returns>Number of removed keys</returns>
        public int RemoveAll(Column column)
        {
            return _keys.RemoveAll(x => x.Columns.Contains(column));
        }

        /// <summary>
        /// Tries to find a certain key
        /// </summary>
        /// <param name="predicate">A function to test each element for a condition</param>
        /// <param name="key">The <see cref="Key"/> instancce if found</param>
        /// <returns>True, if successfull</returns>
        public bool TryFind(Func<Key, bool> predicate, out Key key)
        {
            key = FindOrDefault(predicate);
            return key != null;
        }

        /// <summary>
        /// Parses a <see cref="JsonElement"/> array into a new <see cref="Key"/> instances
        /// and adds them to the internal collection.
        /// </summary>
        /// <param name="jsonElement">The json array</param>
        internal void ParseAndAdd(JsonElement jsonElement)
        {
            foreach (var jsonArrayElement in jsonElement.EnumerateArray())
            {
                if (jsonArrayElement.ValueKind == JsonValueKind.Object)
                {
                    _keys.Add(Key.Parse(jsonArrayElement, _document));
                }
            }
        }
    }
}