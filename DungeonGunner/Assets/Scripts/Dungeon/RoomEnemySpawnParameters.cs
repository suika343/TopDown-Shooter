using UnityEngine;
using System;

[Serializable]
public class RoomEnemySpawnParameters
{
    public DungeonLevelSO dungeonLevel;
    public int minTotalEnemiesToSpawn;
    public int maxTotalEnemiesToSpawn;
    public int minConcurrentEnemies;
    public int maxConcurrentEnemies;
    public float minimumSpawnInterval;
    public float maximumSpawnInterval;
}
