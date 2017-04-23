using UnityEngine;
using System.Collections.Generic;

public class NN_Agent : Unit
{
    Raycast             _raycast;
    NeuralNetwork       _neuralNetwork;

    [SerializeField]
    float               _lifeSpan;
    [SerializeField]
    float               _progressingDistance;
    [SerializeField]
    float               _fitness;


    /// <summary>
    /// Initialize the class with its type.
    /// </summary>
    /// <param name="type"></param>
    public override void Initialize(UnitType type)
    {
        base.Initialize(type);

        _raycast = GetComponent<Raycast>();

        _neuralNetwork = new NeuralNetwork();
        //_neuralNetwork.Create( new int[]{ 5, 2});

        _raycast.Initialize();

        Reset();
    }

    /// <summary>
    /// Initialize the neural network with a Neural Network.
    /// </summary>
    /// <param name="neuralNetwork"></param>
    public void InitializeNeuralNetwork(NeuralNetwork neuralNetwork)
    {
        _neuralNetwork = neuralNetwork;
    }


    /// <summary>
    /// Update the class.
    /// </summary>
    public override void ManualUpdate()
    {
        if (_alive)
        {
            _raycast.UpdateRays();

            UpdateNeuralNetwork();

            _points = _yellowBallsCollected + (int)position.z;
        }
    }

    /// <summary>
    /// Update the Neural Network.
    /// </summary>
    void UpdateNeuralNetwork()
    {
        UpdateInputs();
        UpdateOutputs();

        UpdateFitness();
        CheckForBadBehaviour();
    }

    #region Neural Network Updates

    /// <summary>
    /// Updates inputs for the network from the sensors.
    /// </summary>
    void UpdateInputs()
    {
        List<float> inputs = new List<float>(_neuralNetwork.inputs.Count);

        float lenght = ConstHolder.UNIT_LINE_OF_SIGHT;

        inputs.Add(Normalise(_raycast.GetDistance(Raycast.Direction.Left), lenght));
        inputs.Add(Normalise(_raycast.GetDistance(Raycast.Direction.FrontFrontLeft), lenght));
        inputs.Add(Normalise(_raycast.GetDistance(Raycast.Direction.FrontLeft), lenght));
        inputs.Add(Normalise(_raycast.GetDistance(Raycast.Direction.Front), lenght));
        inputs.Add(Normalise(_raycast.GetDistance(Raycast.Direction.FrontRight), lenght));
        inputs.Add(Normalise(_raycast.GetDistance(Raycast.Direction.FrontFrontRight), lenght));
        inputs.Add(Normalise(_raycast.GetDistance(Raycast.Direction.Right), lenght));

        _neuralNetwork.inputs = inputs;
        _neuralNetwork.Evaluate();
    }

    /// <summary>
    /// Takes all of the outputs and acts accordingly.
    /// Sets its new rotation and a velocity based of it.
    /// </summary>
    void UpdateOutputs()
    {
        List<float> outputs = _neuralNetwork.outputs;

        float rightForce = outputs[0];
        float leftForce = outputs[1];

        //calculate by how much to rotate and a new angle.
        float angleChange = (rightForce - leftForce) * ConstHolder.UNIT_MAX_ROTATION * Time.deltaTime;
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
    /// Updates the fitness of the network.
    /// </summary>
    void UpdateFitness()
    {
        _lifeSpan += Time.deltaTime;

        _fitness = _lifeSpan;// + _progressingDistance - _extraFitness;
    }

    /// <summary>
    /// Terminates the Agent if its misbehaving.
    /// </summary>
    void CheckForBadBehaviour()
    {
        if (_progressingDistance > position.z)
        {// if its going downwards, terminate.
            Killed();
            _fitness = 0;
        }
        else
        {//otherwise keep moving
            _progressingDistance = position.z;
        }
    }

    #region Maths

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
    /// Clamping function between min and max
    /// </summary>
    /// <param name="val"></param>
    /// <param name="min"></param>
    /// <param name="max"></param>
    /// <returns></returns>
    float Clamp(float val, float min, float max)
    {
        if (val < min)
        {
            val = min;
        }
        else if (val > max)
        {
            val = max;
        }

        return val;
    }

    #endregion

    #endregion


    /// <summary>
    /// Resets the agent for a new generation.
    /// </summary>
    public override void Reset()
    {
        base.Reset();

        _progressingDistance = 0.0f;
        _fitness = 0.0f;
        _lifeSpan = 0.0f;
    }


    public float fitness
    {
        get { return _fitness; }
    }
}
