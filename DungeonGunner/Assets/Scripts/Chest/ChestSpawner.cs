using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UIElements;
using UnityEngine.InputSystem.Composites;

public class ChestSpawner : MonoBehaviour
{
    [System.Serializable]
    private struct RangeByLevel
    {
        public DungeonLevelSO dungeonLevel;
        [Range(0, 100)] public int min;
        [Range(0, 100)] public int max;
    }

    #region HEADER CHEST PREFAB
    [Space(10)]
    [Header("CHEST PREFAB")]
    #endregion HEADER CHEST PREFAB
    [SerializeField] private GameObject chestPrefab;

    #region HEADER CHEST SPAWN CHANCE
    [Space(10)]
    [Header("CHEST SPAWN CHANCE")]
    #endregion HEADER CHEST SPAWN CHANCE
    [SerializeField] [Range(0, 100)] private int chestSpawnChanceMin;
    [SerializeField] [Range(0, 100)] private int chestSpawnChanceMax;
    [SerializeField] private List<RangeByLevel> chestSpawnChanceByLevelList;

    #region HEADER CHEST SPAWN DETAILS
    [Space(10)]
    [Header("CHEST SPAWN DETAILS")]
    #endregion HEADER CHEST SPAWN DETAILS
    [SerializeField] private ChestSpawnEvent chestSpawnEvent;
    [SerializeField] private ChestSpawnPosition chestSpawnPosition;
    [SerializeField][Range(0, 3)] private int numberOfItemsToSpawnMin; 
    [SerializeField][Range(0, 3)] private int numberOfItemsToSpawnMax;

    #region HEADER CHEST CONTENT DETAILS
    [Space(10)]
    [Header("CHEST CONTENT DETAILS")]
    #endregion HEADER CHEST CONTENT DETAILS
    #region TOOLTIP
    [Tooltip("The weapon to spawn for each dungeon level and their respective spawn chances.")]
    #endregion TOOLTIP
    [SerializeField] private List<SpawnableObjectsByLevel<WeaponDetailsSO>> weaponSpawnByLevelList;
    #region TOOLTIP
    [Tooltip("The range of health item to spawn for each level")]
    #endregion TOOLTIP
    [SerializeField] private List<RangeByLevel> healthItemSpawnByLevelList;
    #region TOOLTIP
    [Tooltip("The range of ammo item to spawn for each level")]
    #endregion TOOLTIP
    [SerializeField] private List<RangeByLevel> ammoItemSpawnByLevelList;

    private bool chestSpawned = false;
    private Room chestRoom;

    private void OnEnable()
    {
        StaticEventHandler.OnRoomChanged += StaticEventHandler_OnRoomChanged;

        StaticEventHandler.OnRoomEnemiesDefeated += StaticEventHandler_OnRoomEnemiesDefeated;
    }

    private void OnDisable()
    {
        StaticEventHandler.OnRoomChanged -= StaticEventHandler_OnRoomChanged;

        StaticEventHandler.OnRoomEnemiesDefeated -= StaticEventHandler_OnRoomEnemiesDefeated;
    }

    private void StaticEventHandler_OnRoomChanged(RoomChangedEventArgs roomChangedEventArgs)
    {
        //get the room the chest is in if it is not already set
        if (chestRoom == null)
        {
            chestRoom = GetComponentInParent<InstantiatedRoom>().room;
        }

        //if the chest is spawned on room entry, then spawn chest when entering the room
        if(chestSpawnEvent == ChestSpawnEvent.onRoomEntry && !chestSpawned && chestRoom == roomChangedEventArgs.room)
        {
            SpawnChest();
        }
    }

    private void StaticEventHandler_OnRoomEnemiesDefeated(RoomEnemiesDefeatedEventArgs roomEnemiesDefeatedEventArgs)
    {
        //get the room the chest is in if it is not already set
        if (chestRoom == null)
        {
            chestRoom = GetComponentInParent<InstantiatedRoom>().room;
        }
        //if the chest is spawned on enemies defeated, then spawn chest when all enemies are defeated in the room
        if(chestSpawnEvent == ChestSpawnEvent.onEnemiesDefeated && !chestSpawned && chestRoom == roomEnemiesDefeatedEventArgs.room)
        {
            SpawnChest();
        }
    }

