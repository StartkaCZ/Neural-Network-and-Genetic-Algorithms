using UnityEngine;
using System.Collections.Generic;

public class GameManager : MonoBehaviour
{
    enum RegionOrder
    {
        Bot,
        BotMid,
        Mid,
        TopMid,
        Top,

        Count
    }

    enum GameState
    {
        Starting,
        Playing,
        GameOver
    }

    Dictionary<RegionOrder, Region>     _regions;
    
    List<Unit>                          _units;

    List<ParticleSystem>                _particles;

    CameraLogic                         _camera;
    SpotLight[]                         _spotLights;
    EvolutionManager                    _evolutionManager;

    Vector2                             _regionSize;
    Vector2                             _halfRegionSize;

    float                               _gameTime;
    float                               _startingTime;

    int                                 _currentTarget;
    int                                 _currentRegion;
    int                                 _previousRegion;
    int                                 _testsRan;

    bool                                _checkForTransition;

    GameState                           _gameState;

    [SerializeField]
    TransitionAnimation                 _transitionPanel;
    [SerializeField]
    GameObject                          _gameOverPanel;
    [SerializeField]
    UnityEngine.UI.Text                 _playersText;
    [SerializeField]
    UnityEngine.UI.Text                 _scoresText;
    [SerializeField]
    UnityEngine.UI.Text                 _typePlayerText;
    [SerializeField]
    UnityEngine.UI.Text                 _countDownText;


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

    /// <summary>
    /// Intiialize the critical variables for the class.
    /// </summary>
    void Awake()
    {
        Application.targetFrameRate = 60;
        Time.timeScale = 1;

        _units = new List<Unit>(ConstHolder.MAX_AGENTS);

        _regions = new Dictionary<RegionOrder, Region>();

        _particles = new List<ParticleSystem>(ConstHolder.MAX_AGENTS);

        _camera = Camera.main.GetComponent<CameraLogic>();
        _evolutionManager = GetComponent<EvolutionManager>();

        _testsRan = 0;
        _checkForTransition = false;
        _transitionPanel.Initialize();
    }

    /// <summary>
    /// Creates all of entities here and initializes them.
    /// </summary>
    void Start()
    {
        PrefabFactory.Initialize();
        ParticleSystemFactory.Initialize();

        GenerateRegions();

        GeneratePlayers();

        SetupSpotLights();

        SetupCamera();

        AudioManager.instance.PlaySoundEffect(AudioManager.SoundEffect.RoundStart);
    }

    #region Setup

    /// <summary>
    /// Generates all of the player units.
    /// </summary>
    /// <param name="unitDataContainer"></param>
    void GeneratePlayers()
    {
        //creates a unit container - not needed for final build
        GameObject unitContainer = new GameObject();
        unitContainer.name = "UnitContainer";

        //create initial position and rotation
        Vector3 initialPosition = new Vector3(ConstHolder.INITIAL_POSITION_X, ConstHolder.INITIAL_POSITION_Y, ConstHolder.INITIAL_POSITION_Z);
        Vector3 initialEulerAngle = new Vector3(ConstHolder.INITIAL_ANGLE_X, ConstHolder.INITIAL_ANGLE_Y, ConstHolder.INITIAL_ANGLE_Z);

        int agentSize = ConstHolder.MAX_AGENTS;
        List<NN_Agent> agents = new List<NN_Agent>(agentSize);

        //create players based on the mode
        switch (DataManager.gameMode)
        {
            case DataManager.GameMode.FreeForAll:
                GeneratePlayerForFreeForALl(ref agents, ref unitContainer, initialPosition, initialEulerAngle, agentSize);
                break;

            case DataManager.GameMode.AI_vs_AI:
                GeneratePlayersForAIsBattle(ref agents, ref unitContainer, initialPosition, initialEulerAngle, agentSize);
                break;

            case DataManager.GameMode.NeuralNetworkTraining:
                GeneratePlayersForNeuralNetworkTraining(ref agents, ref unitContainer, initialPosition, initialEulerAngle, agentSize);
                break;

            default:
                Debug.Log("What is this mode? " + DataManager.gameMode);
                break;
        }

        //remove collision between the units
        IgnoreCollisionBetweenUnits();

        //initialize the evolution manager with agents
        _evolutionManager.Initialize(agents);
    }

    #region Player Generation

