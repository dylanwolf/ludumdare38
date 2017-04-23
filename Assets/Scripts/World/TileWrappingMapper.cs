using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TileWrappingMapper  {

    Dictionary<int, int> OldXToNewX = new Dictionary<int, int>();
    Dictionary<int, int> NewXToOldX = new Dictionary<int, int>();
    Dictionary<int, int> OldYToNewY = new Dictionary<int, int>();
    Dictionary<int, int> NewYToOldY = new Dictionary<int, int>();

    public void Clear()
    {
        OldXToNewX.Clear();
        OldYToNewY.Clear();
        NewXToOldX.Clear();
        NewYToOldY.Clear();
    }

    public void MapX(int original, int newX)
    {
        OldXToNewX[original] = newX;
        NewXToOldX[newX] = original;
    }

    public void MapY(int original, int newY)
    {
        OldYToNewY[original] = newY;
        NewYToOldY[newY] = original;
    }

    int GetValue(Dictionary<int, int> mapping, int key)
    {
        if (mapping.ContainsKey(key))
            return mapping[key];

        if (key < mapping.Keys.Min())
            key = mapping.Keys.Min();

        if (key > mapping.Keys.Max())
            key = mapping.Keys.Max();

        return mapping[key];
    }

    public int GetNewX(int x)
    {
        return GetValue(OldXToNewX, x);
    }

    public int GetOldX(int x)
    {
        return GetValue(OldXToNewX, x);
    }

    public int GetNewY(int y)
    {
        return GetValue(OldYToNewY, y);
    }

    public int GetOldY(int y)
    {
        return GetValue(OldYToNewY, y);
    }
}
