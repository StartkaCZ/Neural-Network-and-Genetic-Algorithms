using UnityEngine;

public class MenuManager : MonoBehaviour
{
    bool                    _checkForTransition;

    [SerializeField]
    TransitionAnimation     _transitionPanel;


    /// <summary>
    /// Initialize when created
    /// </summary>
    void Awake()
    {
        _checkForTransition = false;
        _transitionPanel.Initialize();
    }


    /// <summary>
    /// Updates the class
    /// </summary>
    void Update()
    {
        _transitionPanel.ManualUpdate();

        if (_checkForTransition)
        {//if scene is being changed
            if (!_transitionPanel.playingAnimation)
            {//check if transition finished to switch the scene
                SceneLoader.LoadTargetScene();
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
    public void FreeForAllBattle()
    {
        SetupSelectedMode(DataManager.GameMode.FreeForAll);
        ChangeToGameScene();
    }

    /// <summary>
    /// AI vs AI game mode has been selected.
    /// </summary>
    public void ArtificialIntelligenceBattle()
    {
        SetupSelectedMode(DataManager.GameMode.AI_vs_AI);
        ChangeToGameScene();
    }

    /// <summary>
    /// Neural network training has been selected
    /// </summary>
    public void NeuralNetworkTraining()
    {
        SetupSelectedMode(DataManager.GameMode.NeuralNetworkTraining);
        ChangeToGameScene();
    }

    /// <summary>
    /// setups everything there is to switch scene.
    /// </summary>
    void ChangeToGameScene()
    {
        _transitionPanel.FadeIn();
        SceneLoader.targetScene = SceneLoader.Scene.Game;
        _checkForTransition = true;

        PlaySoundEffect();
    }


    /// <summary>
    /// setups the data manager for a specific mode
    /// </summary>
    /// <param name="mode">mode selected</param>
    void SetupSelectedMode(DataManager.GameMode mode)
    {
        switch (mode)
        {
            case DataManager.GameMode.FreeForAll:
                SetupForFreeForAll(mode);
                break;
            case DataManager.GameMode.AI_vs_AI:
                SetupArtificialIntelligenceBattle(mode);
                break;
            case DataManager.GameMode.NeuralNetworkTraining:
                SetupNeuralNetworkTraining(mode);
                break;

            default:
                break;
        }
    }

    /// <summary>
    /// setups data manager with the data for free for all battle
    /// </summary>
    /// <param name="mode">mode selected</param>
    void SetupForFreeForAll(DataManager.GameMode mode)
    {
        DataManager.gameMode = mode;
    }

    /// <summary>
    /// setups data manager with the data for AI vs AI battle
    /// </summary>
    /// <param name="mode">mode selected</param>
    void SetupArtificialIntelligenceBattle(DataManager.GameMode mode)
    {
        DataManager.gameMode = mode;
    }

    /// <summary>
    /// setups data manager with the data for neural network training
    /// </summary>
    /// <param name="mode">mode selected</param>
    void SetupNeuralNetworkTraining(DataManager.GameMode mode)
    {
        DataManager.gameMode = mode;
    }

    /// <summary>
    /// Exits the appication.
    /// </summary>
    public void Quit()
    {
        SceneLoader.QuitGame();
    }
}
