using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    public static SceneLoader instance;

    [SerializeField]
    string nextSceneName = "TowerDefenseFreshMap";
    bool shouldTransition = false;
    Coroutine corLoad;

    public enum GameScene
    {
        MainMenu,
        Level1,
        Settings
    }

    private string SceneToName(GameScene gameScene)
    {
        string sceneName = "";
        switch (gameScene) {
            case GameScene.MainMenu:
            sceneName = "StartMenu";
            break;

            case GameScene.Level1:
            sceneName = "TowerDefenseFreshMap";
            break;

            default:
            sceneName = "StartMenu";
            break;
        }

        return sceneName;
    }

    private void Start()
    {
        if (instance != null) Destroy(this);
        instance = this;

        //DontDestroyOnLoad(this);

        DontDestroyOnLoad(gameObject);
    }

    void Update()
    {
        //// Press the space key to start coroutine
        //if (Input.GetKeyDown(KeyCode.Space))
        //{
        //    // Use a coroutine to load the Scene in the background
        //    StartCoroutine(LoadYourAsyncScene());
        //}
        //if (shouldTransition)
        //{
        //    StartCoroutine();
        //}
    }

    private void OnGUI()
    {
        //int xCenter = (Screen.width / 2);
        //int yCenter = (Screen.height / 2);
        //int width = 400;
        //int height = 120;

        //GUIStyle fontSize = new GUIStyle(GUI.skin.GetStyle("button"));
        //fontSize.fontSize = 32;

        //Scene scene = SceneManager.GetActiveScene();

        //if (scene.name == "scene1")
        //{
        //    // Show a button to allow scene2 to be switched to.
        //    if (GUI.Button(new Rect(xCenter - width / 2, yCenter - height / 2, width, height), "Load second scene", fontSize))
        //    {
        //        SceneManager.LoadScene("scene2");
        //    }
        //}
        //else
        //{
        //    // Show a button to allow scene1 to be returned to.
        //    if (GUI.Button(new Rect(xCenter - width / 2, yCenter - height / 2, width, height), "Return to first scene", fontSize))
        //    {
        //        SceneManager.LoadScene("scene1");
        //    }
        //}
    }

    public void SetNextScene(GameScene gameScene)
    {
        nextSceneName = SceneToName(gameScene);
        LoadNextScene();
    }

    public void SetNextScene(string sceneName) 
    {
        nextSceneName = sceneName;
        //if (loadNow) LoadNextScene();
        LoadNextScene();
    }

    public void LoadNextScene()
    {
        if (corLoad != null) StopCoroutine(corLoad);
        corLoad = StartCoroutine(LoadYourAsyncScene());
    }

    IEnumerator LoadYourAsyncScene()
    {
        // The Application loads the Scene in the background as the current Scene runs.
        // This is particularly good for creating loading screens.
        // You could also load the Scene by using sceneBuildIndex. In this case Scene2 has
        // a sceneBuildIndex of 1 as shown in Build Settings.

        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(nextSceneName);

        // Wait until the asynchronous scene fully loads
        while (!asyncLoad.isDone)
        {
            yield return null;
        }
    }

    public void PlayerExit()
    {
        Application.Quit();
    }
}