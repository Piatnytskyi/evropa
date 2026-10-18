---
name: evropa-clean-architecture
description: Clean-architecture layering, project map, dependency rules and folder/namespace conventions for the Evropa game solution (Godot client, Mathematics library, World Domain/Application, tests). Use when adding, moving or renaming classes, folders, namespaces or projects in Evropa, deciding where new code belongs, wiring dependency injection, or adding new game contexts such as networking, server, NPC behaviour, economy, politics, culture, audio or battle.
---

# Evropa Clean Architecture

Evropa is a Godot 4 (.NET 8) game. The client renders; game logic lives in plain .NET projects so a future authoritative server can run it and stream world data to clients. Keep every change compatible with that direction.

## Solution map

```text
Evropa.sln                         solution folders: Client / Shared / World / Tests
src/
  Evropa.Client/                   Godot project (presentation + composition root)
  Evropa.Mathematics/              pure algorithms shared by every context
  Evropa.World.Domain/             world model (grids, cells, ...)
  Evropa.World.Application/        world use cases (e.g. Planets/PlanetSurfaceGenerator)
tests/
  Evropa.Mathematics.UnitTests/
  Evropa.World.UnitTests/
```

Every bounded context follows `Evropa.<Context>.<Layer>` with layers `Domain`, `Application`, `Infrastructure`. Create a layer project only when it has real content; do not scaffold empty projects.

## Dependency rules

References point inward only. The compiler enforces boundaries through project references, so never add a reference that breaks this table.

| Project | May reference |
|---|---|
| `Evropa.Mathematics` | no solution projects; only MathNet.Numerics and `Microsoft.Extensions.DependencyInjection.Abstractions` |
| `Evropa.<Context>.Domain` | `Evropa.Mathematics` (and a future `Evropa.SharedKernel`) |
| `Evropa.<Context>.Application` | its own Domain, `Evropa.Mathematics`, DI abstractions; other contexts only through their Application interfaces or `Evropa.Contracts` |
| `Evropa.<Context>.Infrastructure` | its own Application and Domain, external libraries (I/O, transport, storage) |
| Hosts (`Evropa.Client`, future `Evropa.Server`) | Application and Infrastructure projects; the only place that builds an `IServiceProvider` |
| `Evropa.Contracts` (future) | nothing; network DTOs and messages only |
| Test projects | the project under test (and its transitive references) |

Forbidden everywhere except `Evropa.Client`: references to Godot (`Godot.NET.Sdk`, `Godot.*` types). Use `System.Numerics` vectors in all non-client code and convert to Godot types in the client.

## Where does new code go?

1. Pure, deterministic algorithm with no game meaning (noise, sampling, triangulation, projection, tessellation, smoothing, DSP)? Put it in `Evropa.Mathematics/<Feature>/`.
2. A game concept with state and rules (planet, tile, settlement, faction, culture, unit)? Put it in `Evropa.<Context>.Domain`.
3. Orchestration of a scenario (generate a planet, advance the economy tick, resolve a battle), or an interface to the outside world (storage, network, clock)? Put it in `Evropa.<Context>.Application`, and declare the outside-world interface there as a port.
4. An implementation of a port that does I/O (files, database, sockets, serialization)? Put it in `Evropa.<Context>.Infrastructure`.
5. Rendering, input, scenes, audio playback, camera? Put it in `Evropa.Client`. Godot nodes call Application interfaces and only translate results into Godot objects.

If logic shows up inside a Godot node (as the planet pipeline once did in `Meshes/World.cs`), extract it into an Application use case.

## Folder and namespace conventions

- Organize by feature, never by kind. Do not create `Abstractions/`, `Implementations/`, `Interfaces/`, `Enums/` or `Structs/` folders. An interface sits next to its implementations (`Triangulation/ITriangulator.cs` + `Triangulation/DelaunayTriangulator.cs`).
- When a feature folder gets crowded, split it by role inside the feature: contracts stay at the feature root, and variants go one level down (e.g. `Sampling/Strategies/`, `Sampling/Regions/`).
- Namespace = project name + folder path (`src/Evropa.Mathematics/Sampling/Regions` → `Evropa.Mathematics.Sampling.Regions`).
- Use file-scoped namespaces with `using` directives placed after the namespace line, matching existing files.
- Never put a `Godot` segment in a namespace (it would shadow the `Godot` namespace), and avoid a namespace segment named `Math` (it shadows `System.Math`).
- Interfaces use the `I` prefix. Strategy-style interfaces inside `Evropa.Mathematics` are algorithm seams, not architectural ports; keep them in the same project.

