using UnityEngine;
using System.Collections.Generic;

public class ScriptedAI : Unit
{
    Raycast             _raycast;

    float               _progressingDistance;

    [SerializeField]
    List<float>         _distances;

    [SerializeField]
    float               _rightForce;
    [SerializeField]
    float               _leftForce;


    /// <summary>
    /// Initializes the class with its type.
    /// </summary>
    /// <param name="type"></param>
    public override void Initialize(UnitType type)
    {
        base.Initialize(type);

        _raycast = GetComponent<Raycast>();

        _raycast.Initialize();

        _distances = new List<float>(_raycast.numberOfRays);

        for (int i = 0; i < _raycast.numberOfRays; i++)
        {//sets up distances to be the amount of rays being casted
            _distances.Add(1);
        }

        _progressingDistance = 0;
        _rightForce = 0;
        _leftForce = 0;
    }

    /// <summary>
    /// Updates the class.
    /// </summary>
    public override void ManualUpdate()
    {
        if (_alive)
        {// if it is alive, it updates
            _raycast.UpdateRays();

            UpdateInputs();
            Evaluate();
            UpdateOutputs();
            CheckForBadBehaviour();

            _points = _yellowBallsCollected + (int)position.z;
        }
    }

    /// <summary>
    /// Reads in normalized distances of the rays.
    /// Each distance is reversed and normalized.
    /// </summary>
    void UpdateInputs()
    {
        float lenght = ConstHolder.UNIT_LINE_OF_SIGHT;

        _distances[(int)Raycast.Direction.Left] = Normalise(_raycast.GetDistance(Raycast.Direction.Left), lenght);
        _distances[(int)Raycast.Direction.FrontFrontLeft] = Normalise(_raycast.GetDistance(Raycast.Direction.FrontFrontRight), lenght);
        _distances[(int)Raycast.Direction.FrontLeft] = Normalise(_raycast.GetDistance(Raycast.Direction.FrontLeft), lenght);
        _distances[(int)Raycast.Direction.Front] = Normalise(_raycast.GetDistance(Raycast.Direction.Front), lenght);
        _distances[(int)Raycast.Direction.FrontRight] = Normalise(_raycast.GetDistance(Raycast.Direction.FrontRight), lenght);
        _distances[(int)Raycast.Direction.FrontFrontRight] = Normalise(_raycast.GetDistance(Raycast.Direction.FrontFrontRight), lenght);
        _distances[(int)Raycast.Direction.Right] = Normalise(_raycast.GetDistance(Raycast.Direction.Right), lenght);
    }

    /// <summary>
    /// If there is nothing in the way, distance will be 0.
    /// Reverse it.
    /// </summary>
    /// <param name="value"></param>
    /// <param name="maxValue"></param>
    /// <returns></returns>
    float Normalise(float value, float maxValue)
    {
        float depth = value / maxValue;
        return 1 - depth;
    }

    /// <summary>
    /// Sets left and right forces based relative to the distances.
    /// Based on rays that have hit.
    /// </summary>
    void Evaluate()
    {
        _leftForce = 0.0f;
        _rightForce = 0.0f;

        int front = (int)Raycast.Direction.Front;

        //loops from left to front. (front is excluded)
        for (int i = 0; i < front; i++)
        {
            if (_distances[i] != 1.0f)
            {//checks if the ray has hit anything.
                _rightForce += _distances[i];
            }
        }

        //loops from front to right. (front is excluded)
        for (int i = front+1; i < _distances.Count; i++)
        {
            if (_distances[i] != 1.0f)
            {//checks if the ray has hit anything.
                _leftForce += _distances[i];
            }
        }

        
        if (_distances[(int)Raycast.Direction.Front] != 1.0f)
        {//checks if the front ray has hit anything.
            if (_leftForce > _rightForce)
            {//if the left force is greater
                _leftForce += _distances[(int)Raycast.Direction.Front];
            }
            else
            {//if the right force is greater
                _rightForce += _distances[(int)Raycast.Direction.Front];
            }
        }

        float lenght = new Vector2(_leftForce, _rightForce).magnitude;
        //normalize the forces so they add to 1
        if (lenght > 0)
        {
            _leftForce /= lenght;
            _rightForce /= lenght;
        }
    }

    /// <summary>
    /// Update the outcome.
    /// Rotates based on the forces.
    /// Sets velocity relative to its angle.
    /// </summary>
    void UpdateOutputs()
    {
        //calculate by how much to rotate and a new angle.
        float angleChange = (_rightForce - _leftForce) * ConstHolder.UNIT_MAX_ROTATION * Time.deltaTime;
        float headingAngle = -_transform.rotation.eulerAngles.y + angleChange;
        float radian = headingAngle * Mathf.PI / 180;

        //set new angle
        _transform.eulerAngles += new Vector3(0, angleChange, 0);
        //set velocity based on direction
        _rigidBody.velocity = new Vector3(DataManager.currentSpeed * Mathf.Cos(radian),
                                          0,
                                          DataManager.currentSpeed * Mathf.Sin(radian));
    }

    /// <summary>
    /// Checks for bad behaviour.
    /// If it tries to reverse, it gets terminated.
    /// </summary>
    void CheckForBadBehaviour()
    {
        if (_progressingDistance > position.z)
        {
            Killed();
        }
        else
        {
            _progressingDistance = position.z;
        }
    }


    /// <summary>
    /// Resets the agent for a new test.
    /// </summary>
    public override void Reset()
    {
        base.Reset();

        _progressingDistance = 0.0f;
    }
}
