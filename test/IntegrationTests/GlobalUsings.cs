// <copyright file="GlobalUsings.cs" company="Tfs.Portfolio">
// Copyright (c) Tfs.Portfolio. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

global using System;
global using System.Collections.Generic;
global using System.Diagnostics.CodeAnalysis;
global using System.Linq;
global using System.Threading.Tasks;
global using FluentAssertions;
global using Microsoft.EntityFrameworkCore;
global using Tfs.Portfolio.Application.Clients.Commands;
global using Tfs.Portfolio.Application.Clients.Dtos;
global using Tfs.Portfolio.Application.Clients.Queries;
global using Tfs.Portfolio.Application.Common.CQRS;
global using Tfs.Portfolio.Application.Common.Modules;
global using Tfs.Portfolio.Application.Common.Services;
global using Tfs.Portfolio.Application.CompanyProfile.Commands;
global using Tfs.Portfolio.Application.CompanyProfile.Dtos;
global using Tfs.Portfolio.Application.CompanyProfile.Queries;
global using Tfs.Portfolio.Application.Projects.Commands;
global using Tfs.Portfolio.Application.Projects.Dtos;
global using Tfs.Portfolio.Application.Projects.Queries;
global using Tfs.Portfolio.Application.Sectors.Commands;
global using Tfs.Portfolio.Application.Sectors.Dtos;
global using Tfs.Portfolio.Application.Sectors.Queries;
global using Tfs.Portfolio.Application.Services.Commands;
global using Tfs.Portfolio.Application.Services.Dtos;
global using Tfs.Portfolio.Application.Services.Queries;
global using Tfs.Portfolio.Domain.Clients.Entities;
global using Tfs.Portfolio.Domain.Clients.Repositories;
global using Tfs.Portfolio.Domain.Common;
global using Tfs.Portfolio.Domain.Common.ValueObjects;
global using Tfs.Portfolio.Domain.CompanyProfile.Entities;
global using Tfs.Portfolio.Domain.CompanyProfile.Repositories;
global using Tfs.Portfolio.Domain.Projects.Entities;
global using Tfs.Portfolio.Domain.Projects.Repositories;
global using Tfs.Portfolio.Domain.Sectors.Entities;
global using Tfs.Portfolio.Domain.Sectors.Repositories;
global using Tfs.Portfolio.Domain.Services.Entities;
global using Tfs.Portfolio.Domain.Services.Repositories;
global using Tfs.Portfolio.Infrastructure.Persistence;
global using Tfs.Portfolio.Infrastructure.Persistence.Modules;
global using Tfs.Portfolio.IntegrationTests;
global using Xunit;