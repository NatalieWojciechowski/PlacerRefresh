using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TD_MainMenu : MonoBehaviour
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
        UpdateSaveDataExist();
        ContinueButton.onClick.AddListener(OnContinue);
        ClearDataButton.onClick.AddListener(OnClearData);
        NewGameButton.onClick.AddListener(OnNewGame);
        SettingsButton.onClick.AddListener(OnSetings);
        ExitButton.onClick.AddListener(OnPlayerExit);
    }

    private void OnDisable()
    {
        ContinueButton.onClick.RemoveListener(OnContinue);
        ClearDataButton.onClick.RemoveListener(OnClearData);
        NewGameButton.onClick.RemoveListener(OnNewGame);
        SettingsButton.onClick.RemoveListener(OnSetings);
        ExitButton.onClick.RemoveListener(OnPlayerExit);
    }

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

    private void UpdateSaveDataExist()
    {
        HasSaveData = TD_GameSerializer.SaveDataExists();
        ContinueButton.interactable = HasSaveData;
        // Clear DataButton
        ClearDataButton.gameObject.SetActive(HasSaveData);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void LoadAndContinue()
    {
        if (!HasSaveData) return;
        if (TD_GameSerializer.LoadGame())
            SceneLoader.instance.SetNextScene(SceneLoader.GameScene.Level1);
    }

    public void ClearSaveData()
    {
        if (!HasSaveData) return;
        TD_GameSerializer.ResetData();
        UpdateSaveDataExist();
    }
}
