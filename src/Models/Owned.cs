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

namespace OpenCodeList;

/// <summary>
/// Represents an object that is owned by another object of type <typeparamref name="TOwner"/>.
/// </summary>
/// <typeparam name="TOwner">The type of the owner object</typeparam>
public abstract class Owned<TOwner> where TOwner : CodeListBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="Owned"/> class.
    /// </summary>
    /// <param name="owner">The owner of the object</param>
    public Owned(TOwner owner)
    {
        Owner = owner;
    }

    /// <summary>
    /// Gets the owner of the object.
    /// </summary>
    protected TOwner Owner { get; }
}