# RayTracer

A simple ray tracer written in C#.

## Prerequisites

- [.NET 8.0 SDK](https://dotnet.microsoft.com/download) or later
- (All NuGet dependencies are automatically restored)
  - MathNet.Numerics 5.0.0
  - MathNet.Spatial 0.6.0
  - System.Drawing.Common 9.0.1

## Getting Started

### 1. Clone the repository

```bash
git clone https://github.com/DevWalrus/CSCI711-RayTracer.git
cd CSCI711-RayTracer
```

### 2. Build the solution

```bash
dotnet build RayTracer/RayTracer.csproj
```

### 3. Run the ray tracer

```bash
dotnet run --project RayTracer/RayTracer.csproj [options]
```

#### Available options

- `-p, --path <BasePath>`
  - Base directory for **Input/** and **Output/** folders. Defaults to `C:\`.

- `-m, --multithreaded`
  - Enable parallel (multithreaded) rendering. Default is single-threaded.

- `-t, --test`
  - Run the built-in test suite against files in `<BasePath>/Input` and exit.

- `-f, --filename`
  - Adjust the output filename (overrides the default for the scene or bunny routines)

- `-l, --ldmax`
  - Adjust the ldmax provided to the tone reproduction system

- `-i, --intensity`
  - Changes the intensity of the scene (e.g., 0.1, 1, or 10).

- `-r, --tone`
  - Change the tone reproduction model (`Ward` or `Reinhard`).

- `-b, --bunny`
  - Render the Stanford bunny scene

> **No flags**: Renders the default scene to `Scene.ppm`.

#### Examples

```bash
# Render default scene (single-threaded)

dotnet run --project RayTracer/RayTracer.csproj

# Render bunny scene (multithreaded) from a custom path (the bunny file will need to be in the input folder)

dotnet run --project RayTracer/RayTracer.csproj -- -p D:\\MyData -m -b

# Render scene with a custom tone reproduction model

dotnet run --project RayTracer/RayTracer.csproj -- --tone Ward -p D:\\MyData -f Ward_0_1.ppm -i 0.1

# Run tests only

dotnet run --project RayTracer/RayTracer.csproj -- -t
```

## Input & Output

- **Input**: Place your data files (e.g. `bun_zipper.ply`) under `<BasePath>/Input/`.
- **Output**: Rendered PPM images appear in `<BasePath>/Output/`:
  - `Scene.ppm`
  - `Bunny.ppm`

## Acknowledgements

Stanford Bunny model (Greg Turk & Marc Levoy, 1994), from the Stanford Computer Graphics Laboratory’s <a href="https://graphics.stanford.edu/data/3Dscanrep/">3D Scanning Repository</a>.  