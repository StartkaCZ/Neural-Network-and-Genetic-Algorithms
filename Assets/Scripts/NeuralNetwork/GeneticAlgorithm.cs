using UnityEngine;
using System.Collections.Generic;
using System.Linq;


public class GeneticAlgorithm
{
    List<Genome>    _population;
    Genome          _legend;

    int[]           _networkStructure;

    int             _generation;           
    int             _genomeID;
    int             _timesLegendUsed;
    int             _totalWeights;


    public GeneticAlgorithm(int[] networkStructure)
    {
        _genomeID = 0;
        _generation = 1;

        _networkStructure = networkStructure;
        SetTotalWeights();

        _population = new List<Genome>();
    }


    /// <summary>
    /// Generates a new population by the given size with the provided amount of weights.
    /// </summary>
    /// <param name="totalPopulation"></param>
    /// <param name="totalWeights"></param>
    public void GenerateNewPopulation(int totalPopulation)
    {
        //clears all of the population
        ClearPopulation();
        _population.Capacity = totalPopulation;

        _genomeID = 0;
        _generation = 1;

        int count = totalPopulation - _population.Count;

        //Randomizes all of the weights for each individual member.
        for (int i = 0; i < count; i++)
        {
            _population.Add(CreateNewGenome());
        }

        _legend = new Genome();
    }

    /// <summary>
    /// Calculates the total amount of weights based on the hidden layer
    /// </summary>
    /// <returns></returns>
    void SetTotalWeights()
    {
        _totalWeights = 0;
        for (int i = 0; i < _networkStructure.Length - 1; i++)
        {
            int j = i + 1;
            _totalWeights += _networkStructure[i] * _networkStructure[j] + _networkStructure[j];
        }
    }

    /// <summary>
    /// Clears all of the population
    /// </summary>
    public void ClearPopulation()
    {
        _population.Clear();
    }

    /// <summary>
    /// Creates a new randomized Genome.
    /// </summary>
    /// <returns></returns>
    Genome CreateNewGenome()
    {
        Genome genome = new Genome();
        genome.id = _genomeID;
        genome.fitness = 0.0f;
        genome.weights = new List<float>(_totalWeights);

        //Initialize all of the weights to a random value between 0-1
        for (int i = 0; i < _totalWeights; i++)
        {//resize
            genome.weights.Add(RandomClamped());
        }

        _genomeID++;

        return genome;
    }


    /// <summary>
    /// Breeds the population.
    /// Makes 2 babies every time.
    /// the top 3 parents mate first. (0 -> 1, 2, 3)
    /// The 2 parents mate. (1 -> 2, 3)
    /// Rest gets random genome
    /// </summary>
    public void BreedPopulation()
    {
        List<Genome> bestGenomes = new List<Genome>(ConstHolder.MAX_TOP_GENOMES);

        ElitismSelection(ConstHolder.MAX_TOP_GENOMES, ref bestGenomes);
        //RouletteSelection(ConstHolder.MAX_TOP_GENOMES, ref bestGenomes);

        if (bestGenomes.Count != 0)
        {
            //tries to assign a new legend.
            AssignLegend(ref bestGenomes);

            bool containsLegend = false;
            
            List<Genome> children = new List<Genome>(_population.Count);

            //breeds all of the parents together.
            BreedParents(ref bestGenomes, ref children, ref containsLegend);

            if (!containsLegend)
            {//if it doesn't contain a legend
                //add it into the population.
                SetupLegend(ref children);
            }

            //For the remainding n population, add some random
            int remainingChildren = (_population.Count - children.Count);
            for (int i = 0; i < remainingChildren; i++)
            {
                children.Add(CreateNewGenome());
            }

            ClearPopulation();
            _population = children;
        }
        
        _generation++;
    }

    /// <summary>
    /// Elitism Selection.
    /// Selects the top fit members
    /// </summary>
    /// <param name="totalGenomes"></param>
    /// <param name="output"></param>
    void ElitismSelection(int totalGenomes, ref List<Genome> output)
    {
        int sizeLeft = totalGenomes - output.Count;
        for (int i = 0; i < sizeLeft; i++)
        {
            //Find the best cases for cross breeding based on fitness score
            float bestFitness = 0;
            int bestIndex = -1;


            for (int j = 0; j < _population.Count; j++)
            {//if fitness of this genome is higher than the best fitness so far
                if (population[j].fitness >= bestFitness)
                {
                    bool isUsed = false;

                    //check that the genome isn't already in the list
                    for (int k = 0; k < output.Count; k++)
                    {
                        if (output[k].id == population[j].id)
                        {
                            isUsed = true;
                        }
                    }

                    //if its not in the list, set values to this
                    if (!isUsed)
                    {
                        bestIndex = j;
                        bestFitness = population[bestIndex].fitness;
                    }
                }
            }

            if (bestFitness == 0)
            {
                break;
            }

            //Check that a fit memeber has been found
            if (bestIndex != -1)
            {
                output.Add(population[bestIndex]);
            }
        }
    }

