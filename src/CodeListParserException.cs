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

namespace OpenCodeList
{
    /// <summary>
    /// Represents an error that occurs during JSON parsing inside a <see cref="CodeListDocument"/> or 
    /// a <see cref="CodeListSetDocument"/> instance.
    /// </summary>
    [Serializable]
    public class CodeListParserException : Exception
    {
        public CodeListParserException(string message)
            : base(message)
        { }
    }
}

