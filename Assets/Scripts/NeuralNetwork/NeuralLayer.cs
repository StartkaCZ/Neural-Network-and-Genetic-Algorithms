using UnityEngine;
using System.Collections.Generic;


public class NeuralLayer
{
    int             _totalNeurons;
    int             _totalInputs;

    List<Neuron>    _neurons;


    public NeuralLayer()
    {
        _totalNeurons = 0;
        _totalInputs = 0;

        _neurons = new List<Neuron>();
    }

    /// <summary>
    /// Populates a layer with new neurons and all neurons with all inputs.
    /// </summary>
    /// <param name="numberNeurons"></param>
    /// <param name="numberInputs"></param>
    public void PopulateLayer(int numberNeurons, int numberInputs)
    {
        _totalInputs = numberInputs;
        _totalNeurons = numberNeurons;

        //Safety check: Adds any missing Neurons
        if (_neurons.Count < numberNeurons)
        {
            for (int i = 0; i < numberNeurons; i++)
            {
                _neurons.Add(new Neuron());
            }
        }

        //Populates all of the neurons
        for (int i = 0; i < numberNeurons; i++)
        {
            _neurons[i].Populate(numberInputs);
        }
    }

    /// <summary>
    /// Loads a layer into this layer.
    /// </summary>
    /// <param name="input"></param>
    public void LoadLayer(List<Neuron> input)
    {
        _totalInputs = input[0].numberInputs;

        _totalNeurons = input.Count;
        _neurons = input;
    }

    /// <summary>
    /// Evaluates 
    /// </summary>
    /// <param name="input"></param>
    /// <param name="output"></param>
    public void Evaluate(List<float> input, ref List<float> output)
    {
        int inputIndex = 0;

        List<float> outputs = new List<float>(_totalNeurons);

        //cycle over all the neurons and sum their weights against the inputs
        for (int i = 0; i < _totalNeurons; i++)
        {
            float activation = 0.0f;

            //sum the weights to the activation value
            //we do the sizeof the weights - 1 so that we can add in the bias to the activation afterwards.
            for (int j = 0; j < _neurons[i].numberInputs - 1; j++)
            {
                activation += input[inputIndex] * _neurons[i].weights[j];
                inputIndex++;
            }

            //add the bias, the bias will act as a threshold value to
            activation += _neurons[i].weights[_neurons[i].numberInputs] * (-1.0f);//BIAS == -1.0f

            outputs.Add(Sigmoid(activation, 1.0f));
            inputIndex = 0;
        }

        output = outputs;
    }

    /// <summary>
    /// Sigmoid activation function.
    /// Sigmoid(x) = 1 / (1 + exp(-x))
    /// </summary>
    /// <param name="a">activation</param>
    /// <param name="p">probability</param>
    /// <returns></returns>
    public float Sigmoid(float a, float p)
    {
        float ap = (-a) / p;
        return (1 / (1 + Mathf.Exp(ap)));
    }


    /// <summary>
    /// Sets all of the weights for all of the neurons
    /// </summary>
    /// <param name="weights"> all of the weights collected from all of the neurons</param>
    /// <param name="numberNeurons"></param>
    /// <param name="numberInputs"></param>
    public void SetWeights(List<float> weights, int numberNeurons, int numberInputs)
    {
        _totalInputs = numberInputs;
        _totalNeurons = numberNeurons;

        //Safe Check: adds more neurons if some are missing
        if (_neurons.Count < numberNeurons)
        {
            for (int i = 0; i < numberNeurons - _neurons.Count; i++)
            {
                _neurons.Add(new Neuron());
            }
        }

        //Copy the weights into the neurons.
        for (int i = 0; i < numberNeurons; i++)
        {
            //Safe Check: adds more weights if some are missing
            if (_neurons[i].weights.Count < numberInputs)
            {
                int amount = numberInputs - _neurons[i].weights.Count;

                for (int k = 0; k < amount; k++)
                {
                    _neurons[i].weights.Add(0.0f);
                }
            }

            //sets a weight to a weight for all weights.
            for (int j = 0; j < numberInputs; j++)
            {
                _neurons[i].weights[j] = weights[j];
            }
        }
    }

    /// <summary>
    /// Sets all of the neurons within the layer.
    /// </summary>
    /// <param name="neurons"></param>
    /// <param name="numberNeurons"></param>
    /// <param name="numberInputs"></param>
    public void SetNeurons(List<Neuron> neurons, int numberNeurons, int numberInputs)
    {
        _totalInputs = numberInputs;
        _totalNeurons = numberNeurons;
        _neurons = neurons;
    }

    /// <summary>
    /// Gets all of the weights within the Layer.
    /// Gets all weights for every neuron
    /// </summary>
    /// <param name="output"></param>
    public void GetWeights(ref List<float> output)
    {
        //clears previous outputs
        output.Clear();

        //assigns with all of the weights that the layer has.
        for (int i = 0; i < _totalNeurons; i++)
        {
            for (int j = 0; j < _neurons[i].weights.Count; j++)
            {
                output.Add(_neurons[i].weights[j]);
            }
        }
    }
}