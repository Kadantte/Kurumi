NOTE FOR CLAUDE CODE: Do not make assumptions, mock data, or placeholders. Always implement production-ready code.
GLOBAL UPGRADE REQUIREMENTS
- Upgrade all NuGet packages to the latest stable versions compatible with .NET 8.
- Migrate all projects to .NET 8 (net8.0) and update code for any breaking changes.
- Update the Discord API library to the latest stable version.
- Replace prefix commands with Discord slash commands and update parsing, modules, and documentation accordingly.

# TASK-012 - Localization and I18n Consistency

## Source
- CLAUDE-REPORT section 12 (Localization and Internationalization)

## Objective
- Ensure localization keys are complete and fallbacks are correct.

## Tasks
1. Audit localization keys across all languages and report missing keys.
2. Add a validation test that loads all dictionaries and checks fallbacks.
3. Ensure localization retrieval uses guild language and defaults to English.

## Implementation Notes
- Localization loading and access is in `nhitomi/Globalization/Localization.cs`.

## Code Snippet
```csharp
public static Localization GetLocalization(string culture) =>
    culture != null && _localizations.TryGetValue(culture.ToLowerInvariant(), out var localization)
        ? localization
        : Default;
```

## Acceptance Criteria
- No missing keys at runtime for supported languages.
- English fallback works for any missing translation.

