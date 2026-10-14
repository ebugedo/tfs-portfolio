# .NET Code Analysis Style Guide

## Overview
.NET Code Analysis (Roslyn analyzers) enforces code quality, correctness, security, and maintainability standards. This project uses:
- **Microsoft.CodeAnalysis.NetAnalyzers** v9.0.0 - .NET runtime analysis rules (CAxxxx)
- **StyleCop.Analyzers** v1.1.118 - Style and documentation rules (SAxxxx)

Both are configured as `PrivateAssets="all"` in `Directory.Build.targets` to prevent transitive dependency flow.

## Configuration

### Central Configuration Files
| File | Purpose |
|------|---------|
| `Directory.Build.targets` | Analyzer packages, analysis mode, suppressed warnings |
| `Directory.Build.props` | Common MSBuild properties (framework, nullable, analysis level) |
| `stylecop.json` | StyleCop-specific rule configuration |

### Analysis Level & Mode
```xml
<PropertyGroup>
  <AnalysisLevel>latest</AnalysisLevel>
  <AnalysisMode>All</AnalysisMode>
  <EnableNETAnalyzers>true</EnableNETAnalyzers>
  <CodeAnalysisTreatWarningsAsErrors>true</CodeAnalysisTreatWarningsAsErrors>
  <EnforceCodeStyleInBuild>true</EnforceCodeStyleInBuild>
</PropertyGroup>
```

| Setting | Value | Description |
|---------|-------|-------------|
| `AnalysisLevel` | `latest` | Enables all current analyzer rules (includes preview) |
| `AnalysisMode` | `All` | Runs all rules: design, performance, security, style |
| `EnableNETAnalyzers` | `true` | Enables built-in .NET analyzers |
| `CodeAnalysisTreatWarningsAsErrors` | `true` | All analysis warnings become build errors |
| `EnforceCodeStyleInBuild` | `true` | Enforces .editorconfig style rules at build |

### Treat Warnings as Errors (Directory.Build.props)
```xml
<TreatWarningsAsErrors>true</TreatWarningsAsErrors>
<Nullable>enable</Nullable>
<GenerateDocumentationFile>true</GenerateDocumentationFile>
<NoWarn>$(NoWarn);1591</NoWarn>
```

- **CS1591 suppressed**: Missing XML comments on public members (StyleCop SA1600 handles this)
- **Nullable enabled**: Nullable reference types enforced
- **Documentation file generated**: For Swagger/OpenAPI and IDE tooltips

### Suppressed Warnings (Directory.Build.targets)
```xml
<NoWarn>$(NoWarn);
  SA1636;SA1633;SA1200;SA1309;SA1127;SA1101;SA1313;SA1009;SA1623;
  CA1040;CA1721;CA1032;SA1413;SA1513;SA1600;SA1625;CA1812;CA1848;
  CA1305;CA1515;CA1707;SA1615;SA1131;CA1508;CA1062;SA1000
</NoWarn>
```

## Suppressed Rules Catalog

### StyleCop Rules (SAxxxx)
| Rule ID | Name | Category | Justification |
|---------|------|----------|---------------|
| SA1000 | KeywordsMustBeSpacedCorrectly | Spacing | Minor formatting preference |
| SA1101 | PrefixLocalCallsWithThis | Readability | Team prefers implicit `this` |
| SA1127 | UsingDirectivePlacement | Ordering | Legacy code migration |
| SA1131 | ReadonlyFieldDeclaration | Readability | Initialization pattern flexibility |
| SA1200 | UsingDirectivesPlacement | Ordering | Configured `outsideNamespace` in stylecop.json |
| SA1309 | StringLiteralsInExceptions | Maintainability | Pragmatic exception messages |
| SA1313 | ParameterNamesMustBeginWithLowerCaseLetter | Naming | Conflicts with team convention |
| SA1413 | TrailingCommasInMultiLine | Layout | Allowed in initializers |
| SA1513 | ClosingBraceFollowedByBlankLine | Layout | Conflicts with formatter |
| SA1600 | ElementsMustBeDocumented | Documentation | Internal APIs exempt; public API documented |
| SA1615 | PartialMethodAccessibility | Documentation | Explicit not required |
| SA1623 | PropertySummaryDocumentation | Documentation | Single-line properties too verbose |
| SA1625 | ElementDocumentation | Documentation | Partial suppression for DTOs |
| SA1633 | FileHeaderRequiresCopyright | Documentation | Handled by LICENSE file |
| SA1636 | FileHeaderRequiresCopyrightText | Documentation | Handled by LICENSE file |

