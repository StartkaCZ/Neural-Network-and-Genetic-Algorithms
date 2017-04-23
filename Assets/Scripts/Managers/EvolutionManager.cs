using UnityEngine;
using System.Collections.Generic;

public class EvolutionManager : MonoBehaviour
{
    List<JsonWriterReader.ImprovementData>  _dataTracked;
    List<NN_Agent>                          _agents;

    GeneticAlgorithm                        _geneticAlgorithm;

    int[]                                   _networkStructure;

    float                                   _currentBestFitness;
    float                                   _bestFitness;
    float                                   _timeTillEvolution;

    int                                     _genomesLeftAlive;
    int                                     _generationsSinceImprovement;
    int                                     _testsRan;

    bool                                    _displayText;
    bool                                    _newGeneration;
    bool                                    _saveBestGenome;

    [SerializeField]
    float[]                                 _fitnesses;

    [SerializeField]
    UnityEngine.UI.Text                     _textGA_data;
    [SerializeField]
    GameObject                              _fpsText;

    [SerializeField]
    int                                     _generationToLoad;

    /* * * * * * * * * * * * * * * * *
     * DATA TO TRACK
     * * * * * * * * * * * * * * * * *
     * genomes alive
     * completed
     * * * * * * * * * * * * * * * * *
     * Generation
     * fitness
     * generations since improvement
     * * * * * * * * * * * * * * * * * */

    
    /// <summary>
    /// Initializes the class with agents created by the game manager
    /// </summary>
    /// <param name="agents"></param>
    public void Initialize(List<NN_Agent> agents)
    {
        _fitnesses = new float[agents.Count];
        _networkStructure = new int[] { 7, 2 };
        _agents = agents;

        InitializeVariables();

        if (DataManager.gameMode == DataManager.GameMode.NeuralNetworkTraining)
        {//if we are playing the training, set it up for training
            _geneticAlgorithm.GenerateNewPopulation(_agents.Count);
            LoadBestGenomeIntoPopulation();
            SetupPopulation();
        }
        else
        {//otherwise load the best genome into the agents.
            LoadBestGenomes();
        }

        _textGA_data.text = "";
    }

    /// <summary>
    /// Initializes all of the variables
    /// </summary>
    void InitializeVariables()
    {
        _displayText = true;
        _newGeneration = false;
        _saveBestGenome = false;

        _dataTracked = new List<JsonWriterReader.ImprovementData>();
        _geneticAlgorithm = new GeneticAlgorithm(_networkStructure);

        _currentBestFitness = 0.0f;
        _bestFitness = _currentBestFitness;

        _genomesLeftAlive = _agents.Count;
        _generationsSinceImprovement = 0;

        _timeTillEvolution = ConstHolder.TIME_TILL_EVOLUTION;
        _testsRan = 0;
    }

    /// <summary>
    /// Loads the best genome to add into the population.
    /// </summary>
    void LoadBestGenomeIntoPopulation()
    {
        JsonWriterReader.GenomeData data = LoadBestGenome();

        if (data != null)
        {//if not null, add it into the population
            _geneticAlgorithm.LoadGenome(ref data);
        }
    }

    /// <summary>
    /// Setups the Neural networks of the Agents
    /// to be represented by the genomes created
    /// by the genetic algorithm.
    /// </summary>
    void SetupPopulation()
    {
        List<Genome> population = _geneticAlgorithm.population;

        for (int i = 0; i < _agents.Count; i++)
        {
            NeuralNetwork neuralNetwork = new NeuralNetwork();
            neuralNetwork.SetupFromGenome(population[i], _networkStructure);

            _agents[i].InitializeNeuralNetwork(neuralNetwork);
        }
    }

    /// <summary>
    /// Loads the best genome into all of the agents.
    /// </summary>
    void LoadBestGenomes()
    {
        JsonWriterReader.GenomeData data = LoadBestGenome();
        NeuralNetwork neuralNetwork = new NeuralNetwork();

        if (data != null)
        {//if the data isn't null 
            //create the best genome
            Genome genome = _geneticAlgorithm.CreateGenomeFromData(LoadBestGenome());
            //set up a neural network with that genome
            neuralNetwork.SetupFromGenome(genome, _networkStructure);

            for (int i = 0; i < _agents.Count; i++)
            {//set each member with that network
                _agents[i].InitializeNeuralNetwork(neuralNetwork);
            }
        }
        else
        {//otherwise create a new neural network for each memeber.
            for (int i = 0; i < _agents.Count; i++)
            {
                neuralNetwork.Create(_networkStructure);
                _agents[i].InitializeNeuralNetwork(neuralNetwork);
            }
        }
    }


    /// <summary>
    /// Updates the class
    /// </summary>
    public void ManualUpdate()
    {
        UpdateAgents();
        CheckForBestFitness();
        EvolutionTimer();

        InputCheck();
        UpdateText();
    }

