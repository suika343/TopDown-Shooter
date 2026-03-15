using UnityEngine;
using System.Collections.Generic;

public class RandomSpawnableObject<T>
{
    public struct ChanceBoundaries
    {
        public T spawnableObject;
        public int lowBoundaryValue;
        public int highBoundaryValue;

    }

    private int ratioValueTotal = 0;
    private List<ChanceBoundaries> chanceBoundariesList = new List<ChanceBoundaries>();
    private List<SpawnableObjectsByLevel<T>> spawnableObjectsByLevelList;

    public RandomSpawnableObject(List<SpawnableObjectsByLevel<T>> spawnableObjectsByLevelList)
    {
        this.spawnableObjectsByLevelList = spawnableObjectsByLevelList;
    }

    public T GetItem()
    {
        int upperBoundary = -1;
        ratioValueTotal = 0;
        chanceBoundariesList.Clear();
        T spawnableObject = default(T);

        foreach (SpawnableObjectsByLevel<T> spawnableObjectByLevel in spawnableObjectsByLevelList)
        {
            //check if current level is the same as the one in the list
            if (spawnableObjectByLevel.dungeonLevel == GameManager.Instance.GetDungeonLevel())
            {
                foreach (SpawnableObjectRatio<T> spawnableObjectRatio in spawnableObjectByLevel.spawnableObjectsRatioList)
                {

                    int lowerBoundary = upperBoundary + 1;
                    upperBoundary = lowerBoundary + spawnableObjectRatio.ratio - 1;

                    ratioValueTotal += spawnableObjectRatio.ratio;

                    //add object to list
                    chanceBoundariesList.Add(new ChanceBoundaries()
                    {
                        spawnableObject = spawnableObjectRatio.dungeonObject,
                        lowBoundaryValue = lowerBoundary,
                        highBoundaryValue = upperBoundary
                    });
                }
            }
        }

        //if there are no objects to spawn, return default value of T
        if (chanceBoundariesList.Count == 0)
        {
            return spawnableObject;
        }

        //randomly get a value between 0 and the total ratio value, then check which object corresponds to that value based on the boundaries
        int lookupValue = Random.Range(0, ratioValueTotal);

        foreach (ChanceBoundaries chanceBoundaries in chanceBoundariesList)
        {
            if (lookupValue >= chanceBoundaries.lowBoundaryValue && lookupValue <= chanceBoundaries.highBoundaryValue)
            {
                spawnableObject = chanceBoundaries.spawnableObject;
                break;
            }
        }

        return spawnableObject;
    }
}
