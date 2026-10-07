// <copyright file="CompanyProfileAlreadyExistsException.cs" company="Tfs.Portfolio">
// Copyright (c) Tfs.Portfolio. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace Tfs.Portfolio.Domain.CompanyProfile.Exceptions;

using Tfs.Portfolio.Domain.Common;

/// <summary>
/// Exception thrown when attempting to create a company profile when one already exists.
/// </summary>
public sealed class CompanyProfileAlreadyExistsException : DomainException
{
    /// <summary>
    /// Initializes a new instance of the <see cref="CompanyProfileAlreadyExistsException"/> class.
    /// </summary>
    public CompanyProfileAlreadyExistsException()
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="CompanyProfileAlreadyExistsException"/> class with a specified error message.
    /// </summary>
    /// <param name="message">The error message.</param>
    public CompanyProfileAlreadyExistsException(string message)
        : base(message)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="CompanyProfileAlreadyExistsException"/> class with a specified error message and a reference to the inner exception.
    /// </summary>
    /// <param name="message">The error message.</param>
    /// <param name="innerException">The inner exception.</param>
    public CompanyProfileAlreadyExistsException(string message, Exception innerException)
        : base(message, innerException)
    {
    }
}