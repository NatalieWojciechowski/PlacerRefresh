using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TD_MainMenu : MonoBehaviour
{
    [SerializeField] GameObject ContinueButton;
    [SerializeField] SceneLoader SceneLoader;

    bool HasSaveData;

    // Start is called before the first frame update
    void Start()
    {
        UpdateContinueState();
    }

    private void OnEnable()
    {
        UpdateContinueState();
    }

    private void UpdateContinueState()
    {
        HasSaveData = TD_GameSerializer.SaveDataExists();
        ContinueButton.GetComponent<Button>().interactable = HasSaveData;
        // Clear DataButton
        ContinueButton.GetComponentInChildren<Button>().gameObject.SetActive(HasSaveData);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void LoadAndContinue()
    {
        if (!HasSaveData) return;
        if (TD_GameSerializer.LoadGame())
            SceneLoader?.SetNextScene(SceneLoader.GameScene.Level1);
    }

    public void ClearSaveData()
    {
        if (!HasSaveData) return;
        TD_GameSerializer.ResetData();
        UpdateContinueState();
    }
}
