using UnityEngine;
using System.Collections.Generic;
using System.IO;

public class JsonWriterReader
{
    /// <summary>
    /// Turns provided data into a json string.
    /// Saved it under the pathname.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="path"></param>
    /// <param name="classData"></param>
    public static void WriteJson<T>(string path, ref T classData)
    {
        string fullPath = Application.persistentDataPath + "/" + path;
        string data = JsonUtility.ToJson(classData);

        Debug.Log("WRITING...");
        Debug.Log("Path: " + fullPath);

        File.WriteAllText(fullPath, data);

        Debug.Log("WRITING FINISHED.");
    }

    /// <summary>
    /// Reads Json file under the path name.
    /// Returs a data based on the type set.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="path"></param>
    /// <returns></returns>
    public static T ReadJson<T>(string path)
    {
        string fullPath = Application.persistentDataPath + "/" + path;

        Debug.Log("Path: " + fullPath);
        Debug.Log("READING...");

        T classData = default(T);

        if (File.Exists(fullPath))
        {
            string data = File.ReadAllText(fullPath);

            classData = (T)JsonUtility.FromJson<T>(data);

            Debug.Log("READING FINISHED.");
        }
        else
        {
            Debug.Log("FILE DOES NOT EXIST!!!");
        }

        return classData;
    }


    [System.Serializable]
    public class GenomeData
    {
        public GenomeData(List<float> w)
        {
            weights = w;
        }

        public List<float> weights;
    }


    [System.Serializable]
    public class EvolutionData
    {
        public EvolutionData(List<ImprovementData> dt, int genomesAlive, int timesLegendUsed)
        {
            genomesLeftAlive = genomesAlive;
            dataTracked = dt;
            timesLegendSpawned = timesLegendUsed;
        }

        public int                      genomesLeftAlive;
        public int                      timesLegendSpawned;

        public List<ImprovementData>    dataTracked;
    }

    [System.Serializable]
    public class ImprovementData
    {
        public ImprovementData(float f, int g, int gsi)
        {
            fitness = f;
            generation = g;
            generationsSinceImprovement = gsi;
        }

        public int      generation;
        public int      generationsSinceImprovement;

        public float    fitness;
    }

    /* * * * * * * * * * * * * * * * *
     * DATA TO TRACK
     * * * * * * * * * * * * * * * * *
     * winner
     * NN_averagePoints
     * AI_averagePoints
     * NN_totalPoints
     * AI_totalPoints
     * * * * * * * * * * * * * * * * *
     * tyoe
     * points
     * player
     * * * * * * * * * * * * * * * * * */

    [System.Serializable]
    public class GameData
    {
        public GameData(List<AI_Data> dt, string w, int nnap, int aiap, int nntp, int aitp)
        {
            dataTracked = dt;

            winner = w;
            nnAveragePoints = nnap;
            aiAveragePoints = aiap;
            nnTotalPoints = nntp;
            aiTotalPoints = aitp;
        }

        public string           winner;

        public int              nnAveragePoints;
        public int              aiAveragePoints;
        public int              nnTotalPoints;
        public int              aiTotalPoints;


        public List<AI_Data>    dataTracked;
    }

    [System.Serializable]
    public class AI_Data
    {
        public AI_Data(string t, int p)
        {
            type = t;
            points = p;
        }

        public string   type;
        public int      points;
    }
}
