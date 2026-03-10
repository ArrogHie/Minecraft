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
        int[] amounts1 = new int[] { 1, 0, 0, 0 };
        recipes.Add(new CraftingRecipe("Planks", 4, inputs1, amounts1));

        string[] inputs2 = new string[] { null, "Wood", null, null };
        int[] amounts2 = new int[] { 0, 1, 0, 0 };
        recipes.Add(new CraftingRecipe("Planks", 4, inputs2, amounts2));

        string[] inputs3 = new string[] { null, null, "Wood", null };
        int[] amounts3 = new int[] { 0, 0, 1, 0 };
        recipes.Add(new CraftingRecipe("Planks", 4, inputs3, amounts3));

        string[] inputs4 = new string[] { null, null, null, "Wood" };
        int[] amounts4 = new int[] { 0, 0, 0, 1 };
        recipes.Add(new CraftingRecipe("Planks", 4, inputs4, amounts4));
    }

    /// <summary>
    /// 查找匹配的配方
    /// </summary>
    public CraftingRecipe FindRecipe(string[] inputItems, int[] inputAmounts)
    {
        foreach (CraftingRecipe recipe in recipes)
        {
            if (recipe.Matches(inputItems, inputAmounts))
            {
                return recipe;
            }
        }
        return null;
    }
}
