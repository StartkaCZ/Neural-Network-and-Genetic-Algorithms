using UnityEngine;
using System.Collections.Generic;

public enum UnitType
{
    NeuralNetwork,
    ScriptedAI,
    Player,

    Count
}

public class PrefabFactory : MonoBehaviour
{
    public enum Prefab
    {
        Region,
        YellowBall,
        Obstacle,
        Unit,
        Wall,
        ParticleSystem,

        Count
    }


    static Dictionary<Prefab, GameObject> _prefabs;
    static Dictionary<UnitType, Material> _materials;


    /// <summary>
    /// Initializes the class.
    /// </summary>
    static public void Initialize()
    {
        _prefabs = new Dictionary<Prefab, GameObject>((int)Prefab.Count);
        _materials = new Dictionary<UnitType, Material>((int)UnitType.Count);

        LoadContent();
    }

    /// <summary>
    /// Loads all of the contect from resource folder.
    /// Loads prefabs.
    /// Loads materials.
    /// </summary>
    static void LoadContent()
    {
        _prefabs.Add(Prefab.Region, Resources.Load<GameObject>("Prefabs/RegionPrefab"));
        _prefabs.Add(Prefab.YellowBall, Resources.Load<GameObject>("Prefabs/YellowBallPrefab"));
        _prefabs.Add(Prefab.Obstacle, Resources.Load<GameObject>("Prefabs/ObstaclePrefab"));
        _prefabs.Add(Prefab.Unit, Resources.Load<GameObject>("Prefabs/UnitPrefab"));
        _prefabs.Add(Prefab.Wall, Resources.Load<GameObject>("Prefabs/WallPrefab"));
        _prefabs.Add(Prefab.ParticleSystem, Resources.Load<GameObject>("Prefabs/ParticleSystem"));

        _materials.Add(UnitType.NeuralNetwork, Resources.Load<Material>("Materials/NNAI"));
        _materials.Add(UnitType.ScriptedAI, Resources.Load<Material>("Materials/ScriptedAI"));
        _materials.Add(UnitType.Player, Resources.Load<Material>("Materials/Player"));
    }


    /// <summary>
    /// Create a game object clone of a specific prefab
    /// </summary>
    /// <param name="prefab"></param>
    /// <returns></returns>
    static public GameObject CreateGameObject(Prefab prefab)
    {
        GameObject clone = null;

        switch (prefab)
        {
            case Prefab.Region:
                clone = CreateRegionClone();
                break;
            case Prefab.YellowBall:
                clone = CreateYellowBallClone();
                break;
            case Prefab.Obstacle:
                clone = CreateObstacleClone();
                break;
            case Prefab.Unit:
                clone = CreateUnitClone();
                break;
            case Prefab.Wall:
                clone = CreateWallClone();
                break;
            case Prefab.ParticleSystem:
                clone = CreateParticleSystemClone();
                break;

            default:
                Debug.Log("WHAAAt is this prefab?");
                break;
        }

        return clone;
    }

    #region GameObject Creation

    static  GameObject CreateRegionClone()
    {
        GameObject clone = Instantiate(_prefabs[Prefab.Region],
                                       _prefabs[Prefab.Region].transform.position,
                                       _prefabs[Prefab.Region].transform.rotation) as GameObject;

        return clone;
    }

    static GameObject CreateYellowBallClone()
    {
        GameObject clone = Instantiate(_prefabs[Prefab.YellowBall],
                                       _prefabs[Prefab.YellowBall].transform.position,
                                       _prefabs[Prefab.YellowBall].transform.rotation) as GameObject;

        return clone;
    }

    static GameObject CreateObstacleClone()
    {
        GameObject clone = Instantiate(_prefabs[Prefab.Obstacle],
                                       _prefabs[Prefab.Obstacle].transform.position,
                                       _prefabs[Prefab.Obstacle].transform.rotation) as GameObject;

        return clone;
    }

    static GameObject CreateUnitClone()
    {
        GameObject clone = Instantiate(_prefabs[Prefab.Unit],
                                       _prefabs[Prefab.Unit].transform.position,
                                       _prefabs[Prefab.Unit].transform.rotation) as GameObject;

        return clone;
    }

