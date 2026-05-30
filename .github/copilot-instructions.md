# Copilot Instructions for Wayblazer

## Build & Test

```bash
# Build all non-Godot projects (root solution excludes src/Wayblazer):
dotnet build Wayblazer.sln --configuration Release

# Run core tests (xUnit, 209 tests):
dotnet test tests/Wayblazer.Core.Tests

# Run a single test by name:
dotnet test tests/Wayblazer.Core.Tests --filter "FullyQualifiedName~TestMethodName"
```

## Solution Architecture

The root `Wayblazer.sln` contains all projects **except** the Godot game project:

| Project | Framework | Type | Notes |
|---------|-----------|------|-------|
| `src/Wayblazer.Core` | net9.0 | Library | Core data models, game logic systems, and shared types |
| `tests/Wayblazer.Core.Tests` | net9.0 | Tests | xUnit tests for Core |
| `src/Wayblazer.World` | net9.0 | Exe | World generation CLI |
| `src/Wayblazer.Configurator` | net9.0 | Exe | Configuration utility |
| `src/Wayblazer.TilesetProcessor` | net8.0 | Exe | Tileset processing (Windows-only, uses System.Drawing) |
| `src/WFC.Utility` | net8.0 | Library | WFC helpers (Windows-only) |
| `lib/wave-function-collapse/` | — | Submodule | WFC.Core algorithm (git submodule) |

**Not in the solution** (requires Godot engine runtime):
| `src/Wayblazer` | net8.0 | Godot | Uses `Godot.NET.Sdk/4.5.1` — cannot be built or tested without the Godot editor. Do not reference this project from test projects (causes `AccessViolationException`). |

## Conventions

- **No Sprint/iteration references in code.** Do not use "Sprint N:" prefixes in `///` XML doc comments, commit messages, or code comments. Use descriptive names only.
- **Nullable enabled everywhere.** All projects use `<Nullable>enable</Nullable>`.
- **XML documentation.** Core projects have `<GenerateDocumentationFile>true</GenerateDocumentationFile>` with CS1591 suppressed. Add `/// <summary>` comments to public APIs when creating new classes.

## Scope Discipline

When asked to "write tests" for planned features, clarify whether the corresponding implementation should also be written or whether tests should be written as specifications that may initially fail. Do not silently implement full feature systems when only tests were requested.

## Environment

- The `gh` CLI is pre-authenticated. Do not run `gh auth refresh` or `gh auth login` — use `gh` commands directly.
- The `lib/wave-function-collapse` submodule uses SSH (`git@github.com:chrisculy/wave-function-collapse.git`). Ensure submodules are checked out with `--recurse-submodules` or `git submodule update --init --recursive`.
