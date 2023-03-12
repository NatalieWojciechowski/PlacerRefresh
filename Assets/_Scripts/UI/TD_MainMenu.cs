using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class TD_MainMenu : MonoBehaviour, I_RefreshOnSceneChange
{
    [SerializeField] Button ContinueButton;
    [SerializeField] Button ClearDataButton;
    [SerializeField] Button NewGameButton;
    [SerializeField] Button SettingsButton;
    [SerializeField] Button ExitButton;

    bool HasSaveData;

    // Start is called before the first frame update
    void Start()
    {
        UpdateSaveDataExist();
    }

    private void OnEnable()
    {
        ResetButtonInteractable();
        SceneManager.activeSceneChanged += OnSceneChange;
        SceneManager.sceneLoaded += OnSceneLoad;
        UpdateSaveDataExist();
        ContinueButton.onClick.AddListener(OnContinue);
        ClearDataButton.onClick.AddListener(OnClearData);
        NewGameButton.onClick.AddListener(OnNewGame);
        SettingsButton.onClick.AddListener(OnSetings);
        ExitButton.onClick.AddListener(OnPlayerExit);
    }

    private void ResetButtonInteractable()
    {
        ContinueButton.interactable = false;
        ClearDataButton.interactable = false;
        NewGameButton.interactable = true;
        SettingsButton.interactable = false;
        ExitButton.interactable = true;
    }

    private void OnDisable()
    {
        SceneManager.activeSceneChanged -= OnSceneChange;
        SceneManager.sceneLoaded -= OnSceneLoad;
        ContinueButton.onClick.RemoveListener(OnContinue);
        ClearDataButton.onClick.RemoveListener(OnClearData);
        NewGameButton.onClick.RemoveListener(OnNewGame);
        SettingsButton.onClick.RemoveListener(OnSetings);
        ExitButton.onClick.RemoveListener(OnPlayerExit);
    }

    #region ButtonEffects
    private void OnContinue()
    {
        LoadAndContinue();
    }
    private void OnClearData()
    {
        ClearSaveData();
    }
    private void OnNewGame()
    {
        SceneLoader.instance.SetNextScene(SceneLoader.GameScene.Level1);
    }

    private void OnSetings()
    {
        SceneLoader.instance.SetNextScene(SceneLoader.GameScene.Settings);
    }
    private void OnPlayerExit()
    {
        Application.Quit();
    }
    #endregion

    private void UpdateSaveDataExist()
    {
        HasSaveData = TD_GameSerializer.SaveDataExists();
        ContinueButton.interactable = HasSaveData;
        // Clear DataButton
        ClearDataButton.gameObject.SetActive(HasSaveData);
        ClearDataButton.interactable = HasSaveData;
    }

    // Update is called once per frame
    void Update()
    {

    }

    #region ButtonEffectHelpers

    void LoadAndContinue()
    {
        if (!HasSaveData) return;
        TD_GameManager.instance.useSaveData = true;
        //if (TD_GameSerializer.LoadGame())
        SceneLoader.instance.SetNextScene(SceneLoader.GameScene.Level1);
    }

    void ClearSaveData()
    {
        if (!HasSaveData) return;
        TD_GameSerializer.ResetData();
        UpdateSaveDataExist();
    }
    #endregion

    #region Interfaces
    public void OnSceneChange(Scene current, Scene next)
    {

    }

    public void ReInit()
    {
        TD_AudioManager.instance.PlayMusic(TD_AudioManager.instance.MainMenuMusic);
        UpdateSaveDataExist();
        ResetButtonInteractable();
    }

    public void OnSceneLoad(Scene current, LoadSceneMode loadSceneMode)
    {
        if (current.name == SceneLoader.SceneToName(SceneLoader.GameScene.MainMenu))
        {
            ReInit();
        }
    }
    #endregion
}
