using System.Collections.Generic;
using System;

[Serializable]
public class ItemInstance
{
    public ItemData baseData;
    public ItemStat mainStat;
    public List<ItemStat> subStats;
}