    /// <summary>
    /// sets up the player, scripted AIs and NN Agents
    /// </summary>
    /// <param name="agents"></param>
    /// <param name="unitContainer"></param>
    /// <param name="initialPosition"></param>
    /// <param name="initialEulerAngle"></param>
    /// <param name="agentSize"></param>
    void GeneratePlayerForFreeForALl(ref List<NN_Agent> agents, ref GameObject unitContainer, Vector3 initialPosition, Vector3 initialEulerAngle, int agentSize)
    {
        agentSize--;
        _units.Add(SetupUnit(UnitType.Player, ref unitContainer, initialPosition, initialEulerAngle));
        initialPosition.x += ConstHolder.SPACING_BETWEEN_AGENTS;

        agentSize = (int)(agentSize * 0.5f);

        for (int i = 0; i < agentSize; i++)
        {
            _units.Add(SetupUnit(UnitType.ScriptedAI, ref unitContainer, initialPosition, initialEulerAngle));
            initialPosition.x += ConstHolder.SPACING_BETWEEN_AGENTS;

            _units.Add(SetupUnit(UnitType.NeuralNetwork, ref unitContainer, initialPosition, initialEulerAngle));
            initialPosition.x += ConstHolder.SPACING_BETWEEN_AGENTS;

            agents.Add(_units[2 + (i * 2)].GetComponent<NN_Agent>());
        }
    }

    /// <summary>
    /// Sets up Scripted AIs and NN Agents
    /// </summary>
    /// <param name="agents"></param>
    /// <param name="unitContainer"></param>
    /// <param name="initialPosition"></param>
    /// <param name="initialEulerAngle"></param>
    /// <param name="agentSize"></param>
    void GeneratePlayersForAIsBattle(ref List<NN_Agent> agents, ref GameObject unitContainer, Vector3 initialPosition, Vector3 initialEulerAngle, int agentSize)
    {
        agentSize = (int)(agentSize * 0.5f);

        if (agentSize % 2 != 0)
        {
            initialPosition.x += ConstHolder.SPACING_BETWEEN_AGENTS * 0.5f;
        }

        for (int i = 0; i < agentSize; i++)
        {
            _units.Add(SetupUnit(UnitType.ScriptedAI, ref unitContainer, initialPosition, initialEulerAngle));
            initialPosition.x += ConstHolder.SPACING_BETWEEN_AGENTS;

            _units.Add(SetupUnit(UnitType.NeuralNetwork, ref unitContainer, initialPosition, initialEulerAngle));
            initialPosition.x += ConstHolder.SPACING_BETWEEN_AGENTS;

            agents.Add(_units[1 + (i * 2)].GetComponent<NN_Agent>());
        }
    }

    /// <summary>
    /// Sets up NN Agents.
    /// </summary>
    /// <param name="agents"></param>
    /// <param name="unitContainer"></param>
    /// <param name="initialPosition"></param>
    /// <param name="initialEulerAngle"></param>
    /// <param name="agentSize"></param>
    void GeneratePlayersForNeuralNetworkTraining(ref List<NN_Agent> agents, ref GameObject unitContainer, Vector3 initialPosition, Vector3 initialEulerAngle, int agentSize)
    {
        for (int i = 0; i < agentSize; i++)
        {
            _units.Add(SetupUnit(UnitType.NeuralNetwork, ref unitContainer, initialPosition, initialEulerAngle));
            initialPosition.x += ConstHolder.SPACING_BETWEEN_AGENTS;

            agents.Add(_units[i].GetComponent<NN_Agent>());
        }
    }

    /// <summary>
    /// Removes collision betweem unit game objects.
    /// Sets their names to players.
    /// </summary>
    void IgnoreCollisionBetweenUnits()
    {
        for (int i = 0; i < _units.Count; i++)
        {
            _units[i].name = "Player" + i;

            for (int j =  i + 1; j < _units.Count; j++)
            {
                Physics.IgnoreCollision(_units[i].collider, _units[j].collider);
            }
        }
    }

