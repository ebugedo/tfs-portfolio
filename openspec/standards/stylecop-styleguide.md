# StyleCop Style Guide

## Overview
StyleCop.Analyzers enforces a consistent set of style, documentation, and layout rules for C# code across this project. This document defines the standards, configuration, and workflows applied to maintain code quality.

## Configuration
The StyleCop configuration is defined in `stylecop.json` at the solution root, using the official schema from the StyleCop.Analyzers repository.

### Documentation Rules
```json
"documentationRules": {
  "companyName": "Tfs.Portfolio",
  "copyrightText": "Copyright (c) Tfs.Portfolio. All rights reserved.\nLicensed under the MIT license. See LICENSE file in the project root for full license information.",
  "documentInternalElements": false,
  "xmlHeader": false
}
```

| Setting | Value | Description |
|---------|-------|-------------|
| `companyName` | `Tfs.Portfolio` | Company name for file headers |
| `copyrightText` | MIT license text | Copyright notice (multi-line supported) |
| `documentInternalElements` | `false` | Internal types/members don't require XML docs |
| `xmlHeader` | `false` | No XML declaration required at file start |

**Implication**: Public and protected members require XML documentation (`///` comments). Internal and private members are exempt.

### Layout Rules
```json
"layoutRules": {
  "newlineAtEndOfFile": "require"
}
```

- **Require** final newline at end of every source file (SA1651/SA1652)
- Prevents diff noise and ensures POSIX compliance

### Naming Rules
```json
"namingRules": {
  "allowCommonHungarianPrefixes": true,
  "allowedHungarianPrefixes": ["i", "id", "ui", "io", "ip", "on", "db", "api", "dto", "html", "css", "js", "ts", "sql", "xml", "json", "yaml", "gui", "cli", "sdk", "ide", "os", "cpu", "gpu", "ram", "ssd", "hdd", "url", "uri", "http", "https", "tcp", "udp", "dns", "dhcp", "ftp", "ssh", "ssl", "tls", "api", "rest", "grpc", "crud", "acl", "jwt", "oidc", "saml", "rsa", "aes", "sha", "md5", "crc", "utf", "ascii", "iso", "ansi", "unicode"]
}
```

- **Common Hungarian prefixes allowed**: Recognizes common technical acronyms as valid prefixes (e.g., `idUser`, `apiClient`, `dtoResponse`)
- **Casing enforcement**: PascalCase for types/members, camelCase for parameters/locals
- **Prefix list**: Extensive coverage of .NET/web/cloud/database acronyms

### Ordering Rules
```json
"orderingRules": {
  "systemUsingDirectivesFirst": true,
  "usingDirectivesPlacement": "outsideNamespace"
}
```

| Rule | Setting | Enforced By |
|------|---------|-------------|
| System usings first | `true` | SA1210, SA1208 |
| Using placement | `outsideNamespace` | SA1200 |

**Ordering within groups**:
1. `System.*` namespaces
2. Other .NET framework namespaces
3. Third-party package namespaces
4. Project/local namespaces
5. Aliased usings (alphabetical within each group)

## Running StyleCop

### Command Line
```bash
# Run on entire solution
dotnet build

# Run StyleCop specifically (if using global tool)
dotnet stylecop

# Run with verbose output
dotnet build /v:d | findstr "SA"
```

### Visual Studio / IDE
- **Visual Studio**: Enabled by default with `StyleCop.Analyzers` NuGet package
- **VS Code**: Requires C# Dev Kit or OmniSharp with analyzers enabled
- **JetBrains Rider**: Built-in StyleCop support via Roslyn analyzers

### CI/CD Integration
```yaml
# Example GitHub Actions step
- name: Run StyleCop
  run: dotnet build --no-restore --verbosity minimal
```

Build fails on any StyleCop violation due to `TreatWarningsAsErrors=true` and `CodeAnalysisTreatWarningsAsErrors=true`.

## Rule Categories Reference

### Documentation Rules (SA16xx)
| Rule ID | Name | Severity | Description |
|---------|------|----------|-------------|
| SA1600 | ElementsMustBeDocumented | Warning | Public/protected types and members must have XML docs |
| SA1601 | PartialElementsMustBeDocumented | Warning | Partial elements must be documented |
| SA1602 | EnumerationItemsMustBeDocumented | Warning | Enum values must be documented |
| SA1611 | ElementParametersMustBeDocumented | Warning | Method parameters must have `<param>` |
| SA1612 | ElementTypeParametersMustBeDocumented | Warning | Generic type params must have `<typeparam>` |
| SA1614 | ElementParameterDocumentationMustMatch | Warning | `<param>` names must match signature |
| SA1615 | ElementReturnValueMustBeDocumented | Warning | Non-void returns must have `<returns>` |
| SA1616 | ElementReturnValueDocumentationMustMatch | Warning | `<returns>` must match actual return |
| SA1618 | GenericTypeParametersMustBeDocumented | Warning | Generic type params must be documented |

