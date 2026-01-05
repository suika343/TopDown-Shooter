using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEditor.PackageManager.Requests;

[DisallowMultipleComponent]
public class PoolManager : SingletonMonobehaviour<PoolManager>
{
    #region
    [Tooltip("Populate this array with prefabs that you want to add to the pool, and specify the number of gameObjects to be created for each")]
    #endregion
    [SerializeField] private Pool[] poolArray;
    private Transform objectPoolTransform;
    private Dictionary<int, Queue<Component>> poolDictionary = new Dictionary<int, Queue<Component>>();

    [System.Serializable]
    public struct Pool
    {
        public int poolSize;
        public GameObject prefab;
        public string componentType;
    }

    private void Start()
    {
        //set this singleton pool manager as parent of all pooled objects
        objectPoolTransform = gameObject.transform; 

        for(int i = 0; i < poolArray.Length; i++)
        {
            CreatePool(poolArray[i].prefab, poolArray[i].poolSize, poolArray[i].componentType);
        }
    }
    /// <summary>
    /// Create pool with specified prefab and pool size
    /// </summary>
    /// <param name="prefab"></param>
    /// <param name="poolSize"></param>
    /// <param name="componentType"></param>
    private void CreatePool(GameObject prefab, int poolSize, string componentType)
    {
        int poolKey = prefab.GetInstanceID();
        string prefabName = prefab.name; // get prefab name

        GameObject parentGameObject = new GameObject(prefabName + "Anchor"); // create parent GameObject with prefab name to parent child ojects to

        parentGameObject.transform.SetParent(objectPoolTransform); // set parent to PoolManager

        if(!poolDictionary.ContainsKey(poolKey))
        {
            poolDictionary.Add(poolKey, new Queue<Component>());
            for(int i = 0; i < poolSize; i++)
            {
                GameObject newGameObject = Instantiate(prefab, parentGameObject.transform) as GameObject; // instantiate new game object from prefab
                newGameObject.SetActive(false); // deactivate the new game object
                poolDictionary[poolKey].Enqueue(newGameObject.GetComponent(Type.GetType(componentType))); // add the new game object to the pool queue
            }
        }
    }
    /// <summary>
    /// Reuse component from pool. 'Prefab' is the prefab gameobject containing the component.
    /// 'position' is the world position for the gameObject to appear at when it is enabled.
    /// 'rotation' should be set if the gameObject requires a specific rotation when enabled.
    /// </summary>
    /// <param name="prefab"></param>
    /// <param name="position"></param>
    /// <param name="rotation"></param>
    /// <returns></returns>
    public Component ReuseComponent(GameObject prefab, Vector3 position, Quaternion rotation)
    {
        int poolKey = prefab.GetInstanceID();

        if (poolDictionary.ContainsKey(poolKey))
        {
            // Get Object from pool Queue
            Component componentToReuse = GetComponentFromPool(poolKey);

            ResetObject(position, rotation, componentToReuse, prefab);

            return componentToReuse;
        }
        else
        {
            Debug.Log("No Object pool for " + prefab);
            return null;
        }
    }

    private Component GetComponentFromPool(int poolKey)
    {
        Component componentToReuse = poolDictionary[poolKey].Dequeue();
        // Re-enqueue the component back to the pool
        poolDictionary[poolKey].Enqueue(componentToReuse);
        if(componentToReuse.gameObject.activeSelf == true)
        {
            componentToReuse.gameObject.SetActive(false);
        }
        return componentToReuse;
    }

    private void ResetObject(Vector3 position, Quaternion rotation, Component componentToReuse, GameObject prefab)
    {
        // Reset position, rotation, and scale
        componentToReuse.transform.position = position;
        componentToReuse.transform.rotation = rotation;
        componentToReuse.transform.localScale = prefab.transform.localScale;
    }


    #region VALIDATION

#if UNITY_EDITOR
    private void OnValidate()
    {
        HelperUtilities.ValidateCheckEnumerableValues(this, nameof(poolArray), poolArray);
    }
#endif
    #endregion
}
