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

    public CraftingRecipe(string result, int amount, string[] inputs)
    {
        this.resultItem = result;
        this.resultAmount = amount;
        this.inputItems = inputs;
    }

    /// <summary>
    /// 检查输入是否匹配此配方
    /// </summary>
    public bool Matches(string[] inputItems)
    {
        if (inputItems == null ) return false;

        for (int i = 0; i < 4; i++)
        {
            string input = i < inputItems.Length ? inputItems[i] : null;

            string required = i < this.inputItems.Length ? this.inputItems[i] : null;

            if (required == null)
            {
                // 配方该位置为空，但输入有物品，不匹配
                if (input != null) return false;
            }
            else
            {
                // 配方该位置需要物品
                if (input == null) return false;
                if (input != required) return false;
            }
        }
        return true;
    }
}