## Dependency injection

- Each project with services exposes one `DependencyInjection` static class at its root with an `Add<Project>()` extension, e.g. `services.AddMathematics()` and `services.AddWorldApplication()`.
- Libraries reference only `Microsoft.Extensions.DependencyInjection.Abstractions`. Only hosts reference `Microsoft.Extensions.DependencyInjection` and call `BuildServiceProvider()`.
- The client's composition root is `src/Evropa.Client/ServiceProviderFactory.cs`. Godot nodes cannot use constructor injection, so they resolve services from `ServiceProviderFactory.ServiceProvider` in `_Ready`.
- Constructor-inject dependencies in non-client code and guard them with `?? throw new ArgumentNullException(nameof(x))`.

## Client/server readiness

- **Determinism:** generation (world, culture, voices) must be reproducible from a seed so the server and client agree. Inject `RandomSource` (or a seeded abstraction) and never create unseeded RNGs inside algorithms or domain code.
- **Domain types never go over the network.** Map them to DTOs in `Evropa.Contracts` at the Application/Infrastructure boundary.
- **Authority:** simulation (NPC domestic/political/economic behaviour, battles) is server-side Application/Domain code. The client gets state through Contracts, not by referencing those simulation projects. Presentation-only generation (visual effects, voice synthesis playback) may run on the client.
- Long-running behaviour systems that may become separate processes get their own context and host (e.g. `Evropa.Economy.*` plus a worker host). They communicate through Contracts and messages, not shared in-memory state.

Planned contexts, named so they slot into the rules above: `Evropa.Server`, `Evropa.Contracts`, `Evropa.Networking`, `Evropa.World.Infrastructure`, `Evropa.Society.*`, `Evropa.Politics.*`, `Evropa.Economy.*`, `Evropa.Culture.*`, `Evropa.Audio.Synthesis`, `Evropa.Battle.*`.

## Godot client specifics

- `project.godot` → `dotnet/project/assembly_name` must equal the client `.csproj` file name (`Evropa.Client`). Rename both together.
- Godot compiles every `.cs` under `src/Evropa.Client/`, so never place other projects inside it.
- Scenes reference scripts by `res://` path and `uid`. When moving a script, move its `.cs.uid` file with it and update the `path=` in every `.tscn` that uses it.
- The Godot SDK has no `Release` configuration. In `Evropa.sln` the client's `Release|Any CPU` maps to `Debug|Any CPU`; preserve that mapping when editing the solution.
- `.vscode/launch.json` launches `$GODOT4` with `cwd` = `src/Evropa.Client`; update it if the client moves.

## Adding a new project

1. Create `src/Evropa.<Context>.<Layer>/Evropa.<Context>.<Layer>.csproj` targeting `net8.0` with `ImplicitUsings` and `Nullable` enabled.
2. Add only the project references the dependency table allows.
3. Add it to the matching solution folder: `dotnet sln Evropa.sln add src/<Project>/<Project>.csproj --solution-folder <Context>`. Check that the Godot client's Release→Debug mapping is untouched.
4. Add `DependencyInjection.Add<Project>()` if it registers services, and call it from the host composition root(s).
5. Add tests in `tests/Evropa.<Context>.UnitTests/` (xUnit; put `ClassData` providers in `Data/` named `<Subject>PositiveData` / `<Subject>NegativeData`; name tests `Method_Condition_Expectation`).

## Verify every structural change

```bash
dotnet build && dotnet test --no-build
"$GODOT4" --headless --path src/Evropa.Client --quit-after 30   # client still loads its scripts
```

NuGet restore needs network access. Godot loading the scene with no `ERROR` lines in the output means the client assembly and script paths are intact.
