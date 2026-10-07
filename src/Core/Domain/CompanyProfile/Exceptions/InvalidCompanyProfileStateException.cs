// <copyright file="InvalidCompanyProfileStateException.cs" company="Tfs.Portfolio">
// Copyright (c) Tfs.Portfolio. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace Tfs.Portfolio.Domain.CompanyProfile.Exceptions;

using Tfs.Portfolio.Domain.Common;

/// <summary>
/// Exception thrown when a company profile is in an invalid state.
/// </summary>
public sealed class InvalidCompanyProfileStateException : DomainException
{
    /// <summary>
    /// Initializes a new instance of the <see cref="InvalidCompanyProfileStateException"/> class.
    /// </summary>
    public InvalidCompanyProfileStateException()
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="InvalidCompanyProfileStateException"/> class with a specified error message.
    /// </summary>
    /// <param name="message">The error message.</param>
    public InvalidCompanyProfileStateException(string message)
        : base(message)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="InvalidCompanyProfileStateException"/> class with a specified error message and a reference to the inner exception.
    /// </summary>
    /// <param name="message">The error message.</param>
    /// <param name="innerException">The inner exception.</param>
    public InvalidCompanyProfileStateException(string message, Exception innerException)
        : base(message, innerException)
    {
    }
}