    /// <summary>
    /// Creates a unit and sets it in the rotation and position required.
    /// Unit is initialized based on its type.
    /// </summary>
    /// <param name="unitDataContainer"></param>
    /// <param name="unitSize"> size of a unit</param>
    /// <param name="regionSize"></param>
    /// <param name="type"></param>
    /// <returns>game object</returns>
    Unit SetupUnit(UnitType type, ref GameObject container, Vector3 spawnPosition, Vector3 spawnEulerAngle)
    {
        Unit unit = PrefabFactory.CreateUnit(type);

        Vector2 unitSize = PrefabFactory.GetSize(PrefabFactory.Prefab.Unit);

        unit.transform.position = spawnPosition;
        unit.transform.eulerAngles = spawnEulerAngle;
        unit.transform.parent = container.transform;

        unit.Initialize(type);

        return unit;
    }

    #endregion

    /// <summary>
    /// Generates regions and restarts the level.
    /// </summary>
    void GenerateRegions()
    {
        RestartGame();

        //setup region size
        _regionSize = PrefabFactory.GetSize(PrefabFactory.Prefab.Region);
        _halfRegionSize = _regionSize * 0.5f;

        //create the first region and initialize it
        Region region = PrefabFactory.CreateGameObject(PrefabFactory.Prefab.Region).GetComponent<Region>();
        region.Initialize(_halfRegionSize);
        _regions.Add(RegionOrder.Bot, region);

        //set the z position to the next position to connect the regions.
        float z = _regions[RegionOrder.Bot].position.z + _regionSize.y;

        int extraRegions = (int)RegionOrder.Count;
        for (int i = 1; i < extraRegions; i++)
        {//create each individual region, set its position and initialize it.
            GameObject regionClone = PrefabFactory.CreateGameObject(PrefabFactory.Prefab.Region);

            Vector3 position = regionClone.transform.position;
            position.z = z;

            regionClone.transform.position = position;

            region = regionClone.GetComponent<Region>();
            region.Initialize(_halfRegionSize);

            _regions.Add((RegionOrder) i, region);

            z += _regionSize.y;
        }
    }

    /// <summary>
    /// Restarts the game, and initialized all of the variables.
    /// </summary>
    void RestartGame()
    {
        _gameTime = 0;
        _currentRegion = 0;
        _previousRegion = 0;
        _currentTarget = 0;

        _gameOverPanel.gameObject.SetActive(false);
        _countDownText.gameObject.SetActive(true);

        _startingTime = ConstHolder.TIME_TILL_GAME_STARTS;
        _countDownText.text = "" + (int)_startingTime;

        _gameState = GameState.Starting;

        DataManager.currentSpeed = ConstHolder.UNIT_MIN_SPEED;
        DataManager.maximumObstacles = ConstHolder.MIN_OBSTACLES;
    }

    /// <summary>
    /// Finds all of the spotlighta from the scene.
    /// Sets them up to track the first player.
    /// </summary>
    void SetupSpotLights()
    {
        GameObject[] spotLights = GameObject.FindGameObjectsWithTag("SpotLight");
        _spotLights = new SpotLight[spotLights.Length];

        for (int i = 0; i < spotLights.Length; i++)
        {
            _spotLights[i] = spotLights[i].GetComponent<SpotLight>();

            _spotLights[i].Initialize(_units[0].transform);
            _spotLights[i].SetCurrentColour(_units[0].colour);
        }
    }

    /// <summary>
    /// Sets up the camera so that it points at the player.
    /// </summary>
    void SetupCamera()
    {
        _camera.Initialize(_units[0].transform);
    }

    #endregion

    /// <summary>
    /// Updates the class.
    /// </summary>
    void Update()
    {
        switch (_gameState)
        {
            case GameState.Starting:
                UpdateStarting();
                break;
            case GameState.Playing:
                UpdatePlaying();
                break;
            case GameState.GameOver:
                UpdateGameOver();
                break;
        }
    }

    /// <summary>
    /// Updates the state before the game is played.
    /// Updates the countdown timer and fade in animation.
    /// </summary>
    void UpdateStarting()
    {
        _transitionPanel.ManualUpdate();

        if (!_transitionPanel.playingAnimation)
        {//if the transition has finished playing
            if (_startingTime > 0)
            {//and countdown is still going
                _startingTime -= Time.deltaTime;

                if (_startingTime < 0)
                {//if coundown finished
                    _startingTime = 0;
                    _countDownText.gameObject.SetActive(false);
                }

                _countDownText.text = "" + (int)_startingTime;
            }
            else
            {//change the state to playing
                _gameState = GameState.Playing;
            }

            //update input.
            CheckForInput();
        }
    }

