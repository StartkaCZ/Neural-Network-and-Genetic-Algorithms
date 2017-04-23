using UnityEngine;
using System.Collections.Generic;


public class NeuralNetwork
{
    List<NeuralLayer>   _hiddenLayers;

    NeuralLayer         _inputlayer;
    NeuralLayer         _outputLayer;

    List<float>         _inputs;
    List<float>         _outputs;

    int                 _inputAmount;
    int                 _outputAmount;


    public NeuralNetwork()
    {
        _inputAmount = 0;
        _outputAmount = 0;

        _inputlayer = new NeuralLayer();
        _outputLayer = new NeuralLayer();

        _inputs = new List<float>();
        _outputs = new List<float>();

        _hiddenLayers = new List<NeuralLayer>();
    }

    /// <summary>
    /// Creates a neural network based on the provided info.
    /// </summary>
    /// <param name="numberHiddenLayers"></param>
    /// <param name="numberInputs"></param>
    /// <param name="NeuronsPerHidden"></param>
    /// <param name="numberOutputs"></param>
    public void Create(int[] networkStructure)
    {
        _inputAmount = networkStructure[0];
        _outputAmount = networkStructure[networkStructure.Length-1];

        int hiddenLayers = networkStructure.Length - 2;
        for (int i = 0; i < hiddenLayers; i++)
        {
            NeuralLayer layer = new NeuralLayer();
            layer.PopulateLayer(networkStructure[i+1], networkStructure[i]);
            _hiddenLayers.Add(layer);
        }
        
        _outputLayer.PopulateLayer(networkStructure[networkStructure.Length-1],
                                   networkStructure[networkStructure.Length-2]);
    }


    /// <summary>
    /// Clears the output.
    /// Evaluates inputs and creates outputs.
    /// Outputs are fed across other layers until output is reached.
    /// </summary>
    public void Evaluate()
    {
        _outputs.Clear();

        for (int i = 0; i < _hiddenLayers.Count; i++)
        {
            _hiddenLayers[i].Evaluate(_inputs, ref _outputs);
            _inputs = _outputs;
        }

        //Process the layeroutputs through the output layer to
        _outputLayer.Evaluate(_inputs, ref _outputs);
    }


    /// <summary>
    /// Based on the given genome, it sets the hidden layer and output layer to it.
    /// </summary>
    /// <param name="genome"></param>
    /// <param name="numberInputs"></param>
    /// <param name="numberHiddenNeurons"></param>
    /// <param name="numberOutcomes"></param>
    public void SetupFromGenome(Genome genome, int[] networkStructure)
    {
        Release();

        _inputAmount = networkStructure[0];
        _outputAmount = networkStructure[networkStructure.Length - 1];

        int index = 0;

        //set up the hidden layers
        int hiddenLayers = networkStructure.Length - 2;
        for (int i = 0; i < hiddenLayers; i++)
        {
            NeuralLayer hidden = new NeuralLayer();
            List<Neuron> neurons = new List<Neuron>();

            int hiddenNeurons = networkStructure[i + 1];
            for (int j = 0; j < hiddenNeurons; j++)
            {
                neurons.Add(new Neuron());
                List<float> weights = new List<float>();

                for (int k = 0; k < networkStructure[i] + 1; k++)
                {
                    weights.Add(genome.weights[index]);
                    index++;
                }

                neurons[j].Initialize(weights, networkStructure[i]);
            }

            hidden.LoadLayer(neurons);
            _hiddenLayers.Add(hidden);
        }

        SetupOutputLayer(ref genome, networkStructure[networkStructure.Length - 1], 
                                     networkStructure[networkStructure.Length - 2], index);
    }

    /// <summary>
    /// Sets up all of the nerons with their input weights and a bias
    /// </summary>
    /// <param name="genome"></param>
    /// <param name="numberOutputs"></param>
    /// <param name="numberInputs"></param>
    /// <param name="index"></param>
    void SetupOutputLayer(ref Genome genome, int numberOutputs, int numberInputs, int index)
    {
        //set up the output layer
        List<Neuron> outneurons = new List<Neuron>();

        for (int i = 0; i < numberOutputs; i++)
        {
            outneurons.Add(new Neuron());

            List<float> weights = new List<float>();

            for (int j = 0; j < numberInputs + 1; j++)
            {
                weights.Add(genome.weights[index]);
                index++;
            }

            outneurons[i].Initialize(weights, numberInputs);
        }

        _outputLayer = new NeuralLayer();
        _outputLayer.LoadLayer(outneurons);
    }

    /// <summary>
    /// Converts all of its weight to a genome that is returned (hidden layers + output layer).
    /// </summary>
    /// <returns></returns>
    public Genome ConvertToGenome()
    {
        Genome genome = new Genome();

        List<float> weights = new List<float>();

        for (int i = 0; i < _hiddenLayers.Count; i++)
        {
            _hiddenLayers[i].GetWeights(ref weights);
        }

        _outputLayer.GetWeights(ref weights);

        genome.weights = weights;

        return genome;
    }

    /// <summary>
    /// Clears the NeuralNetwork
    /// </summary>
    void Release()
    {
        _inputlayer = new NeuralLayer();
        _outputLayer = new NeuralLayer();
        _hiddenLayers = new List<NeuralLayer>();
    }


    public int outputAmount
    {
        get { return _outputAmount; }
    }

    public int hiddenLayersCount
    {
        get { return _hiddenLayers.Count; }
    }

    public List<float> outputs
    {
        get { return _outputs; }
    }

    public List<float> inputs
    {
        get { return _inputs; }
        set { _inputs = value; }
    }
}