    /// <summary>
    /// Roulette selection.
    /// Picks the top and random fitness members based on probability.
    /// </summary>
    /// <param name="totalGenomes"></param>
    /// <param name="output"></param>
    void RouletteSelection(int totalGenomes, ref List<Genome> output)
    {
        //add the best genome
        if (output.Count == 0)
        {
            output.Add(GetBestGenome());
        }

        float fitnessSum = 0;
        //sum all of the fitness
        for (int i = 0; i < _population.Count; i++)
        {
            fitnessSum += _population[i].fitness;
        }

        int safeGuard = 0;
        //loop until enough cases obtained.
        while (output.Count != totalGenomes)
        {
            // Get a floating point number in the interval 0.0 ... sumFitness
            float randomNumber = Random.Range(0.0f, 1.1f) * fitnessSum;

            float partialSum = 0.0f;
            int index = 0;

            //find a genome based on propability
            for (int i = 0; i < _population.Count; i++)
            {
                if (randomNumber < partialSum)
                {
                    index = i;
                    break;
                }
                else
                {
                    partialSum += _population[i].fitness;
                }
            }

            //if its not inside, add it.
            if (SafetyChecks(ref output, index))
            {
                output.Add(_population[index]);
            }

            if (safeGuard == totalGenomes * 4)
            {
                ElitismSelection(totalGenomes, ref output);
                break;
            }
            else
            {
                safeGuard++;
            }
        }

        output.Sort(CompareFitness);
    }

    /// <summary>
    /// Checks that a none fit member is being added.
    /// Checks that the member isn't already in the list.
    /// </summary>
    /// <param name="output"></param>
    /// <param name="index"></param>
    /// <returns></returns>
    bool SafetyChecks(ref List<Genome> output, int index)
    {
        bool goodToGo = true;

        if (_population[index].fitness != 0)
        {
            //check that the genome isn't already in the list.
            for (int i = 0; i < output.Count; i++)
            {
                if (output[i].id == _population[index].id)
                {
                    goodToGo = false;
                    break;
                }
            }
        }
        else
        {
            goodToGo = false;
        }

        return goodToGo;
    }

    /// <summary>
    /// Checks if the most fit member of the current
    /// generation is fitter than the legend.
    /// if so, he is the new legend.
    /// </summary>
    /// <param name="bestGenomes"></param>
    void AssignLegend(ref List<Genome> bestGenomes)
    {
        if (bestGenomes[0].fitness > _legend.fitness)
        {
            _legend.id = bestGenomes[0].id;
            _legend.fitness = bestGenomes[0].fitness;
            _legend.weights = bestGenomes[0].weights;
        }
    }

    /// <summary>
    /// Creates babies from the children
    /// </summary>
    /// <param name="bestGenomes"></param>
    /// <param name="children"></param>
    /// <param name="containsLegend"></param>
    void BreedParents(ref List<Genome> bestGenomes, ref List<Genome> children, ref bool containsLegend)
    {
        //Child genomes
        Genome baby1 = new Genome();
        Genome baby2 = new Genome();

        for (int i = 0; i < bestGenomes.Count; i++)
        {
            //chance of not crossbreeding
            if (Random.Range(0.0f, 1.1f) > ConstHolder.CROSSOVER_CHANCE)
            {
                children.Add(bestGenomes[i]);

                if (children.Last().id == _legend.id)
                {
                    containsLegend = true;
                }
            }
            else
            {
                if (i < ConstHolder.MAX_ALPHAS)
                {
                    //Carry on the best
                    children.Add(bestGenomes[i]);
                    //check if its a legend
                    if (children.Last().id == _legend.id)
                    {
                        containsLegend = true;
                    }
                }

                //Crossbreed
                for (int j = i + 1; j < bestGenomes.Count; j++)
                {
                    CrossBreed(bestGenomes[i], bestGenomes[j], ref baby1, ref baby2);
                    Mutate(baby1);
                    Mutate(baby2);
                    children.Add(baby1);
                    children.Add(baby2);
                }
            }
        }
    }

    static int CompareFitness(Genome x, Genome y)
    {
        return y.fitness.CompareTo(x.fitness);
    }

    /// <summary>
    /// Setups a legend for deployment.
    /// </summary>
    /// <param name="children"></param>
    void SetupLegend(ref List<Genome> children)
    {
        if (children[0].fitness < _legend.fitness * ConstHolder.LEGEND_SPAWN_CHANCE)
        {
            Debug.Log("Adding hero fitness: " + _legend.fitness);

            Genome legend = new Genome();
            legend.id = _legend.id;
            legend.weights = _legend.weights;

            //Mutate(legend);

            children.Add(legend);
            _timesLegendUsed++;
        }
    }

