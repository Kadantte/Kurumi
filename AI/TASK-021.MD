NOTE FOR CLAUDE CODE: Do not make assumptions, mock data, or placeholders. Always implement production-ready code.
GLOBAL UPGRADE REQUIREMENTS
- Upgrade all NuGet packages to the latest stable versions compatible with .NET 8.
- Migrate all projects to .NET 8 (net8.0) and update code for any breaking changes.
- Update the Discord API library to the latest stable version.
- Replace prefix commands with Discord slash commands and update parsing, modules, and documentation accordingly.

# TASK-021 - Glossary Documentation

## Source
- CLAUDE-REPORT appendix A (Glossary)

## Objective
- Maintain a consistent glossary for project and domain terminology.

## Tasks
1. Create or update a glossary document and keep it versioned.
2. Ensure glossary terms match the domain language used in code and docs.

## Code Snippet
```markdown
| Term | Definition |
|------|------------|
| Doujin | Self-published Japanese work, often manga |
| Feed Channel | Auto-posting channel for new content |
```

## Acceptance Criteria
- Glossary exists in documentation and is referenced by README.
- Definitions are consistent and current.