    static GameObject CreateWallClone()
    {
        GameObject clone = Instantiate(_prefabs[Prefab.Wall],
                                       _prefabs[Prefab.Wall].transform.position,
                                       _prefabs[Prefab.Wall].transform.rotation) as GameObject;

        return clone;
    }

    static GameObject CreateParticleSystemClone()
    {
        GameObject clone = Instantiate(_prefabs[Prefab.ParticleSystem],
                                       _prefabs[Prefab.ParticleSystem].transform.position,
                                       _prefabs[Prefab.ParticleSystem].transform.rotation) as GameObject;

        return clone;
    }

    #endregion

    /// <summary>
    /// Create a unit based on the given type.
    /// </summary>
    /// <param name="unitType"></param>
    /// <returns></returns>
    static public Unit CreateUnit(UnitType unitType)
    {
        Unit unit = null;

        switch (unitType)
        {
            case UnitType.NeuralNetwork:
                unit = CreateNeuralNetworkUnit();
                break;
            case UnitType.ScriptedAI:
                unit = CreateScriptedAIUnit();
                break;
            case UnitType.Player:
                unit = CreatePlayerUnit();
                break;

            default:
                Debug.Log("WHAAAt is this unit?");
                break;
        }

        return unit;
    }

    #region Unit Creation

    /// <summary>
    /// Creates a Neural Network AI unit from a prefab.
    /// </summary>
    /// <returns></returns>
    static Unit CreateNeuralNetworkUnit()
    {
        GameObject unitClone = CreateUnitClone();

        unitClone.AddComponent<NN_Agent>();

        MeshRenderer[] meshes = unitClone.GetComponentsInChildren<MeshRenderer>();
        for (int i = 0; i < meshes.Length; i++)
        {
            meshes[i].sharedMaterial = _materials[UnitType.NeuralNetwork];
        }

        return unitClone.GetComponent<NN_Agent>();
    }

    /// <summary>
    /// Creates a scripted AI unit from a prefab.
    /// </summary>
    /// <returns></returns>
    static Unit CreateScriptedAIUnit()
    {
        GameObject unitClone = CreateUnitClone();

        unitClone.AddComponent<ScriptedAI>();

        MeshRenderer[] meshes = unitClone.GetComponentsInChildren<MeshRenderer>();
        for (int i = 0; i < meshes.Length; i++)
        {
            meshes[i].sharedMaterial = _materials[UnitType.ScriptedAI];
        }

        return unitClone.GetComponent<ScriptedAI>();
    }

    /// <summary>
    /// Creates a Player unit from a prefab.
    /// </summary>
    /// <returns></returns>
    static Unit CreatePlayerUnit()
    {
        GameObject unitClone = CreateUnitClone();

        unitClone.AddComponent<Player>();
        Destroy(unitClone.GetComponent<Raycast>());

        MeshRenderer[] meshes = unitClone.GetComponentsInChildren<MeshRenderer>();
        for (int i = 0; i < meshes.Length; i++)
        {
            meshes[i].sharedMaterial = _materials[UnitType.Player];
        }

        return unitClone.GetComponent<Player>();
    }

    #endregion


    /// <summary>
    /// Get the size of a particle effect box shape
    /// </summary>
    /// <returns></returns>
    static public Vector2 GetParticleSystemSize()
    {
        return new Vector2(_prefabs[Prefab.ParticleSystem].GetComponent<ParticleSystem>().shape.box.x,
                           _prefabs[Prefab.ParticleSystem].GetComponent<ParticleSystem>().shape.box.z);
    }

    /// <summary>
    /// Get a size of a prefab
    /// </summary>
    /// <param name="prefab"></param>
    /// <returns></returns>
    static public Vector2 GetSize(Prefab prefab)
    {
        return GetObjectSize(_prefabs[prefab]);
    }

    /// <summary>
    /// Gets the size of a given object
    /// </summary>
    /// <param name="theObject">Object to get size from</param>
    /// <returns></returns>
    static Vector2 GetObjectSize(GameObject theObject)
    {
        return new Vector2(theObject.GetComponent<MeshFilter>().sharedMesh.bounds.size.x * theObject.transform.localScale.x,
                           theObject.GetComponent<MeshFilter>().sharedMesh.bounds.size.z * theObject.transform.localScale.z);
    }
}
