using System.Collections.Generic;
using UnityEngine;

public class EffectManager : Singleton<EffectManager>
{
    static public GameObject GetNextObjectWithIndex(int _index, bool activateObject = true)
    {
        if (Instance.m_fxPrefabs.Length <= _index)
        {
            Debug.LogError("EffectManager.GetNextObject: out of range");
            return null;
        }

        return GetNextObject(Instance.m_fxPrefabs[_index], activateObject);
    }

    static public GameObject GetNextObjectWithName(string pName, bool activateObject = true)
    {
        int hash = pName.GetHashCode();
        if (Instance.poolNameHash.ContainsKey(hash))
        {
            int idx = Instance.poolNameHash[hash];

            return GetNextObjectWithIndex(idx, activateObject);
        }
        return null;
    }

    /// <summary>
    /// Get the next available preloaded Object.
    /// </summary>
    /// <returns>
    /// The next available preloaded Object.
    /// </returns>
    /// <param name='sourceObj'>
    /// The source Object from which to get a preloaded copy.
    /// </param>
    /// <param name='activateObject'>
    /// Activates the object before returning it.
    /// </param>
    static public GameObject GetNextObject(GameObject sourceObj, bool activateObject = true)
    {
        int uniqueId = sourceObj.GetInstanceID();

        if (!Instance.poolCursors.ContainsKey(uniqueId))
        {
            Debug.LogError("[EffectManager.GetNextObject()] Object hasn't been preloaded: " + sourceObj.name + " (ID:" + uniqueId + ")");
            return null;
        }

        int cursor = Instance.poolCursors[uniqueId];
        Instance.poolCursors[uniqueId]++;
        if (Instance.poolCursors[uniqueId] >= Instance.instantiatedObjects[uniqueId].Count)
        {
            Instance.poolCursors[uniqueId] = 0;
        }

        GameObject returnObj = Instance.instantiatedObjects[uniqueId][cursor];
        if (activateObject)
        {
#if UNITY_3_5
            returnObj.SetActiveRecursively(true);
#else
            returnObj.SetActive(true);
#endif
        }

        return returnObj;
    }

    /// <summary>
    /// Preloads an object a number of times in the pool.
    /// </summary>
    /// <param name='sourceObj'>
    /// The source Object.
    /// </param>
    /// <param name='poolSize'>
    /// The number of times it will be instantiated in the pool (i.e. the max number of same object that would appear simultaneously in your Scene).
    /// </param>
    static public void PreloadObject(GameObject sourceObj, int poolSize = 1)
    {
        Instance.addObjectToPool(sourceObj, poolSize);
    }

    /// <summary>
    /// Unloads all the preloaded objects from a source Object.
    /// </summary>
    /// <param name='sourceObj'>
    /// Source object.
    /// </param>
    static public void UnloadObjects(GameObject sourceObj)
    {
        Instance.removeObjectsFromPool(sourceObj);
    }

    /// <summary>
    /// Gets a value indicating whether all objects defined in the Editor are loaded or not.
    /// </summary>
    /// <value>
    /// <c>true</c> if all objects are loaded; otherwise, <c>false</c>.
    /// </value>
    static public bool AllObjectsLoaded
    {
        get
        {
            return Instance.m_allObjectsLoaded;
        }
    }

    ////////////////////////////////////////////////////////////////////////////////

    [SerializeField]
    bool hideObjectsInHierarchy = false;
    [SerializeField]
    [Tooltip("pool size for each object")]
    int m_poolSize = 1;
    [SerializeField]
    GameObject[] m_fxPrefabs = new GameObject[0];

    private bool m_allObjectsLoaded;
    private Dictionary<int, List<GameObject>> instantiatedObjects = new Dictionary<int, List<GameObject>>();
    private Dictionary<int, int> poolCursors = new Dictionary<int, int>();
    private Dictionary<int, int> poolNameHash = new Dictionary<int, int>();

    // Use this for initialization
    void Start()
    {
        m_allObjectsLoaded = false;

        for (int i = 0; i < m_fxPrefabs.Length; i++)
        {
            PreloadObject(m_fxPrefabs[i], m_poolSize);
        }

        m_allObjectsLoaded = true;
    }

    ////////////////////////////////////////////////////////////////////////////////
    private void addObjectToPool(GameObject sourceObject, int number)
    {
        int uniqueId = sourceObject.GetInstanceID();

        //Add new entry if it doesn't exist
        if (!instantiatedObjects.ContainsKey(uniqueId))
        {
            instantiatedObjects.Add(uniqueId, new List<GameObject>());
            poolCursors.Add(uniqueId, 0);
            poolNameHash.Add(sourceObject.name.GetHashCode(), instantiatedObjects.Count - 1);
        }

        //Add the new objects
        GameObject newObj;
        for (int i = 0; i < number; i++)
        {
            newObj = (GameObject)Instantiate(sourceObject);
#if UNITY_3_5
				newObj.SetActiveRecursively(false);
#else
            newObj.SetActive(false);
#endif

            instantiatedObjects[uniqueId].Add(newObj);

            if (hideObjectsInHierarchy)
                newObj.hideFlags = HideFlags.HideInHierarchy;
        }
    }
    ////////////////////////////////////////////////////////////////////////////////
    private void removeObjectsFromPool(GameObject sourceObject)
    {
        int uniqueId = sourceObject.GetInstanceID();

        if (!instantiatedObjects.ContainsKey(uniqueId))
        {
            Debug.LogWarning("[EffectManager.removeObjectsFromPool()] There aren't any preloaded object for: " + sourceObject.name + " (ID:" + uniqueId + ")");
            return;
        }

        //Destroy all objects
        for (int i = instantiatedObjects[uniqueId].Count - 1; i >= 0; i--)
        {
            GameObject obj = instantiatedObjects[uniqueId][i];
            instantiatedObjects[uniqueId].RemoveAt(i);
            GameObject.Destroy(obj);
        }

        //Remove pool entry
        instantiatedObjects.Remove(uniqueId);
        poolCursors.Remove(uniqueId);
    }
    ////////////////////////////////////////////////////////////////////////////////
}
