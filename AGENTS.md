# AGENTS.md - Unity Minecraft Project Guidelines

## Project Overview

This is a Unity-based Minecraft clone. The project uses Unity 2022.3.62f3c1 and contains:
- Voxel-based world generation with chunk system
- First-person player controller
- Inventory and hotbar UI system with crafting
- Block placement and destruction mechanics

## Build & Test Commands

### Building the Project

```bash
# Open in Unity Editor
# File > Open Scene > Assets/Scenes/<scene>

# Command-line build (Windows)
Unity.exe -buildTarget WindowsStandalone -quit -batchmode -projectPath "D:\~~~unity\Minecraft" -logFile build.log
```

### Running Tests

```bash
# Run all tests via Unity Test Runner (GUI)
# Window > General > Test Runner > Run All

# Command-line test execution
Unity.exe -runTests -projectPath "D:\~~~unity\Minecraft" -testResults results.xml
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
├── World/           # World generation (World.cs, Chunk.cs, Block.cs)
├── Player/          # Player controller and related
├── DroppedItem/     # Item pickup system
├── UI/              # Inventory, hotbar, crafting, slots
└── Entity.cs        # Base class for entities
```

### Formatting Rules

```csharp
// Braces: K&R style (same line)
if (condition)
{
    DoSomething();
}

// Indentation: 4 spaces (no tabs)
// Line length: Keep under 120 characters when practical

// Using directives at top, grouped:
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
```

### Import Guidelines

```csharp
// Always include:
using UnityEngine;

// Use as needed:
using System.Collections;           // Coroutines
using System.Collections.Generic;    // Lists, dictionaries
using UnityEngine.UI;                // UI components
using UnityEngine.EventSystems;      // UI events

// Avoid unnecessary imports
```

### Error Handling

- **Return null/bool** instead of throwing exceptions for expected failures
- **Use Debug.Log()** for diagnostic output (existing code has commented debug logs)
- **Check null explicitly** with `== null` rather than `?.` operator (matches existing code style)

### Unity-Specific Patterns

```csharp
// Coroutines for async operations:
private IEnumerator GenerateChunk()
{
    // ...
    yield return null;
}

// Awake for initialization:
private void Awake()
{
    instance = this;
    rigidbody = GetComponent<Rigidbody>();
}

// Update for per-frame logic:
private void Update()
{
    CheckMove();
}
```

## Block System

### BlockType Enum (Assets/Scripts/World/Block.cs:3-17)

```csharp
public enum BlockType
{
    Air,
    Dirt,
    Grass,
    Stone,
    Wood,
    Leaves,
    Cobblestone,
    Planks,
    Stick,
    CraftingTable,
    Coal,
    Torch
}
```

### BlockFaceType Enum (Assets/Scripts/World/Block.cs:29-45)

Maps to texture UV coordinates for block faces.

## Common Patterns

### Singleton Pattern
```csharp
public class World : MonoBehaviour
{
    public static World instance;
    private void Awake() { instance = this; }
}
```

### Block Mesh Generation
```csharp
Block.CreateMesh(blockType, side, transform, offset, size);
Block.CombineMeshes(gameObject, material);
```

### Raycast for Block Interaction
```csharp
Ray ray = camera.ScreenPointToRay(Input.mousePosition);
if (Physics.Raycast(ray, out hit, 5f, LayerMask.GetMask("Chunk")))
{
    // Handle block hit
}
```

### Inventory Drag & Drop
```csharp
EventTrigger.Entry entry = new EventTrigger.Entry();
entry.eventID = EventTriggerType.PointerDown;
entry.callback.AddListener((eventData) => { /* handler */ });
```

## Crafting System

### RecipeManager (Assets/Scripts/UI/RecipeManager.cs)
- Singleton pattern for global access via `RecipeManager.instance`
- `FindRecipe(string[] inputItems, int[] inputAmounts)` finds matching recipe
- Currently hardcoded recipes in `InitializeRecipes()`, planned to be replaced with Luban config

### CraftingRecipe (Assets/Scripts/UI/CraftingRecipe.cs)
- `inputItems`: 4-element array for 2x2 crafting grid
- `inputAmounts`: corresponding item amounts
- `Matches()` checks if input matches recipe

### Current Recipes
- Wood (any position) → 4 Planks

### Inventory Integration (Assets/Scripts/UI/Inventory.cs)
- 4 crafting input slots: `craftingInputSlots`
- 1 crafting output slot: `craftingOutputSlot`
- `CheckCraftingInput()` monitors slot changes
- `OnCraftingOutputClick()` triggered when clicking output
- `Craft()` consumes inputs and produces output

## Important Notes

1. **Layer "Chunk"** - Blocks use layer "Chunk" for raycasting
2. **Layer "Player"** - Player collision layer for block placement checks
3. **Resources.Load()** - Item sprites loaded from `Resources/Image/Imgs/Block/`
4. **Perlin Noise** - Terrain generation uses `Mathf.PerlinNoise()` with seed
5. **Chunk size** - 16x48x16 blocks per chunk

## Testing New Code

When adding new features:
1. Test block placement/destruction manually in Play mode
2. Test inventory UI interactions including crafting
3. Test chunk loading/unloading at chunk boundaries
4. Verify no null reference errors in console
