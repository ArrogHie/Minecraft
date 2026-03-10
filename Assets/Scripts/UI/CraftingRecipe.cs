using UnityEngine;

/// <summary>
/// 合成配方数据类
/// </summary>
[System.Serializable]
public class CraftingRecipe
{
    public string resultItem;       // 产物名称
    public int resultAmount = 1;    // 产物数量
    public string[] inputItems;     // 输入物品名称 (4个，对应2x2)
    public int[] inputAmounts;     // 输入物品数量

    public CraftingRecipe(string result, int amount, string[] inputs, int[] inputAmounts)
    {
        this.resultItem = result;
        this.resultAmount = amount;
        this.inputItems = inputs;
        this.inputAmounts = inputAmounts;
    }

    /// <summary>
    /// 检查输入是否匹配此配方
    /// </summary>
    public bool Matches(string[] inputItems, int[] inputAmounts)
    {
        if (inputItems == null || inputAmounts == null) return false;

        for (int i = 0; i < 4; i++)
        {
            string input = i < inputItems.Length ? inputItems[i] : null;
            int amount = i < inputAmounts.Length ? inputAmounts[i] : 0;

            string required = i < this.inputItems.Length ? this.inputItems[i] : null;
            int requiredAmount = i < this.inputAmounts.Length ? this.inputAmounts[i] : 0;

            if (required == null)
            {
                // 配方该位置为空，但输入有物品，不匹配
                if (input != null && amount > 0) return false;
            }
            else
            {
                // 配方该位置需要物品
                if (input == null || amount < requiredAmount) return false;
                if (input != required) return false;
            }
        }
        return true;
    }
}
