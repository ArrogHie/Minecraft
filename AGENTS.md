# AGENTS.md - Unity Minecraft Project Guidelines

## Project Overview

Unity-based Minecraft clone (Unity 2022.3.62f3c1) with voxel world generation, first-person controller, inventory/crafting UI, and block placement/destruction.

## Build & Test Commands

```bash
# Open in Unity Editor: File > Open Scene > Assets/Scenes/<scene>

# Command-line build (Windows)
Unity.exe -buildTarget WindowsStandalone -quit -batchmode -projectPath "D:\~~~unity\Minecraft" -logFile build.log

# Run all tests via Unity Test Runner (GUI): Window > General > Test Runner > Run All

# Command-line test execution
Unity.exe -runTests -projectPath "D:\~~~unity\Minecraft" -testResults results.xml

# Run a single test (use Test Runner GUI or filter by category in command-line)
```

## Code Style Guidelines

### Naming Conventions

| Element | Convention | Example |
|---------|------------|---------|
| Classes | PascalCase | `PlayerControl`, `InventorySlot` |
| Methods | PascalCase | `TryBreakBlock()`, `CreateMesh()` |
| Variables | camelCase | `activeBlock`, `breakSeconds` |
| Constants | PascalCase | `chunkSize = 16` |
| Enum values | PascalCase | `BlockType.Grass`, `CubeSide.Top` |
| Private fields | camelCase (no prefix) | `xRotation`, `targetBlock` |

### File Organization

```
Assets/Scripts/
├── World/           # World.cs, Chunk.cs, Block.cs
├── Player/         # Player controller
├── DroppedItem/    # Item pickup system
└── UI/             # Inventory, hotbar, crafting
```

### Formatting & Imports

```csharp
// K&R braces, 4 spaces indentation, max 120 chars per line
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
```

### Error Handling

- Return null/bool instead of throwing exceptions
- Use Debug.Log() for diagnostics
- Check null explicitly with `== null` (not `?.`)

## Unity-Specific Patterns

```csharp
private IEnumerator GenerateChunk() { yield return null; }
private void Awake() { instance = this; }
private void Update() { CheckMove(); }
```

### Singleton Pattern
```csharp
public class World : MonoBehaviour {
    public static World instance;
    private void Awake() { instance = this; }
}
```

### Block Interaction
```csharp
Ray ray = camera.ScreenPointToRay(Input.mousePosition);
if (Physics.Raycast(ray, out hit, 5f, LayerMask.GetMask("Chunk"))) { }
```

## Block System

| BlockType | Value |
|-----------|-------|
| Air, Dirt, Grass, Stone | 0-3 |
| Wood, Leaves, Cobblestone | 4-6 |
| Planks, Stick, CraftingTable | 7-9 |
| Coal, Torch | 10-11 |

`BlockFaceType` enum maps to texture UV coordinates.

## Crafting System

- **RecipeManager**: Singleton via `RecipeManager.instance`, `FindRecipe(string[], int[])`
- **CraftingRecipe**: 4-element input array for 2x2 grid, `Matches()` method
- **Current Recipe**: Wood → 4 Planks
- **Inventory**: 4 `craftingInputSlots`, 1 `craftingOutputSlot`, methods `CheckCraftingInput()`, `OnCraftingOutputClick()`, `Craft()`

## Important Notes

1. Layer "Chunk" for block raycasting, Layer "Player" for placement checks
2. Resources.Load() loads sprites from `Resources/Image/Imgs/Block/`
3. Terrain uses `Mathf.PerlinNoise()` with seed
4. Chunk size: 16x48x16 blocks

## Testing New Code

1. Test block placement/destruction in Play mode
2. Test inventory UI and crafting interactions
3. Test chunk loading/unloading at boundaries
4. Check for null reference errors in console