    /// <summary>
    /// Updates all of the game objects.
    /// Updates the game play.
    /// </summary>
    void UpdatePlaying()
    {
        _camera.ManualUpdate();

        UpdateDifficulty();
        UpdateSpotLights();
        UpdateRegions();
        UpdateUnits();
        UpdateParticleSystems();
        UpdateEvolutionManager();

        CheckForInput();
    }

    /// <summary>
    /// Updates game over screen.
    /// </summary>
    void UpdateGameOver()
    {
        _transitionPanel.ManualUpdate();

        if (_checkForTransition)
        {//if scene is being changed
            if (!_transitionPanel.playingAnimation)
            {//check if transition finished to switch the scene
                SceneLoader.LoadTargetScene();
            }
        }
        else
        {//updates input.
            CheckForInput();
        }
    }


    #region Updates
    
    /// <summary>
    /// Updates the difficulty of the game.
    /// </summary>
    void UpdateDifficulty()
    {
        if (_gameTime < ConstHolder.TIME_TILL_MAX_DIFFICULTY)
        {//if the maximum difficulty timer hasn't been reached
            _gameTime += Time.deltaTime;

            float difficultyScale = _gameTime / ConstHolder.TIME_TILL_MAX_DIFFICULTY;

            if (difficultyScale > 1)
            {//if the difficulty scale passed the limit
                difficultyScale = 1;
            }

            float newSpeed = ConstHolder.UNIT_MAX_SPEED * difficultyScale;

            if (newSpeed > ConstHolder.UNIT_MIN_SPEED)
            {//change the timescale.
                DataManager.currentSpeed = newSpeed;
            }

            //check how many new obstacles are we to spawn
            int newMaxObstacles = (int)(ConstHolder.MAX_OBSTACLES * difficultyScale);

            if (newMaxObstacles > 1)
            {//if its more than one, set it
                DataManager.maximumObstacles = newMaxObstacles;
            }
        }
        else
        {//increment the game time.
            _gameTime += Time.deltaTime;
        }
    }

    /// <summary>
    /// Updates all of the spot lights.
    /// </summary>
    void UpdateSpotLights()
    {
        for (int i = 0; i < _spotLights.Length; i++)
        {
            _spotLights[i].ManualUpdate();
        }
    }
    
    /// <summary>
    /// Updates the evolution manaager.
    /// </summary>
    void UpdateEvolutionManager()
    {
        if (DataManager.gameMode == DataManager.GameMode.NeuralNetworkTraining)
        {//if the game mode is NN training
            _evolutionManager.ManualUpdate();

            if (_evolutionManager.newGeneration)
            {//if we are spawning a new generation
                ResetGame();

                _evolutionManager.newGeneration = false;
            }
        }
    }

    /// <summary>
    /// Updates all of the regions.
    /// </summary>
    void UpdateRegions()
    {
        Vector3 cameraPosition = _camera.position;
        _currentRegion = (int)(cameraPosition.z / _regionSize.y);

        if (_currentRegion > _previousRegion)
        {//if we have moved up a region.
            //destroy the bottom regions
            Destroy(_regions[RegionOrder.Bot].gameObject);

            int size = _regions.Count - 1;
            for (int i = 0; i < size; i++)
            {//shift all of the regions down
                _regions[(RegionOrder)i] = _regions[(RegionOrder)i + 1];
            }

            //create and intiialize a new regions for the top.
            GameObject regionClone = PrefabFactory.CreateGameObject(PrefabFactory.Prefab.Region);

            Vector3 position = regionClone.transform.position;
            position.z = _regions[(RegionOrder)((int)RegionOrder.Top-1)].position.z + _regionSize.y;

            regionClone.transform.position = position;

            Region region = regionClone.GetComponent<Region>();
            region.Setup(_halfRegionSize);

            _regions[RegionOrder.Top] = region;
        }

        _previousRegion = _currentRegion;
    }

    /// <summary>
    /// Updates all of the units
    /// </summary>
    void UpdateUnits()
    {
        int topUnit = 0;
        int index = 0;
        bool allDead = true;

        for (int i = 0; i < _units.Count; i++)
        {//update each unit
            UpdateUnit(_units[i], ref topUnit, ref index, ref allDead, i);
        }

        //try to set the new top unit to be tracked.
        TrackTopUnit(index, topUnit);

        //set the target of the camera to the current target.
        _camera.SetTarget(_units[_currentTarget].transform);

        for (int i = 0; i < _spotLights.Length; i++)
        {//set each spotlight to point at its target.
            _spotLights[i].SetTarget(_units[_currentTarget].transform);
            _spotLights[i].SetTargetColour(_units[_currentTarget].colour);
        }

        if (allDead)
        {//if all units are dead, set game over.
            SetupGameOver();
        }
    }