### Layout Rules (SA15xx)
| Rule ID | Name | Severity | Description |
|---------|------|----------|-------------|
| SA1500 | BracesForMultiLineStatementsMustNotShareLine | Warning | Opening brace on new line |
| SA1501 | StatementMustNotBeOnSingleLine | Warning | Multi-line statements properly formatted |
| SA1502 | ElementMustNotBeOnSingleLine | Warning | Type/method declarations not on single line |
| SA1503 | BracesMustNotBeOmitted | Warning | Braces required for control structures |
| SA1505 | OpeningBraceMustNotBePrecededByBlankLine | Warning | No blank line before `{` |
| SA1506 | OpeningBraceMustNotBeFollowedByBlankLine | Warning | No blank line after `{` |
| SA1507 | ClosingBraceMustNotBePrecededByBlankLine | Warning | No blank line before `}` |
| SA1513 | ClosingBraceMustBeFollowedByBlankLine | Warning | Blank line after `}` (except else/catch/finally) |
| SA1516 | ElementsMustBeSeparatedByBlankLine | Warning | Members separated by blank line |

### Naming Rules (SA13xx)
| Rule ID | Name | Severity | Description |
|---------|------|----------|-------------|
| SA1300 | ElementMustBeginWithUpperCaseLetter | Warning | Types/members PascalCase |
| SA1301 | ElementMustBeginWithLowerCaseLetter | Warning | Parameters/locals camelCase |
| SA1302 | InterfaceNamesMustBeginWithI | Warning | Interfaces prefixed with `I` |
| SA1303 | ConstFieldNamesMustBeginWithUpperCaseLetter | Warning | Constants PascalCase |
| SA1304 | NonPrivateReadonlyFieldsMustBeginWithUpperCaseLetter | Warning | Public/protected readonly PascalCase |
| SA1305 | FieldNamesMustNotUseHungarianNotation | Warning | No Hungarian (except allowed prefixes) |
| SA1306 | FieldNamesMustBeginWithLowerCaseLetter | Warning | Private fields camelCase |
| SA1307 | AccessibleFieldsMustBeginWithUpperCaseLetter | Warning | Public/protected fields PascalCase |
| SA1308 | VariableNamesMustNotBePrefixed | Warning | No prefixes on local variables |
| SA1309 | FieldNamesMustNotBeginWithUnderscore | Warning | No leading underscore |
| SA1310 | FieldNamesMustNotContainUnderscore | Warning | No underscores in field names |
| SA1311 | StaticReadonlyFieldsMustBeginWithUpperCaseLetter | Warning | Static readonly PascalCase |
| SA1312 | VariableNamesMustBeginWithLowerCaseLetter | Warning | Locals/parameters camelCase |
| SA1313 | ParameterNamesMustBeginWithLowerCaseLetter | Warning | Parameters camelCase |

### Ordering Rules (SA12xx)
| Rule ID | Name | Severity | Description |
|---------|------|----------|-------------|
| SA1200 | UsingDirectivesMustBePlacedWithinNamespace | Warning | Usings outside namespace (configured) |
| SA1201 | UsingDirectivesMustBePlacedCorrectly | Warning | Using placement consistent |
| SA1202 | UsingDirectivesMustBeOrderedAlphabeticallyByNamespace | Warning | Alphabetical within groups |
| SA1204 | StaticElementsMustAppearBeforeInstanceElements | Warning | Static before instance members |
| SA1206 | DeclarationKeywordsMustFollowOrder | Warning | public/protected/internal/private order |
| SA1208 | SystemUsingDirectivesMustBePlacedBeforeOtherUsingDirectives | Warning | System usings first |
| SA1210 | UsingDirectivesMustBeOrderedAlphabeticallyByNamespace | Warning | Alphabetical ordering |

