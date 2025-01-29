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
using System.Linq;
using System.Text.Json;

namespace OpenCodeList
{
    /// <summary>
    /// Foreign keys of a code list.
    /// </summary>
    public class ForeignKeys : IEnumerable<ForeignKey>
    {
        private readonly CodeListDocument _document;
        private readonly List<ForeignKey> _foreignKeys = [];

        /// <summary>
        /// Initializes a new instance of the <see cref="ForeignKeys"/> class.
        /// </summary>
        /// <param name="document">The owner of the data set</param>
        internal ForeignKeys(CodeListDocument document)
        {
            _document = document;
        }

        /// <summary>
        /// Number of foreign keys 
        /// </summary>
        public int Count => _foreignKeys.Count;

        /// <summary>
        /// List of foreign keys by ID
        /// </summary>
        public ForeignKey this[string foreignKeyId]
        {
            get
            {
                var i = IndexOf(x => x.Id == foreignKeyId);
                if (i != -1)
                {
                    return _foreignKeys[i];
                }
                else
                {
                    throw new Exception($"Foreign key with ID \"{foreignKeyId}\" not found");
                }
            }
            set
            {
                var i = IndexOf(x => x.Id == foreignKeyId);
                if (i != -1)
                {
                    _foreignKeys[i] = value;
                }
                else
                {
                    throw new Exception($"Foreign key with ID \"{foreignKeyId}\" not found");
                }
            }
        }

        /// <summary>
        /// List of foreign keys by index
        /// </summary>
        public ForeignKey this[int index]
        {
            get
            {
                return _foreignKeys[index];
            }
            set
            {
                _foreignKeys[index] = value;
            }
        }

        /// <summary>
        /// Creates a new and empty foreign key and adds it to the internal foreign key collection
        /// </summary>
        /// <returns>The new <see cref="ForeignKey"/> instance</returns>
        public ForeignKey Add()
        {
            var foreignKey = new ForeignKey(_document);
            _foreignKeys.Add(foreignKey);
            return foreignKey;
        }

        /// <summary>
        /// Removes all foreign keys from the internal foreign key collection
        /// </summary>
        public void Clear()
        {
            _foreignKeys.Clear();
        }

        /// <summary>
        /// Does a certain foreign key exist?
        /// </summary>
        /// <param name="predicate">A function to test each element for a condition</param>
        /// <returns>True, if found</returns>
        public bool Contains(Func<ForeignKey, bool> predicate)
        {
            return _foreignKeys.Any(predicate);
        }

        /// <summary>
        /// Finds a certain foreign key
        /// </summary>
        /// <param name="predicate">A function to test each element for a condition</param>
        /// <returns>A <see cref="ForeignKey"/> instance or null</returns>
        public ForeignKey FindOrDefault(Func<ForeignKey, bool> predicate)
        {
            return _foreignKeys.FirstOrDefault(predicate);
        }

        /// <summary>
        /// Support for iteration over a <see cref="ForeignKey"/> collection
        /// </summary>
        /// <returns>Enumerator</returns>
        IEnumerator<ForeignKey> IEnumerable<ForeignKey>.GetEnumerator()
        {
            return _foreignKeys.GetEnumerator();
        }

        /// <summary>
        /// Support for iteration over a non-generic collection.
        /// </summary>
        /// <returns>Enumerator</returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return _foreignKeys.GetEnumerator();
        }

        /// <summary>
        /// Index of a foreign key
        /// </summary>
        /// <param name="predicate">A function to test each element for a condition</param>
        /// <returns>Index if found, otherwise -1</returns>
        public int IndexOf(Func<ForeignKey, bool> predicate)
        {
            for (int i = 0; i < _foreignKeys.Count; i++)
            {
                if (predicate(_foreignKeys[i]))
                {
                    return i;
                }
            }
            return -1;
        }

        /// <summary>
        /// Removes a foreign key
        /// </summary>
        /// <param name="foreignKey">The foreign key</param>
        /// /// <returns>TRUE, if successfull</returns>
        public bool Remove(ForeignKey foreignKey)
        {
            return _foreignKeys.Remove(foreignKey);
        }

        /// <summary>
        /// Removes all foreign keys with reference to a given column
        /// </summary>
        /// <param name="column">The column</param>
        /// <returns>Number of removed foreign keys</returns>
        public int RemoveAll(Column column)
        {
            return _foreignKeys.RemoveAll(x => x.Columns.Contains(column));
        }

        /// <summary>
        /// Tries to find a certain foreign key
        /// </summary>
        /// <param name="predicate">A function to test each element for a condition</param>
        /// <param name="foreignKey">The <see cref="ForeignKey"/> instancce if found</param>
        /// <returns>True, if successfull</returns>
        public bool TryFind(Func<ForeignKey, bool> predicate, out ForeignKey foreignKey)
        {
            foreignKey = FindOrDefault(predicate);
            return foreignKey != null;
        }

        /// <summary>
        /// Parses a <see cref="JsonElement"/> array into a new <see cref="ForeignKey"/> instances
        /// and adds them to the internal collection.
        /// </summary>
        /// <param name="jsonElement">The json array</param>
        internal void ParseAndAdd(JsonElement jsonElement)
        {
            foreach (var jsonArrayElement in jsonElement.EnumerateArray())
            {
                if (jsonArrayElement.ValueKind == JsonValueKind.Object)
                {
                    _foreignKeys.Add(ForeignKey.Parse(jsonArrayElement, _document));
                }
            }
        }
    }
}