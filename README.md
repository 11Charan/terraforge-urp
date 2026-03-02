# TerraForge  
Procedural Terrain Generation with Unity URP

An experiment in AI-assisted prototyping and real-time system refinement.

---

## Why I Built This

I wanted to explore procedural terrain generation in a real-time rendering environment and test how AI coding tools perform in performance-sensitive systems.

Unlike backend SaaS systems, real-time rendering has strict runtime constraints:
- Frame rate stability
- Memory management
- Chunk streaming efficiency
- Mesh generation cost

TerraForge was built to explore the boundary between AI-assisted development and manual system refinement.

---

## Core System

TerraForge generates procedural terrain using:

- Perlin noise height mapping
- Chunk-based terrain loading
- Unity URP rendering pipeline

Each terrain chunk:
- Is generated independently
- Uses noise sampling for height calculation
- Builds mesh data dynamically
- Is streamed based on player position

---

## Architecture

### 1. Noise Generation

- Perlin noise for height values
- Configurable scale and amplitude
- Deterministic seed support

Perlin was chosen for:
- Predictable terrain continuity
- Smooth gradients
- Performance stability

---

### 2. Chunk-Based Streaming

The world is divided into terrain chunks:

- Chunks generated relative to player position
- Out-of-range chunks destroyed to free memory
- Mesh generation performed per chunk

Initial chunk system was AI-generated.
I refactored it to:

- Improve separation between chunk management and mesh generation
- Reduce coupling between player controller and terrain system
- Improve readability and maintainability

---

### 3. Mesh Construction

Each chunk:
- Samples Perlin noise grid
- Constructs vertices and triangles
- Applies normals for lighting
- Updates MeshFilter dynamically

---

## Performance Validation

Since this is a real-time system, I monitored:

- FPS stability during chunk loading
- Frame drops during rapid movement
- Mesh regeneration cost

Observations:

- Stable performance during moderate movement
- Predictable cost per chunk generation
- No major spikes after refactor separation

---

## AI Engineering Approach

TerraForge was partially AI-assisted during early prototyping.

AI was used for:
- Initial chunk loading implementation
- Base mesh generation structure

I manually:

- Refactored chunk management logic
- Improved separation of concerns
- Cleaned up system boundaries
- Validated runtime performance

Key insight:
AI accelerates prototyping well — but real-time systems require manual refinement to maintain clarity and performance guarantees.

---

## What I Learned

- AI is effective for structural scaffolding.
- Real-time systems expose architectural weaknesses quickly.
- Separation of concerns matters more under performance constraints.
- Frame stability is a better metric than raw feature count.

---

## What I Would Improve Next

- Introduce LOD (Level of Detail) system
- Add frustum culling optimization
- Implement object pooling for chunk reuse
- Benchmark chunk size tradeoffs
- Add biome-based layered noise

---

## Tech Stack

- Unity (URP)
- C#
- Perlin Noise-based terrain generation

---

## Future Direction

Next iteration would explore:

- GPU-based noise sampling
- Multithreaded chunk generation
- Biome blending
- Procedural object placement

---