    private void SpawnChest()
    {
        chestSpawned = true;

        //check if chest spawn chance is successful, if not, return
        if (!RandomSpawnChest()) return;

        // Get Number of Health, Ammo, and Weapon Items to Spawn (max of 1 each)
        GetItemsToSpawn(out int ammoNum, out int healthNum, out int weaponNum);

        //Instantiate the chest
        GameObject chestGameObject = Instantiate(chestPrefab, this.transform);

        //position the chest based on the spawn position setting
        if(chestSpawnPosition == ChestSpawnPosition.atSpawnPosition)
        {
            chestGameObject.transform.position = this.transform.position;
        }
        else if(chestSpawnPosition == ChestSpawnPosition.atPlayerPosition)
        {
            //Get nearest spawn position to player
            Vector3 spawnPosition = HelperUtilities.GetSpawnPointNearestToPlayer(GameManager.Instance.GetPlayer().transform.position);

            // Calculate random offset
            Vector3 offset = new Vector3(Random.Range(-0.5f, 0.5f), Random.Range(-0.5f, 0.5f), 0);

            chestGameObject.transform.position = spawnPosition + offset;
        }

        //get chest component
        Chest chest = chestGameObject.GetComponent<Chest>();

        //Initialize chest
        if(chestSpawnEvent == ChestSpawnEvent.onRoomEntry)
        {
            //dont use materialize effect when chest spawns on room entry, as it will be hidden behind the room transition effect
            chest.Initialize(false, GetHealthPercentToSpawn(healthNum), GetWeaponDetailsToSpawn(weaponNum), GetAmmoPercentToSpawn(ammoNum));
        }
        else if(chestSpawnEvent == ChestSpawnEvent.onEnemiesDefeated)
        {
            //use materialize effect when chest spawns on enemies defeated, as it will be visible to the player
            chest.Initialize(true, GetHealthPercentToSpawn(healthNum), GetWeaponDetailsToSpawn(weaponNum), GetAmmoPercentToSpawn(ammoNum));
        }
    }

