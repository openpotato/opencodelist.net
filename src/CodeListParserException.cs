#region OpenCodeList.NET - Copyright (c) STÜBER SYSTEMS GmbH
/*    
 *    OpenCodeList.NET 
 *    
 *    Copyright (c) STÜBER SYSTEMS GmbH
 *
 *    Licensed under the MIT License. 
 * 
 */
#endregion

using System;

namespace OpenCodeList;

/// <summary>
/// Represents an error that occurs during JSON parsing inside a <see cref="CodeListDocument"/> or 
/// a <see cref="CodeListSetDocument"/> instance.
/// </summary>
[Serializable]
public class CodeListParserException : Exception
{
    /// <summary>
    /// Initializes a new instance of the <see cref="CodeListParserException"/> class.
    /// </summary>
    /// <param name="message">The error message that explains the reason for the exception.</param>
    public CodeListParserException(string message)
        : base(message)
    { }

    /// <summary>
    /// Initializes a new instance of the <see cref="CodeListParserException"/> class.
    /// </summary>
    /// <param name="message">The error message that explains the reason for the exception.</param>
    /// <param name="innerException">The exception that is the cause of the current exception.</param>
    public CodeListParserException(string message, Exception innerException)
        : base(message, innerException)
    { }
}