    /// <summary>
    /// Crosses the genome of 2 parents and creates 2 new babies.
    /// </summary>
    /// <param name="g1">Parent 1</param>
    /// <param name="g2">Parent 2</param>
    /// <param name="baby1"></param>
    /// <param name="baby2"></param>
    void CrossBreed(Genome g1, Genome g2, ref Genome baby1, ref Genome baby2)
    {
        int crossover = (int)Random.Range(0, _totalWeights - 1);

        baby1 = CreateGenomeWithoutWeights();
        baby2 = CreateGenomeWithoutWeights();

        //Go from start to crossover point, copying the weights from g1
        for (int i = 0; i < crossover; i++)
        {
            baby1.weights[i] = g1.weights[i];
            baby2.weights[i] = g2.weights[i];
        }

        for (int i = crossover; i < _totalWeights; i++)
        {
            baby1.weights[i] = g2.weights[i];
            baby2.weights[i] = g1.weights[i];
        }
    }
    Genome CreateGenomeWithoutWeights()
    {
        Genome baby = new Genome();

        baby.id = _genomeID;
        baby.weights = new List<float>(_totalWeights);

        for (int i = 0; i < _totalWeights; i++)
        {
            baby.weights.Add(0.0f);
        }

        _genomeID++;

        return baby;
    }

    /// <summary>
    /// Has a chance of mutating a weight.
    /// </summary>
    /// <param name="genome"></param>
    void Mutate(Genome genome)
    {
        for (int i = 0; i < genome.weights.Count; i++)
        {
            if (Random.Range(0.00000f, 1.00001f) < ConstHolder.MUTATION_RATE)
            {
                genome.weights[i] += RandomClamped() * ConstHolder.MAX_PERPETUATION;
            }
        }
    }


    /// <summary>
    /// Creates a random float.
    /// </summary>
    /// <returns></returns>
    static public float RandomFloat()
    {
        float rand = (float)Random.Range(0.0f, 32767.0f);
        return rand / 32767.0f + 1.0f;
    }

    /// <summary>
    /// Random float - Random float
    /// </summary>
    /// <returns></returns>
    static public float RandomClamped()
    {
        return RandomFloat() - RandomFloat();
    }
    
    /// <summary>
    /// Takes in the given data and if its not empty,
    /// It generated a new genome with the data from the weights.
    /// Otherwise it creates a new random genome
    /// </summary>
    /// <param name="data"></param>
    /// <returns></returns>
    public Genome CreateGenomeFromData(JsonWriterReader.GenomeData data)
    {
        if (data.weights.Count != 0)
        {
            Genome bestGenome = CreateGenomeWithoutWeights();

            bestGenome.weights = data.weights;

            return bestGenome;
        }
        else
        {
            return CreateNewGenome();
        }
    }

    /// <summary>
    /// Gets the best genome based on the fitness.
    /// </summary>
    /// <returns></returns>
    public Genome GetBestGenome()
    {
        int bestGenome = 0;
        float fitness = _population[0].fitness;

        for (int i = 1; i < _population.Count; i++)
        {
            if (_population[i].fitness > fitness)
            {
                fitness = _population[i].fitness;
                bestGenome = i;
            }
        }

        return _population[bestGenome];
    }

    /// <summary>
    /// Gets the worst genome based on the fitness.
    /// </summary>
    /// <returns></returns>
    public Genome GetWorstGenome()
    {
        int worstGenome = 0;
        float fitness = _population[0].fitness;

        for (int i = 1; i < _population.Count; i++)
        {
            if (_population[i].fitness < fitness)
            {
                fitness = _population[i].fitness;
                worstGenome = i;
            }
        }

        return _population[worstGenome];
    }

    /// <summary>
    /// Gets a genome based on the index.
    /// </summary>
    /// <param name="index"></param>
    /// <returns></returns>
    public Genome GetGenome(int index)
    {
        return _population[index];
    }

    /// <summary>
    /// Sets a genome at an index
    /// </summary>
    public void SetGenome(Genome newGenome, int index)
    {
        _population[index] = newGenome;
    }
    /// <summary>
    /// Sets fitness to a genome by index
    /// </summary>
    /// <param name="fitness"></param>
    /// <param name="index"></param>
    public void SetGenomeFitness(float fitness, int index)
    {
        _population[index].fitness = fitness;
    }

    /// <summary>
    /// Tries to load a new genome out of the data
    /// </summary>
    /// <param name="data"></param>
    public void LoadGenome(ref JsonWriterReader.GenomeData data)
    {
        if (data.weights.Count != 0)
        {
            Genome bestGenome = CreateGenomeWithoutWeights();

            bestGenome.weights = data.weights;

            if (_population.Count > 0)
            {//if there are already memebers, find the worst one to replace
                int worstGenome = 0;
                float fitness = _population[0].fitness;

                for (int i = 1; i < _population.Count; i++)
                {
                    if (_population[i].fitness < fitness)
                    {
                        fitness = _population[i].fitness;
                        worstGenome = i;
                    }
                }

                _population[worstGenome].id = bestGenome.id;
                _population[worstGenome].weights = bestGenome.weights;
            }
            else
            {//otherwise just add it into the population
                _population.Add(bestGenome);
            }
        }
    }

    public int totalWeights
    {
        get { return _totalWeights; }
    }

    public int generation
    {
        get { return _generation; }
    }

    public int timesLegendUsed
    {
        get { return _timesLegendUsed; }
    }

    public List<Genome> population
    {
        get { return _population; }
    }
}