    private static int ComparePoints(Unit x, Unit y)
    {//compare function to sort the unit list in order of points.
        return y.points.CompareTo(x.points);
    }

    /// <summary>
    /// updates individul unit.
    /// </summary>
    /// <param name="unit"></param>
    void UpdateUnit(Unit unit, ref int topUnit, ref int index, ref bool allDead, int i)
    {
        if (unit.alive)
        {
            allDead = false;
            unit.ManualUpdate();

            int zPosition = (int)unit.position.z;
            if (zPosition > topUnit)
            {
                topUnit = zPosition;
                index = i;
            }
        }
        else
        {
            if (unit.hit)
            {
                ParticleSystem particleSystem = ParticleSystemFactory.CreateParticleEffectSystem(ParticleSystemFactory.ParticleEffect.Death, unit.colour);
                particleSystem.transform.position = new Vector3(unit.position.x, particleSystem.transform.position.y, unit.position.z);
                _particles.Add(particleSystem);

                unit.hit = false;
            }
        }
    }

    /// <summary>
    /// Sets the new target to be tracked.
    /// </summary>
    /// <param name="index"></param>
    /// <param name="topUnit"></param>
    void TrackTopUnit(int index, float topUnit)
    {
        if (DataManager.gameMode != DataManager.GameMode.FreeForAll || !_units[_currentTarget].alive)
        {//if its not a free for all game or current target is dead
            if (_currentTarget != index)
            {//if current target is not currently the top unit
                if (_units[_currentTarget].alive)
                {//check if the current target is alive
                    if (_units[_currentTarget].position.z < topUnit + 0.5f)
                    {//check that is has passed at least half a unit above the current target
                        //set teh new target
                        _currentTarget = index;
                    }
                }
                else
                {//set the new target
                    _currentTarget = index;
                }
            }
        }
    }

    /// <summary>
    /// sets up game over screen.
    /// </summary>
    void SetupGameOver()
    {
        _gameState = GameState.GameOver;
        //sorts the list of units.
        _units.Sort(ComparePoints);

        _gameOverPanel.SetActive(true);

        //sets up all of the texts.
        _playersText.text = "PLAYERS\n\n";
        _scoresText.text = "POINTS\n\n";
        _typePlayerText.text = "TYPE\n\n";

        for (int i = 0; i < _units.Count; i++)
        {
            _playersText.text += "" + _units[i].name + "\n";
            _scoresText.text += "" + _units[i].points + "\n";

            switch (_units[i].type)
            {
                case UnitType.NeuralNetwork:
                    _typePlayerText.text += "NN Agent\n";
                    break;
                case UnitType.ScriptedAI:
                    _typePlayerText.text += "Scripted AI\n";
                    break;
                case UnitType.Player:
                    _typePlayerText.text += "Player\n";
                    break;
            }
        }

        if (DataManager.gameMode == DataManager.GameMode.AI_vs_AI)
        {//if its an AI vs AI battle, it saved data.
            SaveAI_Data();
            ResetGame();
            _testsRan++;
        }
    }

    /// <summary>
    /// Resets the Regions, Units, Camera and Spotlights.
    /// </summary>
    void ResetGame()
    {
        for (int i = 0; i < _regions.Count; i++)
        {//destroy all regions
            Destroy(_regions[(RegionOrder)i].gameObject);
        }
        _regions.Clear();

        ///generate new ones and restart the game.
        GenerateRegions();

        for (int i = 0; i < _units.Count; i++)
        {//reset each unit.
            _units[i].Reset();
        }

        //reset teh camera
        _camera.ResetToTarget(_units[0].transform);

        for (int i = 0; i < _spotLights.Length; i++)
        {//reset each spotlight.
            _spotLights[i].ResetToTarget(_units[0].transform);
            _spotLights[i].SetCurrentColour(_units[0].colour);
        }
    }