### .NET Code Analysis Rules (CAxxxx)
| Rule ID | Name | Category | Justification |
|---------|------|----------|---------------|
| CA1032 | ImplementStandardExceptionConstructors | Design | Custom exceptions not needed |
| CA1040 | AvoidEmptyInterfaces | Design | Marker interfaces acceptable |
| CA1062 | ValidateArgumentsOfPublicMethods | Correctness | Validated by FluentValidation |
| CA1305 | SpecifyIFormatProvider | Globalization | Invariant culture default acceptable |
| CA1508 | AvoidDeadConditionalCode | Maintainability | Conditional compilation patterns |
| CA1515 | ParameterNamesShouldBeginWithUpperCase | Naming | Conflicts with SA1313 convention |
| CA1707 | IdentifiersShouldNotMatchKeywords | Naming | Contextual keywords acceptable |
| CA1721 | PropertyNamesShouldNotMatchGetMethods | Naming | Standard naming patterns |
| CA1812 | AvoidUninstantiatedInternalClasses | Performance | DI-registered classes false positive |
| CA1848 | UseNullConditionalOperator | Performance | Explicit null checks preferred |

## Analyzers Included

### Package References (Directory.Build.targets)
```xml
<ItemGroup>
  <PackageReference Include="StyleCop.Analyzers" Version="1.1.118" PrivateAssets="all" />
  <PackageReference Include="Microsoft.CodeAnalysis.NetAnalyzers" Version="9.0.0" PrivateAssets="all" />
</ItemGroup>
```

| Package | Version | Rules Prefix | Focus Area |
|---------|---------|--------------|------------|
| StyleCop.Analyzers | 1.1.118 | SAxxxx | Style, documentation, layout, naming |
| Microsoft.CodeAnalysis.NetAnalyzers | 9.0.0 | CAxxxx | Correctness, design, performance, security, globalization |

### Implicit Analyzers (via SDK)
| Analyzer | Source | Rules Prefix |
|----------|--------|--------------|
| .NET Compiler Platform | .NET SDK 10.0 | CSxxxx |
| IDE Analyzers | .NET SDK 10.0 | IDEXXXX |
| Code Quality | .NET SDK 10.0 | CAxxxx (subset) |

## Rule Categories Deep Dive

### Correctness (CA1xxx, CA2xxx)
| Rule | Title | Default Severity | Description |
|------|-------|------------------|-------------|
| CA1001 | TypesThatOwnDisposableFieldsShouldBeDisposable | Warning | Implement IDisposable |
| CA1062 | ValidateArgumentsOfPublicMethods | Warning | Check for null |
| CA1825 | AvoidZeroLengthArrayAllocations | Warning | Use Array.Empty<T>() |
| CA2007 | ConsiderCallingConfigureAwait | Warning | Avoid deadlocks |
| CA2016 | ForwardCancellationToken | Warning | Pass cancellation tokens |
| CA2200 | RethrowToPreserveStackDetails | Warning | Use `throw;` not `throw ex;` |

### Design (CA1xxx)
| Rule | Title | Default Severity | Description |
|------|-------|------------------|-------------|
| CA1000 | DoNotDeclareStaticMembersOnGenericTypes | Warning | Generic static members problematic |
| CA1032 | ImplementStandardExceptionConstructors | Warning | Serialization support |
| CA1040 | AvoidEmptyInterfaces | Warning | Marker interfaces OK if documented |
| CA1063 | ImplementIDisposableCorrectly | Warning | Dispose pattern |
| CA1716 | IdentifiersShouldNotMatchKeywords | Warning | Avoid reserved words |

### Performance (CA18xx)
| Rule | Title | Default Severity | Description |
|------|-------|------------------|-------------|
| CA1812 | AvoidUninstantiatedInternalClasses | Warning | Remove unused or make public |
| CA1822 | MarkMembersAsStatic | Suggestion | No `this` access |
| CA1825 | AvoidZeroLengthArrayAllocations | Warning | Use `Array.Empty<T>()` |
| CA1848 | UseNullConditionalOperator | Suggestion | `?.` and `??` patterns |
| CA1859 | UseConcreteTypesWhenPossible | Suggestion | Avoid interface allocations |

### Security (CA5xxx, CA3xxx)
| Rule | Title | Default Severity | Description |
|------|-------|------------------|-------------|
| CA5351 | DoNotUseBrokenCryptographicAlgorithms | Error | MD5, SHA1, DES, RC2 |
| CA5358 | DoNotUseUnsafeCipherModes | Error | ECB mode |
| CA5361 | DoNotDisableSslCertificateValidation | Error | Validate certificates |
| CA5394 | DoNotUseInsecureRandomness | Warning | Use RandomNumberGenerator |

### Globalization (CA13xx)
| Rule | Title | Default Severity | Description |
|------|-------|------------------|-------------|
| CA1303 | DoNotPassLiteralsAsLocalizedParameters | Warning | Use resources |
| CA1305 | SpecifyIFormatProvider | Warning | Culture-specific formatting |
| CA1309 | UseOrdinalStringComparison | Warning | `StringComparison.Ordinal` |

## Running Code Analysis

