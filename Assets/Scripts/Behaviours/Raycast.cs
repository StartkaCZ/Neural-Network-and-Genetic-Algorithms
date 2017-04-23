using UnityEngine;
using System.Collections.Generic;

public class Raycast : MonoBehaviour
{
    public enum Direction
    {
        Left,
        FrontFrontLeft,
        FrontLeft,
        Front,
        FrontRight,
        FrontFrontRight,
        Right,

        Count
    }


    public struct Data
    {
        public Vector3      coordinates;
        public float        distance;
    }


    Transform               _transform;

    Data[]                  _raycastData;

    [SerializeField]
    LayerMask               _layerMask;

    [SerializeField]
    Transform               _castingOrigin;


    /// <summary>
    /// Initializes the class
    /// </summary>
    public void Initialize()
    {
        _transform = transform;

        int size = (int)Direction.Count;
        _raycastData = new Data[size];

        //fill in the amount of rays we will cast
        for (int i = 0; i < size; i++)
        {
            _raycastData[i].distance = 0;
        }

        CalculateDirections();
    }

    /// <summary>
    /// Update all of the rays.
    /// </summary>
    public void UpdateRays()
    {
        CalculateDirections();
        CastRays();
    }

    /// <summary>
    /// Update directions relative to the angle facing.
    /// </summary>
    void CalculateDirections()
    {
        const float lenght = ConstHolder.UNIT_LINE_OF_SIGHT;
        float radianOrientation = -_transform.rotation.eulerAngles.y * Mathf.PI / 180;
        float radian = radianOrientation;

        //Front
        radian = radianOrientation;
        _raycastData[(int)Direction.Front].coordinates = new Vector3(CosX(radian, lenght), 0, SinX(radian, lenght));

        //Right
        radian = radianOrientation - GetRadian(0.5f);
        _raycastData[(int)Direction.Right].coordinates = new Vector3(CosX(radian, lenght), 0, SinX(radian, lenght));

        //FrontFrontRight
        radian = radianOrientation - GetRadian(0.25f);
        _raycastData[(int)Direction.FrontFrontRight].coordinates = new Vector3(CosX(radian, lenght), 0, SinX(radian, lenght));

        //FrontRight
        radian = radianOrientation - GetRadian(0.04175f);
        _raycastData[(int)Direction.FrontRight].coordinates = new Vector3(CosX(radian, lenght), 0, SinX(radian, lenght));

        //FrontFrontLeft
        radian = radianOrientation + GetRadian(0.04175f);
        _raycastData[(int)Direction.FrontLeft].coordinates = new Vector3(CosX(radian, lenght), 0, SinX(radian, lenght));

        //FrontFrontLeft
        radian = radianOrientation + GetRadian(0.25f);
        _raycastData[(int)Direction.FrontFrontLeft].coordinates = new Vector3(CosX(radian, lenght), 0, SinX(radian, lenght));

        //Left
        radian = radianOrientation + GetRadian(0.5f);
        _raycastData[(int)Direction.Left].coordinates = new Vector3(CosX(radian, lenght), 0, SinX(radian, lenght));
    }

    /// <summary>
    /// Cos (radian) * lenghts
    /// </summary>
    /// <param name="radian"></param>
    /// <param name="lenght"></param>
    /// <returns></returns>
    float CosX(float radian, float lenght)
    {
        return Mathf.Cos(radian) * lenght;
    }
    /// <summary>
    /// Sin (radian) * lenghts
    /// </summary>
    /// <param name="radian"></param>
    /// <param name="lenght"></param>
    /// <returns></returns>
    float SinX(float radian, float lenght)
    {
        return Mathf.Sin(radian) * lenght;
    }

    /// <summary>
    /// Gets a percentage of a radian relative to PI.
    /// </summary>
    /// <param name="percentageOfPI"></param>
    /// <returns></returns>
    float GetRadian(float percentageOfPI)
    {
        return Mathf.PI * percentageOfPI;
    }

    /// <summary>
    /// Casts rays in all directions 
    /// </summary>
    void CastRays()
    {
        //Right
        CastRay(Direction.Right, Color.red);
        //FrontFrontRight
        CastRay(Direction.FrontFrontRight, Color.yellow);
        //FrontRight
        CastRay(Direction.FrontRight, Color.red);
        //Front
        CastRay(Direction.Front, Color.yellow);
        //FrontLeft
        CastRay(Direction.FrontLeft, Color.red);
        //FrontFrontLeft
        CastRay(Direction.FrontFrontLeft, Color.yellow);
        //Left
        CastRay(Direction.Left, Color.red);
    }

    /// <summary>
    /// Casts a ray in a specific direction
    /// </summary>
    /// <param name="direction"></param>
    /// <param name="debugColour"></param>
    void CastRay(Direction direction, Color debugColour)
    {
        int index = (int)direction;

        RaycastHit hit;
        Physics.Raycast(_castingOrigin.position, _raycastData[index].coordinates, out hit, ConstHolder.UNIT_LINE_OF_SIGHT, _layerMask);

        SetupRaycastData(ref hit, index, debugColour);
    }

    /// <summary>
    /// Sets up the raycast distance
    /// draws it if it hit anything
    /// </summary>
    /// <param name="hit"></param>
    /// <param name="index"></param>
    /// <param name="debugColour"></param>
    void SetupRaycastData(ref RaycastHit hit, int index, Color debugColour)
    {
        if (hit.distance != 0)
        {
            Debug.DrawLine(_castingOrigin.position, hit.point, debugColour);
        }

        _raycastData[index].distance = hit.distance;
    }

    public float GetDistance(Direction direction)
    {
        return _raycastData[(int)direction].distance;
    }

    public int numberOfRays
    {
        get { return (int)Direction.Count; }
    }
}