    private bool RandomSpawnChest()
    {
        int chancePercent = Random.Range(chestSpawnChanceMin, chestSpawnChanceMax + 1);

        //Check if there is an override chance percent for the current dungeon level
        foreach (RangeByLevel rangeByLevel in chestSpawnChanceByLevelList)
        {
            if (GameManager.Instance.GetCurrentDungeonLevel() == rangeByLevel.dungeonLevel)
            {
                chancePercent = Random.Range(rangeByLevel.min, rangeByLevel.max + 1);
                break;
            }
        }

        //get random value between 1 and 100
        int randomPercent = Random.Range(1, 101);

        if (randomPercent <= chancePercent)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    private void GetItemsToSpawn(out int ammoNum, out int healthNum, out int weaponNum)
    {
        //get random number of items to spawn
        int numberOfItemsToSpawn = Random.Range(numberOfItemsToSpawnMin, numberOfItemsToSpawnMax + 1);
        //initialize item counts
        ammoNum = 0;
        healthNum = 0;
        weaponNum = 0;

        //to determine which of the three items to spawn
        int choice;

        if(numberOfItemsToSpawn == 1)
        {
            choice = Random.Range(0,3);
            if (choice == 0)
            {
                weaponNum++;
                return;
            }
            else if (choice == 1)
            {
                healthNum++;
                return;
            }
            else if (choice == 2)
            {
                ammoNum++;
                return;
            }
        }
        else if(numberOfItemsToSpawn == 2)
        {
            choice = Random.Range(0, 3);
            if (choice == 0)
            {
                weaponNum++;
                ammoNum++;
                return;
            }
            else if (choice == 1)
            {
                ammoNum++;
                healthNum++;
                return;
            }
            else if (choice == 2)
            {
                ammoNum++;
                weaponNum++;
                return;
            }
        }
        else if(numberOfItemsToSpawn == 3)
        {
            ammoNum++;
            healthNum++;
            weaponNum++;
            return;
        }
    }

   private int GetAmmoPercentToSpawn(int ammoNumber)
   {
        if(ammoNumber == 0) return 0;

        foreach(RangeByLevel rangeByLevel in ammoItemSpawnByLevelList)
        {
            if (GameManager.Instance.GetCurrentDungeonLevel() == rangeByLevel.dungeonLevel)
            {
                return Random.Range(rangeByLevel.min, rangeByLevel.max);
            }
        }

        return 0;
    }
    private int GetHealthPercentToSpawn(int healthNumber)
    {
        if (healthNumber == 0) return 0;

        foreach (RangeByLevel rangeByLevel in healthItemSpawnByLevelList)
        {
            if (GameManager.Instance.GetCurrentDungeonLevel() == rangeByLevel.dungeonLevel)
            {
                return Random.Range(rangeByLevel.min, rangeByLevel.max);
            }
        }

        return 0;
    }

    private WeaponDetailsSO GetWeaponDetailsToSpawn(int weaponNumber)
    {
        if (weaponNumber == 0) return null;

        //Create an instance of the class used to select a random item from the list based on the
        // relative spawn chances of each item
        RandomSpawnableObject<WeaponDetailsSO> randomWeapon = new RandomSpawnableObject<WeaponDetailsSO>(weaponSpawnByLevelList);

        WeaponDetailsSO weaponDetails = randomWeapon.GetItem();

        return weaponDetails;
    }

    #region VALIDATION
#if UNITY_EDITOR
    private void OnValidate()
    {
        HelperUtilities.ValidateCheckNullValue(this, nameof(chestPrefab), chestPrefab);
        HelperUtilities.ValidateCheckPositiveRange(this, nameof(chestSpawnChanceMin), 
            chestSpawnChanceMin, nameof(chestSpawnChanceMax), chestSpawnChanceMax, true);

        if(chestSpawnChanceByLevelList != null && chestSpawnChanceByLevelList.Count > 0)
        {
            HelperUtilities.ValidateCheckEnumerableValues(this, nameof(chestSpawnChanceByLevelList), chestSpawnChanceByLevelList);
            foreach (RangeByLevel rangeByLevel in chestSpawnChanceByLevelList)
            {
                HelperUtilities.ValidateCheckNullValue(this, nameof(rangeByLevel.dungeonLevel), rangeByLevel.dungeonLevel);
                HelperUtilities.ValidateCheckPositiveRange(this, nameof(rangeByLevel.min), 
                    rangeByLevel.min, nameof(rangeByLevel.max), rangeByLevel.max, true);
            }
        }

        HelperUtilities.ValidateCheckPositiveRange(this, nameof(numberOfItemsToSpawnMin),
            numberOfItemsToSpawnMin, nameof(numberOfItemsToSpawnMax), numberOfItemsToSpawnMax, true);
        
        if(weaponSpawnByLevelList != null && weaponSpawnByLevelList.Count > 0)
        {
            foreach (SpawnableObjectsByLevel<WeaponDetailsSO> spawnableObjectsByLevel in weaponSpawnByLevelList)
            {
                HelperUtilities.ValidateCheckNullValue(this, nameof(spawnableObjectsByLevel.dungeonLevel), spawnableObjectsByLevel.dungeonLevel);
                
                foreach(SpawnableObjectRatio<WeaponDetailsSO> spawnableObjectRatio in spawnableObjectsByLevel.spawnableObjectsRatioList)
                {
                    HelperUtilities.ValidateCheckNullValue(this, nameof(spawnableObjectRatio.dungeonObject), spawnableObjectRatio.dungeonObject);
                    HelperUtilities.ValidateCheckPositiveValue(this, nameof(spawnableObjectRatio.ratio), spawnableObjectRatio.ratio, true);
                }
            }
        }

        if(healthItemSpawnByLevelList != null && healthItemSpawnByLevelList.Count > 0)
        {
            HelperUtilities.ValidateCheckEnumerableValues(this, nameof(healthItemSpawnByLevelList), healthItemSpawnByLevelList);

            foreach (RangeByLevel rangeByLevel in healthItemSpawnByLevelList)
            {
                HelperUtilities.ValidateCheckNullValue(this, nameof(rangeByLevel.dungeonLevel), rangeByLevel.dungeonLevel);
                HelperUtilities.ValidateCheckPositiveRange(this, nameof(rangeByLevel.min), 
                    rangeByLevel.min, nameof(rangeByLevel.max), rangeByLevel.max, true);
            }
        }

        if(ammoItemSpawnByLevelList != null && ammoItemSpawnByLevelList.Count > 0)
        {
            HelperUtilities.ValidateCheckEnumerableValues(this, nameof(ammoItemSpawnByLevelList), ammoItemSpawnByLevelList);
            foreach (RangeByLevel rangeByLevel in ammoItemSpawnByLevelList)
            {
                HelperUtilities.ValidateCheckNullValue(this, nameof(rangeByLevel.dungeonLevel), rangeByLevel.dungeonLevel);
                HelperUtilities.ValidateCheckPositiveRange(this, nameof(rangeByLevel.min), 
                    rangeByLevel.min, nameof(rangeByLevel.max), rangeByLevel.max, true);
            }
        }
    }
#endif
    #endregion
}
