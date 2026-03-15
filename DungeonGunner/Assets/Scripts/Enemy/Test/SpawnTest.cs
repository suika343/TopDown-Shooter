using System.Collections.Generic;
using UnityEngine;

public class SpawnTest : MonoBehaviour
{
    private List<SpawnableObjectsByLevel<EnemyDetailsSO>>testLevelSpawnList;
    private RandomSpawnableObject<EnemyDetailsSO> randomEnemyHelperClass;
    private List<GameObject> instantiatedEnemyList = new List<GameObject>();

    private void OnEnable()
    {
        StaticEventHandler.OnRoomChanged += StaticEventHandler_OnRoomChanged;
    }

    private void OnDisable()
    {
        StaticEventHandler.OnRoomChanged -= StaticEventHandler_OnRoomChanged;
    }

    private void StaticEventHandler_OnRoomChanged(RoomChangedEventArgs roomChangedEventArgs)
    {
        //destroy instantiated enemies when room changes
        if(instantiatedEnemyList.Count > 0 && instantiatedEnemyList != null)
        {
            foreach(GameObject enemy in instantiatedEnemyList)
            {
                Destroy(enemy);
            }
        }

        RoomTemplateSO roomTemplate = DungeonBuilder.Instance.GetRoomTemplate(roomChangedEventArgs.room.templateID);

        if(roomTemplate != null)
        {
            testLevelSpawnList = roomTemplate.enemiesByLevelList;
            randomEnemyHelperClass = new RandomSpawnableObject<EnemyDetailsSO>(testLevelSpawnList);
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.T))
        {
            EnemyDetailsSO enemyDetails = randomEnemyHelperClass.GetItem();

            if(enemyDetails != null)
            {
                instantiatedEnemyList.Add(Instantiate(enemyDetails.enemyPrefab, 
                    HelperUtilities.GetSpawnPointNearestToPlayer(HelperUtilities.GetMouseWorldPosition()), 
                    Quaternion.identity));
            }
        }
    }

}
