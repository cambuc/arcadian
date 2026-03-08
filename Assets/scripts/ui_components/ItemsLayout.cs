using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ItemsLayout : MonoBehaviour
{
    public Vector2Int dimensions;
    public Vector2 cellSize;

    public void AdjustLayout(List<PlayerInventoryUIItem> items)
    {
        items = items.OrderByDescending(r => r.item.Volume()).ToList();
        PlayerInventoryUIItem[,] grid = FillGrid(items);

        List<PlayerInventoryUIItem> assigned = new List<PlayerInventoryUIItem>();
        for (int y = 0; y < grid.GetLength(1); y++)
            for (int x = 0; x < grid.GetLength(0); x++)
            {
                if (grid[x, y] == null || assigned.Contains(grid[x, y])) continue;

                RectTransform itemRect = grid[x, y].GetComponent<RectTransform>();
                itemRect.anchoredPosition = GridToPos(x, y) + new Vector2(itemRect.rect.width / 2, -itemRect.rect.height / 2);
                assigned.Add(grid[x, y]);
            }
    }

    Vector2 GridToPos(int x, int y)
    {
        RectTransform rect = GetComponent<RectTransform>();
        return new Vector2(rect.rect.xMin + x * cellSize.x, rect.rect.yMax - y * cellSize.y);
    }

    PlayerInventoryUIItem[,] FillGrid(List<PlayerInventoryUIItem> items)
    {
        PlayerInventoryUIItem[,] grid = new PlayerInventoryUIItem[dimensions.x, dimensions.y];
        foreach (PlayerInventoryUIItem item in items)
        {
            bool foundPos = false;
            Vector2Int index = Vector2Int.zero;
            for (int y = 0; y < grid.GetLength(1); y++)
            {
                for (int x = 0; x < grid.GetLength(0); x++)
                {
                    if (grid[x, y] != null) continue;

                    if (x + item.item.dimensions.x >= grid.GetLength(0)) continue;

                    bool filled = false;
                    for (int xCheck = x; xCheck < x + item.item.dimensions.x; xCheck++)
                    {
                        for (int yCheck = y; yCheck < y + item.item.dimensions.y; yCheck++)
                        {
                            if (grid[xCheck, yCheck] != null)
                            {
                                filled = true;
                                break;
                            }
                        }
                        if (filled) break;
                    }
                    if (filled) continue;

                    index = new Vector2Int(x, y);
                    foundPos = true;
                    break;
                }
                if (foundPos) break;
            }

            for (int x = index.x; x < index.x + item.item.dimensions.x; x++)
                for (int y = index.y; y < index.y + item.item.dimensions.y; y++)
                    grid[x, y] = item;
        }
        return grid;
    }
}
