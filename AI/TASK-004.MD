NOTE FOR CLAUDE CODE: Do not make assumptions, mock data, or placeholders. Always implement production-ready code.
GLOBAL UPGRADE REQUIREMENTS
- Upgrade all NuGet packages to the latest stable versions compatible with .NET 8.
- Migrate all projects to .NET 8 (net8.0) and update code for any breaking changes.
- Update the Discord API library to the latest stable version.
- Replace prefix commands with Discord slash commands and update parsing, modules, and documentation accordingly.

# TASK-004 - Project Structure and Build Artifact Hygiene

## Source
- CLAUDE-REPORT section 4 (Project Structure)

## Objective
- Keep repository structure clean and ensure build artifacts do not live in source control.

## Tasks
1. Remove tracked build outputs under `**/obj/**` and confirm they are ignored.
2. Validate solution structure matches the intended projects.
3. Add documentation on folder layout and build output locations.

## Implementation Notes
- Solution structure is defined in `nhitomi.sln`.
- Ignore rules are in `.gitignore`.

## Code Snippet
```text
Project("{FAE04EC0-301F-11D3-BF4B-00C04F79EFBC}") = "nhitomi", "nhitomi\nhitomi.csproj", "{6ECBEDCB-EE7D-4697-80DF-9CC90C6369BE}"
Project("{FAE04EC0-301F-11D3-BF4B-00C04F79EFBC}") = "nhitomi.Core", "nhitomi.Core\nhitomi.Core.csproj", "{43251C3A-0508-483D-8859-639DE3B92154}"
```

## Acceptance Criteria
- No `obj` or `bin` artifacts are tracked.
- The documented structure matches the solution and directory layout.

