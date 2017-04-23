using UnityEngine;
using System.Collections.Generic;

public class DataManager : MonoBehaviour 
{
	public enum GameMode
	{
		FreeForAll,
        AI_vs_AI,
		NeuralNetworkTraining,
	}

    /// <summary>
    /// Game mode selected
    /// </summary>
    static public GameMode      gameMode;

    /// <summary>
    /// number of obstacles to currently spawn up to
    /// </summary>
    static public float         currentSpeed = ConstHolder.UNIT_MIN_SPEED;

    /// <summary>
    /// number of obstacles to currently spawn up to
    /// </summary>
    static public int           maximumObstacles = ConstHolder.MIN_OBSTACLES;

    /// <summary>
    /// Initialization of the application upon creation
    /// </summary>
    void Awake()
	{
        Application.targetFrameRate = 60;
        Time.timeScale = 1.0f;
        /*
        const int TESTS = 2000;
        const int GRIDS = 10;
        List<JsonWriterReader.GameData> gameData = new List<JsonWriterReader.GameData>(TESTS);

        string path = "GameDataTest";

        for (int i = 0; i < TESTS; i++)
        {
            gameData.Add(JsonWriterReader.ReadJson<JsonWriterReader.GameData>(path + i + ".json"));
        }
        {
            int nnWins = 0;
            int aiWins = 0;
            int nnTotalPoints = 0;
            int aiTotalPoints = 0;
            int nnAveragePoints = 0;
            int aiAveragePoints = 0;
            for (int i = 0; i < TESTS; i++)
            {
                if (gameData[i].winner == UnitType.NeuralNetwork.ToString())
                {
                    nnWins++;
                }
                else if (gameData[i].winner == UnitType.ScriptedAI.ToString())
                {
                    aiWins++;
                }

                nnTotalPoints += gameData[i].nnTotalPoints;
                aiTotalPoints += gameData[i].aiTotalPoints;
                nnAveragePoints += gameData[i].nnAveragePoints;
                aiAveragePoints += gameData[i].aiAveragePoints;
            }

            Debug.Log("NN WINS: " + nnWins);
            Debug.Log("AI WINS: " + aiWins);

            Debug.Log("NN TOTAL POINTS: " + nnTotalPoints);
            Debug.Log("AI TOTAL POINTS: " + aiTotalPoints);

            Debug.Log("NN AVERAGE POINTS: " + nnAveragePoints);
            Debug.Log("AI AVERAGE POINTS: " + aiAveragePoints);
        }
        {
            int nnTotalPoints = 0;
            int aiTotalPoints = 0;
            int nnAveragePoints = 0;
            int aiAveragePoints = 0;

            int iteration = TESTS / GRIDS;
            for (int i = 0; i < GRIDS; i++)
            {
                Debug.Log("GRID: " + i);

                for (int j = 0; j < iteration; j++)
                {
                    nnTotalPoints += gameData[i].nnTotalPoints;
                    aiTotalPoints += gameData[i].aiTotalPoints;
                    nnAveragePoints += gameData[i].nnAveragePoints;
                    aiAveragePoints += gameData[i].aiAveragePoints;
                }

                Debug.Log("NN TOTAL POINTS: " + nnTotalPoints);
                Debug.Log("AI TOTAL POINTS: " + aiTotalPoints);

                Debug.Log("NN AVERAGE POINTS: " + nnAveragePoints);
                Debug.Log("AI AVERAGE POINTS: " + aiAveragePoints);
            }
        }*/
    }
}
