using System;
using System.Collections.Generic;

[Serializable]
public class  SpawnableObjectsByLevel<T>
{
    public DungeonLevelSO dungeonLevel;
    public List<SpawnableObjectRatio<T>> spawnableObjectsRatioList;
}
