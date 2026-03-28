using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 配方管理器 - 之后将由Luban配置替代
/// </summary>
public class RecipeManager : MonoBehaviour
{
    public static RecipeManager instance;

    public List<CraftingRecipe> recipes = new List<CraftingRecipe>();

    private void Awake()
    {
        instance = this;
        InitializeRecipes();
    }

    private void InitializeRecipes()
    {
        // 原木 -> 木板 (1个原木在任意位置 -> 4个木板)
        string[] inputs1 = new string[] { "Wood", null, null, null };
        recipes.Add(new CraftingRecipe("Planks", 4, inputs1));

        string[] inputs2 = new string[] { null, "Wood", null, null };
        recipes.Add(new CraftingRecipe("Planks", 4, inputs2));

        string[] inputs3 = new string[] { null, null, "Wood", null };
        recipes.Add(new CraftingRecipe("Planks", 4, inputs3));

        string[] inputs4 = new string[] { null, null, null, "Wood" };
        recipes.Add(new CraftingRecipe("Planks", 4, inputs4));

        string[] inputs5 = new string[] { "Planks", "Planks", "Planks", "Planks" };
        recipes.Add(new CraftingRecipe("CraftingTable", 1, inputs5));
    }

    /// <summary>
    /// 查找匹配的配方
    /// </summary>
    public CraftingRecipe FindRecipe(string[] inputItems)
    {
        foreach (CraftingRecipe recipe in recipes)
        {
            if (recipe.Matches(inputItems))
            {
                return recipe;
            }
        }
        return null;
    }
}
