using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TD_MainMenu : MonoBehaviour
{
    [SerializeField] GameObject Continue;
    bool HasSaveData;

    // Start is called before the first frame update
    void Start()
    {
        HasSaveData = TD_GameSerializer.instance.SaveDataExists();
        Continue.GetComponent<Button>().interactable = HasSaveData;
    }

    private void OnEnable()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }

}
