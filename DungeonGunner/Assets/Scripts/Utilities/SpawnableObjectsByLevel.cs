using System;
using System.Collections.Generic;

[Serializable]
public class  SpawnableObjectsByLevel<T>
{
    public DungeonLevelSO dungeonLevelSO;
    public List<SpawnableObjectRatio<T>> spawnableObjectsRatioList;
}
