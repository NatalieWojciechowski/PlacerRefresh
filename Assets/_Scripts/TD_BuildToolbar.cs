using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TD_BuildToolbar : MonoBehaviour
{
    List<Button> barButtons = new();
    public GameObject buttonTemplatePrefab;
    public GameObject buttonContainer;

    // TODO: Configure presets/  different bulid toolbars

    private void OnDisable()
    {
        foreach (Button button in barButtons)
        {
            button.onClick.RemoveAllListeners();
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        if (!buttonContainer) buttonContainer = gameObject;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public List<Button> ToolbarButtons(List<TD_Building> tD_Buildings, bool clearPrevious = false)
    {
        if (barButtons.Count > 0) return barButtons;

        if (clearPrevious) barButtons.Clear();

        foreach (TD_Building building in tD_Buildings)
        {
            Button barButton = createToolbarButton().GetComponent<Button>();

            if (!barButton) continue;

            if (building && building.GetStats().RawBuildingData)
            {
                building.ConfigureButton(ref barButton);
                barButtons.Add(barButton);
            }
            else barButton.gameObject.SetActive(false);
        }
        return barButtons;
    }

    private GameObject createToolbarButton()
    {
        return Instantiate(buttonTemplatePrefab, buttonContainer.transform);
    }
}
