using System.Collections.Generic;
using UnityEngine;

public class SpawnTest : MonoBehaviour
{
    public RoomTemplateSO roomTemplateSO;
    private List<SpawnableObjectsByLevel<EnemyDetailsSO>>testLevelSpawnList;
    private RandomSpawnableObject<EnemyDetailsSO> randomEnemyHelperClass;
    private GameObject instantiateEnemy;

    private void Update()
    {
        testLevelSpawnList = roomTemplateSO.enemiesByLevelList;

        randomEnemyHelperClass = new RandomSpawnableObject<EnemyDetailsSO>(testLevelSpawnList);

        if (Input.GetKeyDown(KeyCode.T))
        {
            if(instantiateEnemy != null)
            {
                Destroy(instantiateEnemy);
            }

            EnemyDetailsSO enemyDetails = randomEnemyHelperClass.GetItem();

            if(enemyDetails != null)
            {
                instantiateEnemy = Instantiate(enemyDetails.enemyPrefab, 
                    HelperUtilities.GetSpawnPointNearestToPlayer(HelperUtilities.GetMouseWorldPosition()), 
                    Quaternion.identity);
            }
        }
    }

}
