# Unity Minecraft Remake

This is a project dedicated to recreating **Minecraft** using the **Unity** engine. All scripts are located in the `Assets/Scripts` directory.

---

## Script Descriptions

### 1. Player
* **PlayerControl**: Handles player movement, input, and general player-related logic.
* **PickupTrigger**: Manages the collision boxes and logic for picking up dropped items.

### 2. World Generation
* **World.cs**: Oversees global world generation.
* **Chunk.cs**: Manages the generation of individual chunks and chunk-based interactions.
* **Block.cs**: Defines the `Block` class and handles block-specific rendering.

### 3. DroppedItem
* **DroppedItem**: Handles the visual rendering and physics/movement logic of items on the ground.
* **ItemTrigger**: Manages the logic for item collection and movement toward the player.

### 4. UI
* **Inventory (InventoryItem, Inventory)**: Manages inventory items and the storage grid system.
* **Hotbar**: Handles the quick-access item bar (Hotbar) on the HUD.