using UnityEngine.SceneManagement;


/// <summary>
/// Scene laoder, used to load scenes.
/// </summary>
public class SceneLoader 
{
    public enum Scene
	{
		Scene0,
		Menu,
		Game,
	}


    public static Scene currentScene = Scene.Scene0;
    public static Scene targetScene = currentScene;


    /// <summary>
    /// Loads a target scene.
    /// </summary>
    static public void LoadTargetScene()
    {
        switch (targetScene)
        {
            case Scene.Menu:
                GoToMenu();
                break;

            case Scene.Game:
                GoToGame();
                break;
        }

        currentScene = targetScene;
    }

    /// <summary>
    /// Goes straight to menu scene.
    /// </summary>
    static public void GoToMenu()
	{
		AudioManager.instance.PlayMusic(AudioManager.Music.Menu);
		SceneManager.LoadScene ((int)Scene.Menu);

        currentScene = Scene.Menu;
        targetScene = currentScene;
    }

    /// <summary>
    /// Goes straight to game scene.
    /// </summary>
    static public void GoToGame()
	{
		AudioManager.instance.PlayMusic(AudioManager.Music.Game);
		SceneManager.LoadScene ((int)Scene.Game);

        currentScene = Scene.Game;
        targetScene = currentScene;
    }

    /// <summary>
    /// Quits the game.
    /// </summary>
    static public void QuitGame()
	{
        UnityEngine.Application.Quit();
	}
}