    /// <summary>
    /// Checks which Agents are Alive.
    /// Checks what is the current best fitness.
    /// </summary>
    void UpdateAgents()
    {
        _currentBestFitness = 0;
        _genomesLeftAlive = 0;

        for (int i = 0; i < _agents.Count; i++)
        {
            if (_agents[i].alive)
            {
                _genomesLeftAlive++;
            }

            _fitnesses[i] = _agents[i].fitness;
            if (_agents[i].fitness > _currentBestFitness)
            {
                _currentBestFitness = _agents[i].fitness;
            }
        } 
    }


    /// <summary>
    /// Checks if a new top fitness has been reached
    /// </summary>
    void CheckForBestFitness()
    {
        if (_currentBestFitness > _bestFitness)
        {//if new fitness has been reachehd
            _bestFitness = _currentBestFitness;

            if (_generationsSinceImprovement > 1)
            {//if we haven't improved more than once then add new data and save the best genome.
                _dataTracked.Add(new JsonWriterReader.ImprovementData(_currentBestFitness, _geneticAlgorithm.generation, _generationsSinceImprovement));
                _saveBestGenome = true;
            }

            _generationsSinceImprovement = 0;
        }
    }

    /// <summary>
    /// Checks whether its time to evolve.
    /// </summary>
    void EvolutionTimer()
    {
        if (_genomesLeftAlive == 0 || _timeTillEvolution < 0)
        {//if all genomes died or time has reached
            if (_timeTillEvolution < 0 || _saveBestGenome)
            {//if time has been reached or best genome is to be saved
                if (_timeTillEvolution < 0)
                {//if time has been reached, save the data
                    SaveEvolutionData();
                    //running new test cycle
                    _testsRan++;
                }
                //save the best genome
                SaveBestGenome();
                _saveBestGenome = false;
            }
            //evolve the population
            EvolvePopulation();
        }
        else
        {//otherwise just decrement the timer.
            _timeTillEvolution -= Time.deltaTime;
        }
    }

    /// <summary>
    /// Checks if there is any input.
    /// </summary>
    void InputCheck()
    {
        if (Input.anyKeyDown)
        {
            if (Input.GetKeyDown(KeyCode.P))
            {//save evolution data
                SaveEvolutionData();
                //SaveBestGenome();
            }
            else if (Input.GetKey(KeyCode.Tab))
            {//toggle text display
                _displayText = !_displayText;
            }
        }
    }

    /// <summary>
    /// Update text information if it is to be shown
    /// </summary>
    void UpdateText()
    {
        if (_displayText)
        {
            _textGA_data.text = "CurrentBestFitness: " + _currentBestFitness + "\n" +
                         "BestFitness: " + _bestFitness + "\n" +
                         "TimeTillEvolution: " + _timeTillEvolution + "\n" +
                         "GenomesAlive: " + _genomesLeftAlive + " of " + _agents.Count + "\n" +
                         "Generation: " + _geneticAlgorithm.generation + "\n" +
                         "Generations since Last Improvement: " + _generationsSinceImprovement + "\n" +
                         "TImeScale: " + Time.timeScale;

            _fpsText.SetActive(true);
        }
        else
        {
            _textGA_data.text = "";
            _fpsText.SetActive(false);
        }
    }


    /// <summary>
    /// Evolves and preapres the new population for testing.
    /// </summary>
    void EvolvePopulation()
    {
        _newGeneration = true;

        BreedPopulation();
        SetupPopulation();

        _currentBestFitness = 0;
        _timeTillEvolution = ConstHolder.TIME_TILL_EVOLUTION;

        _generationsSinceImprovement++;
    }

    /// <summary>
    /// Creates a new population by evolution.
    /// </summary>
    void BreedPopulation()
    {
        for (int i = 0; i < _agents.Count; i++)
        {//sets fitness to the genomes for all agents
            _geneticAlgorithm.SetGenomeFitness(_agents[i].fitness, i);
        }

        //breeds a new population
        _geneticAlgorithm.BreedPopulation();
    }


    /// <summary>
    /// Loads genome based on the information provided
    /// </summary>
    JsonWriterReader.GenomeData LoadBestGenome()
    {
        string path = "BestGenomeWeights-" + _geneticAlgorithm.totalWeights + "_Generation-" + _generationToLoad;

        return JsonWriterReader.ReadJson<JsonWriterReader.GenomeData>(path + ".json");
    }

    /// <summary>
    /// Saves currently the best genome
    /// </summary>
    void SaveBestGenome()
    {
        Genome bestGenome = _geneticAlgorithm.GetBestGenome();

        JsonWriterReader.GenomeData data = new JsonWriterReader.GenomeData(bestGenome.weights);

        string path = "BestGenomeWeights-" + data.weights.Count + "_Generation-" + _geneticAlgorithm.generation;

        JsonWriterReader.WriteJson(path + ".json", ref data);
    }

    /// <summary>
    /// Saves currently the best genome
    /// </summary>
    void SaveEvolutionData()
    {
        JsonWriterReader.EvolutionData data = new JsonWriterReader.EvolutionData(_dataTracked, _genomesLeftAlive, _geneticAlgorithm.timesLegendUsed);

        string path = "EvolutionData" + _testsRan;

        JsonWriterReader.WriteJson(path + ".json", ref data);
    }

    public bool newGeneration
    {
        get { return _newGeneration; }
        set { _newGeneration = value; }
    }
}
