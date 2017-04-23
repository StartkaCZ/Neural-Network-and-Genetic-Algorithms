using UnityEngine;
using System.Collections.Generic;

public class Region : MonoBehaviour
{
    struct CreationData
    {
        public CreationData(Vector2 p, Vector2 s)
        {
            position = p;
            size = s;
        }

        public Vector2 position;
        public Vector2 size;
    }

    Transform           _transform;
    ParticleSystem      _particleSystem;

    List<YellowBall>    _yellowBalls;

    Vector2             _size;

    [SerializeField]
    List<GameObject>    _obstacles;


    /// <summary>
    /// Initialize the region with its own size.
    /// It will not generate objects
    /// Particle effect terminated.
    /// </summary>
    /// <param name="size"></param>
    public void Initialize(Vector2 size)
    {
        _transform = transform;

        _yellowBalls = new List<YellowBall>(ConstHolder.MAX_YELLOWBALLS);
       
        _particleSystem = GetComponentInChildren<ParticleSystem>();
        _particleSystem.enableEmission = false;

        _size = size;
    }
    /// <summary>
    /// Setup a region with objects and particle effect.
    /// </summary>
    /// <param name="size"></param>
    public void Setup (Vector2 size)
    {
        Initialize(size);

        _particleSystem.enableEmission = true;

        List<CreationData> positionsOccupied = new List<CreationData>();

        GenerateObstacles(ref positionsOccupied);
        GenerateYellowBalls(ref positionsOccupied);
    }

    #region Object Generation

    /// <summary>
    /// Generates YellowBall randomly around the region.
    /// </summary>
    /// <param name="positionsOccupied"></param>
    void GenerateYellowBalls(ref List<CreationData> positionsOccupied)
    {
        Vector2 size = PrefabFactory.GetSize(PrefabFactory.Prefab.YellowBall) * 0.5f;
        Vector2 failedToPosition = new Vector3(-1, -1);

        int yellowBallCount = 0;
        int yellowBallsToCreate = Random.Range(0, ConstHolder.MAX_YELLOWBALLS);

        while (yellowBallCount != yellowBallsToCreate)
        {
            Vector2 position = GetAvailabePosition(size, ref positionsOccupied);

            if (position != failedToPosition)
            {
                positionsOccupied.Add(new CreationData(position, size));

                GameObject clone = PrefabFactory.CreateGameObject(PrefabFactory.Prefab.YellowBall);
                clone.transform.position = new Vector3(position.x, clone.transform.position.y, position.y);
                clone.transform.parent = _transform;

                YellowBall yellowBall = clone.GetComponent<YellowBall>();
                yellowBall.Initialize();

                _yellowBalls.Add(yellowBall);
                yellowBallCount++;
            }
            else
            {
                break;
            }
        }
    }

    /// <summary>
    /// Generates obstacles randomly around the region.
    /// Actiavtes one of its walls.
    /// </summary>
    /// <param name="prefab"></param>
    /// <param name="positionsOccupied"></param>
    void GenerateObstacles(ref List<CreationData> positionsOccupied)
    {
        int obstacleCount = 0;
        int obstaclesToCreate = Random.Range(1, DataManager.maximumObstacles + 1);

        ActivateWall(ref positionsOccupied);

        Vector2 size = PrefabFactory.GetSize(PrefabFactory.Prefab.Wall) * 0.5f;
        Vector2 failedToPosition = new Vector2(-1, -1);

        while (obstacleCount < obstaclesToCreate)
        {
            Vector2 position = GetAvailabePosition(size, ref positionsOccupied);

            if (position != failedToPosition)
            {
                positionsOccupied.Add(new CreationData(position, size));

                GameObject clone = PrefabFactory.CreateGameObject(PrefabFactory.Prefab.Wall);
                Transform cloneTransform = clone.transform;

                cloneTransform.position = new Vector3(position.x, cloneTransform.position.y, position.y);
                cloneTransform.parent = _transform;

                _obstacles.Add(clone);
                obstacleCount++;
            }
            else
            {
                break;
            }
        }
    }

    /// <summary>
    /// Activates one of the 3 walls with a random chance.
    /// </summary>
    /// <param name="positionsOccupied"></param>
    void ActivateWall(ref List<CreationData> positionsOccupied)
    {
        float chance = Random.Range(0.0f, 1.1f);
        if (chance < 0.4f)
        {
            ActivateWallAtIndex(ref positionsOccupied, 0);
        }
        else if (chance < 0.8f)
        {
            ActivateWallAtIndex(ref positionsOccupied, 1);
        }
        else
        {
            ActivateWallAtIndex(ref positionsOccupied, 2);
        }
    }

    /// <summary>
    /// Activates an indexed wall.
    /// </summary>
    /// <param name="positionsOccupied"></param>
    /// <param name="index"></param>
    void ActivateWallAtIndex(ref List<CreationData> positionsOccupied, int index)
    {
        _obstacles[index].SetActive(true);

        Transform cloneTransform = _obstacles[index].transform;
        Vector2 position = new Vector2(cloneTransform.position.x, cloneTransform.position.z);
        Vector2 scaleSize = new Vector2(cloneTransform.localScale.x, cloneTransform.localScale.z);

        positionsOccupied.Add(new CreationData(position, scaleSize));
    }

    #endregion

    #region RandomGeneration

    /// <summary>
    /// Gets an available position within the region
    /// </summary>
    /// <param name="groundSize"></param>
    /// <param name="othersize"></param>
    /// <returns></returns>
    Vector2 GetAvailabePosition(Vector2 othersize, ref List<CreationData> positionsOccupied)
    {
        CreationData data = new CreationData(GetRandomPosition(othersize), othersize);

        int guard = 0;
        while (ShouldRespawn(data, ref positionsOccupied))
        {
            data.position = GetRandomPosition(othersize);

            //safety guard.
            if (guard == 1000)
            {
                data.position = new Vector2(-1, -1);
                break;
            }
            else
            {
                guard++;
            }
        }

        return data.position;
    }

    /// <summary>
    /// Creates a random position
    /// </summary>
    /// <param name="groundSize"></param>
    /// <param name="otherSize"></param>
    /// <returns></returns>
    Vector2 GetRandomPosition(Vector2 otherSize)
    {
        Vector2 position = new Vector2(_transform.position.x + Random.Range(-_size.x + otherSize.x, _size.x - otherSize.x),
                                       _transform.position.z + Random.Range(-_size.y + otherSize.y, _size.y - otherSize.y));

        return position;
    }

    /// <summary>
    /// Checks whether or not a new position is needed
    /// </summary>
    /// <param name="other"></param>
    /// <param name="positionsOccupied"></param>
    /// <returns>true if new position is a must</returns>
    bool ShouldRespawn(CreationData other, ref List<CreationData> positionsOccupied)
    {
        bool shouldRespawn = false;

        for (int i = 0; i < positionsOccupied.Count; i++)
        {
            float diameter = other.size.magnitude + positionsOccupied[i].size.magnitude;

            if (Vector2.Distance(other.position, positionsOccupied[i].position) < diameter)
            {
                shouldRespawn = true;
                break;
            }
        }

        return shouldRespawn;
    }

    #endregion


    public Vector3 position
    {
        get { return _transform.position; }
    }
}
