using UnityEngine;
using System.Collections.Generic;


public class Neuron 
{
    int          _numberInputs;

    List<float>  _weights;

   
    public Neuron()
    {
        _numberInputs = 0;
        _weights = new List<float>();
    }
    
    /// <summary>
    /// Initialize the class.
    /// </summary>
    /// <param name="weightsIn"></param>
    /// <param name="numberOfInputs"></param>
    public void Initialize(List<float> weightsIn, int numberOfInputs)
    {
        _numberInputs = numberOfInputs;
        _weights = weightsIn;
    }

    /// <summary>
    /// Pupulates a neuron with random valued weights.
    /// </summary>
    /// <param name="numberOfInputs"></param>
    public void Populate(int numberOfInputs)
    {
        _numberInputs = numberOfInputs;

        //Initilise the weights
        for (int i = 0; i < numberOfInputs; i++)
        {
            _weights.Add(GeneticAlgorithm.RandomClamped());
        }

        //add an extra weight as the bias (the value that acts as a threshold in a step activation).
        _weights.Add(GeneticAlgorithm.RandomClamped());
    }


    public int numberInputs
    {
        get { return _numberInputs; }
    }

    public List<float> weights
    {
        get { return _weights; }
    }
}
