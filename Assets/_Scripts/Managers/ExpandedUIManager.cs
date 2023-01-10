using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// TODO
// There are other UI managers we could also expand off of if wanted different starts
public class ExpandedUIManager : MonoBehaviour
{

    [SerializeField]
    protected GameObject _objectSelectionUI;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        HandleInput();
    }

    void HandleInput()
    {
        // TODO: perhaps parameterize the controls
        if (Input.GetKeyDown(KeyCode.B)) {
            // Send into Build mode 
            _objectSelectionUI.SetActive(true);
        }
        if (Input.GetKeyDown(KeyCode.Escape))
        { 
            _objectSelectionUI.SetActive(false);
        }
    }
}