    /// <summary>
    /// Saves the AI Data collected at the end of each run.
    /// </summary>
    void SaveAI_Data()
    {
        List<JsonWriterReader.AI_Data> aiData = new List<JsonWriterReader.AI_Data>(_units.Count);

        int nnAveragePoints = 0;
        int aiAveragePoints = 0;
        int nnTotalPoints = 0;
        int aiTotalPoints = 0;

        for (int i = 0; i < _units.Count; i++)
        {
            aiData.Add(new JsonWriterReader.AI_Data(_units[i].type.ToString(), _units[i].points));

            if (_units[i].type == UnitType.NeuralNetwork)
            {
                nnTotalPoints += _units[i].points;
            }
            else
            {
                aiTotalPoints += _units[i].points;
            }
        }

        int halfPopulation = (int)(_units.Count * 0.5f);
        nnAveragePoints = nnTotalPoints / halfPopulation;
        aiAveragePoints = aiTotalPoints / halfPopulation;

        JsonWriterReader.GameData gameData = new JsonWriterReader.GameData(aiData, _units[0].type.ToString(), nnAveragePoints, aiAveragePoints, nnTotalPoints, aiTotalPoints);

        string path = "GameDataTest" + _testsRan;

        JsonWriterReader.WriteJson(path + ".json", ref gameData);
    }

    /// <summary>
    /// Updates Particle systems
    /// </summary>
    void UpdateParticleSystems()
    {
        for (int i = 0; i < _particles.Count; i++)
        {
            if (!_particles[i].IsAlive())
            {
                Destroy(_particles[i].gameObject);
                _particles.RemoveAt(i);
                i--;
            }
        }
    }

    #endregion

    /// <summary>
    /// Checks for input.
    /// </summary>
    void CheckForInput()
    {
        if (Input.anyKeyDown)
        {
            const float TIME_SCALE_MODIFIER = 1.0f;

            if (Input.GetKeyDown(KeyCode.Keypad0))
            {
                Time.timeScale = 0;
            }
            else if (Input.GetKeyDown(KeyCode.Keypad1))
            {
                Time.timeScale = 1;
            }
            else if (Input.GetKeyDown(KeyCode.Keypad2))
            {
                Time.timeScale = 2;
            }
            else if (Input.GetKeyDown(KeyCode.Keypad3))
            {
                Time.timeScale = 5;
            }
            else if (Input.GetKeyDown(KeyCode.Keypad5))
            {
                Time.timeScale = 10;
            }
            else if (Input.GetKeyDown(KeyCode.Keypad6))
            {
                if (Time.timeScale + TIME_SCALE_MODIFIER <= 100)
                {
                    Time.timeScale += TIME_SCALE_MODIFIER;
                }
            }
            else if (Input.GetKeyDown(KeyCode.Keypad4))
            {
                if (Time.timeScale - TIME_SCALE_MODIFIER >= 0)
                {
                    Time.timeScale -= TIME_SCALE_MODIFIER;
                }
            }
            else if(Input.GetKeyDown(KeyCode.Escape))
            {//Go to menu.
                Time.timeScale = 1;
                SceneLoader.targetScene = SceneLoader.Scene.Menu;
                SceneLoader.LoadTargetScene();
            }
        }

        if (Input.mouseScrollDelta.y != 0)
        {//Camera Scrolling.
            if (Input.mouseScrollDelta.y == 1)
            {
                _camera.Zoom(true);
            }
            else
            {
                _camera.Zoom(false);
            }
        }
    }


    /// <summary>
    /// Plays a sound effect for clicking a button
    /// </summary>
    public void PlaySoundEffect()
    {
        AudioManager.instance.PlaySoundEffect(AudioManager.SoundEffect.ButtonClick);
    }


    /// <summary>
    /// Free for all game mode has been selected
    /// </summary>
    public void Play()
    {
        ChangeToGameScene(SceneLoader.Scene.Game);
    }

    /// <summary>
    /// AI vs AI game mode has been selected.
    /// </summary>
    public void MainMenu()
    {
        ChangeToGameScene(SceneLoader.Scene.Menu);
    }

    /// <summary>
    /// setups everything there is to switch scene.
    /// </summary>
    void ChangeToGameScene(SceneLoader.Scene sceneToGoTo)
    {
        _transitionPanel.FadeIn();
        SceneLoader.targetScene = sceneToGoTo;
        _checkForTransition = true;

        PlaySoundEffect();
    }
}