### Local Development
```bash
# Full build with analysis (recommended)
dotnet build

# Analysis only (no build output)
dotnet build --no-restore --verbosity minimal 2>&1 | findstr "warning\|error"

# Specific project
dotnet build src/MyProject/MyProject.csproj

# With detailed analyzer output
dotnet build /v:d 2>&1 | findstr "CA\|SA"
```

### CI/CD Pipeline
```yaml
# GitHub Actions example
- name: Restore
  run: dotnet restore

- name: Build & Analyze
  run: dotnet build --no-restore --configuration Release

- name: Run Tests
  run: dotnet test --no-build --configuration Release
```

### IDE Integration
| IDE | Configuration |
|-----|---------------|
| Visual Studio | Enabled by default; **Tools → Options → Text Editor → C# → Code Style → Analyzers** |
| VS Code | Requires **C# Dev Kit** or **OmniSharp** with `roslynAnalyzersEnabled: true` |
| Rider | **Settings → Editor → Inspection Settings → .NET Core Analyzers** |

## Adding / Modifying Rules

### Add New Analyzer Package
```xml
<!-- Directory.Build.targets -->
<ItemGroup>
  <PackageReference Include="Meziantou.Analyzer" Version="2.x.x" PrivateAssets="all" />
</ItemGroup>
```

### Modify Rule Severity (per project)
Create `Directory.Build.props` in project folder:
```xml
<Project>
  <PropertyGroup>
    <NoWarn>$(NoWarn);CA1822</NoWarn>
  </PropertyGroup>
</Project>
```

### Modify Rule Severity (EditorConfig)
```ini
# .editorconfig
[*.cs]
# CA1822 = suggestion, warning, error, none
dotnet_diagnostic.CA1822.severity = suggestion
```

### Suppress Specific Instance
```csharp
#pragma warning disable CA1822 // MarkMembersAsStatic
public int Compute() => 42;
#pragma warning restore CA1822
```

### Suppress with Justification (Recommended)
```csharp
// Suppress: "This method is called via reflection by DI container"
#pragma warning disable CA1822
public int Compute() => 42;
#pragma warning restore CA1822
```

## Project-Specific Overrides

### Test Projects (test/*/Directory.Build.props)
```xml
<Project>
  <PropertyGroup>
    <IsTestProject>true</IsTestProject>
    <NoWarn>$(NoWarn);CA1034;CA1707;CA1812</NoWarn>
  </PropertyGroup>
</Project>
```
- Test-specific suppressions (nesting, naming, instantiation)

### Generated Code
Files matching patterns automatically excluded:
- `*.generated.cs`
- `*.g.cs`
- `*.designer.cs`
- Files with `<auto-generated />` header

## Troubleshooting

### Common Build Failures
| Error | Cause | Fix |
|-------|-------|-----|
| `CA1062` on validated params | Analyzer doesn't see FluentValidation | Add `[ValidatedNotNull]` or suppress |
| `CA1812` on DI services | Internal class not directly instantiated | Make public or suppress |
| `SA1600` on records/DTOs | Auto-properties need docs | Suppress for data types |
| `CA1305` on logging | `ILogger` uses invariant culture | Acceptable; suppress if noisy |

### Performance Optimization
```xml
<!-- Directory.Build.props - limit analysis scope -->
<PropertyGroup>
  <RunAnalyzersDuringBuild>true</RunAnalyzersDuringBuild>
  <RunAnalyzersDuringLiveAnalysis>false</RunAnalyzersDuringLiveAnalysis>
</PropertyGroup>
```

### Analyzer Version Conflicts
```bash
# Clear NuGet cache
dotnet nuget locals all --clear

# Restore with locked versions
dotnet restore --locked-mode
```

## Upgrading Analyzers

### Version Update Process
1. Update versions in `Directory.Build.targets`
2. Run `dotnet build` locally
3. Review **new warnings/errors** in output
4. Categorize: Fix | Suppress (with justification) | Defer
5. Update this document's suppressed rules table
6. Commit changes

### Breaking Changes Checklist
- [ ] New rules enabled by default
- [ ] Rule severity changes (suggestion → warning)
- [ ] Rule renames (CAxxxx → CAyyyy)
- [ ] Removed rules
- [ ] Performance impact

## References
- [Microsoft.CodeAnalysis.NetAnalyzers](https://github.com/dotnet/roslyn-analyzers)
- [StyleCop.Analyzers](https://github.com/DotNetAnalyzers/StyleCopAnalyzers)
- [Analyzer Configuration](https://learn.microsoft.com/en-us/dotnet/fundamentals/code-analysis/configuration-options)
- [Rule Reference](https://learn.microsoft.com/en-us/dotnet/fundamentals/code-analysis/quality-rules/index)
- [Suppression Documentation](https://learn.microsoft.com/en-us/dotnet/fundamentals/code-analysis/suppress-warnings)