### Spacing Rules (SA10xx)
| Rule ID | Name | Severity | Description |
|---------|------|----------|-------------|
| SA1000 | KeywordsMustBeSpacedCorrectly | Warning | Space after keywords (if, while, etc.) |
| SA1001 | CommasMustBeSpacedCorrectly | Warning | Space after comma, none before |
| SA1002 | SemicolonsMustBeSpacedCorrectly | Warning | No space before, space after |
| SA1003 | SymbolsMustBeSpacedCorrectly | Warning | Operators spaced correctly |
| SA1008 | OpeningParenthesisMustBeSpacedCorrectly | Warning | No space after `(` |
| SA1009 | ClosingParenthesisMustBeSpacedCorrectly | Warning | No space before `)` |
| SA1010 | OpeningSquareBracketMustBeSpacedCorrectly | Warning | No space after `[` |
| SA1011 | ClosingSquareBracketMustBeSpacedCorrectly | Warning | No space before `]` |
| SA1013 | OpeningCurlyBracketMustBeSpacedCorrectly | Warning | Space before `{` |
| SA1014 | ClosingCurlyBracketMustBeSpacedCorrectly | Warning | Space after `}` (where applicable) |

## Suppressing Rules

### Per-Instance Suppression
```csharp
#pragma warning disable SA1600 // ElementsMustBeDocumented
public class InternalHelper { }
#pragma warning restore SA1600
```

### Project-Level Suppression (Directory.Build.props)
```xml
<NoWarn>$(NoWarn);SA1600;SA1601;SA1602</NoWarn>
```

### File-Level Suppression
```csharp
// <auto-generated />
// Suppresses all StyleCop rules for generated files
```

### Suppression Guidelines
1. **Prefer fixing** over suppressing
2. **Document justification** in code comment when suppressing
3. **Scope narrowly**: Use `#pragma` over project-level `NoWarn`
4. **Review periodically**: Suppressions should be revisited during upgrades
5. **Generated code**: Use `<auto-generated />` or `.generated.cs` suffix

### Currently Suppressed Rules (from Directory.Build.targets)
| Rule | Justification |
|------|---------------|
| SA1636 | File header copyright - handled by license file |
| SA1633 | File header requires copyright - redundant with LICENSE |
| SA1200 | Using directives placement - configured outside namespace |
| SA1309 | String literals in exceptions - pragmatically allowed |
| SA1127 | Invalid using directive placement - legacy code |
| SA1101 | Prefix local calls with this. - style preference |
| SA1313 | Parameter naming - conflicts with team convention |
| SA1009 | Closing parenthesis spacing - minor formatting |
| SA1623 | Single-line property documentation - too verbose |
| SA1413 | Trailing commas - allowed in multi-line initializers |
| SA1513 | Blank line after closing brace - conflicts with formatting |
| SA1600 | Documentation requirement - internal APIs exempt |
| SA1625 | Element documentation - partial suppression |
| SA1615 | Partial method accessibility - explicit not required |
| SA1131 | Readonly fields - initialization pattern |
| SA1000 | Keyword spacing - minor formatting preference |

## Custom Rules & Extensions

### Adding Custom Rules
1. Create a custom analyzer project or rule set
2. Reference in `Directory.Build.targets`:
```xml
<PackageReference Include="Custom.StyleCop.Rules" Version="1.0.0" PrivateAssets="all" />
```
3. Configure in `stylecop.json` under `settings`

### Project-Specific Configurations
Individual projects can override by placing a `stylecop.json` in the project root. Settings merge with solution-level config.

## IDE Configuration

### EditorConfig Integration
Add to `.editorconfig` for consistent formatting:
```ini
[*.cs]
indent_style = space
indent_size = 4
insert_final_newline = true
```

### Visual Studio Settings
- **Tools → Options → Text Editor → C# → Code Style**: Match StyleCop rules
- **Analyzers**: Ensure StyleCop.Analyzers is enabled for solution

## Troubleshooting

### Common Issues
| Issue | Resolution |
|-------|------------|
| False positives on generated code | Add `<auto-generated />` or use `.generated.cs` |
| Performance slow on large solutions | Exclude test projects via `.editorconfig` or `NoWarn` |
| Conflicts with ReSharper/Rider | Disable duplicate inspections in IDE |
| SA1600 on DTOs/Records | Consider suppressing for data-only types |

### Upgrading StyleCop.Analyzers
1. Update version in `Directory.Build.targets`
2. Run `dotnet build` to detect new violations
3. Review release notes for breaking changes
4. Update suppressions as needed
5. Update this document

## References
- [StyleCop.Analyzers GitHub](https://github.com/DotNetAnalyzers/StyleCopAnalyzers)
- [Rule Documentation](https://github.com/DotNetAnalyzers/StyleCopAnalyzers/blob/master/documentation/Readme.md)
- [Schema Definition](https://raw.githubusercontent.com/DotNetAnalyzers/StyleCopAnalyzers/master/StyleCop.Analyzers/StyleCop.Analyzers/Settings/stylecop.schema.json)