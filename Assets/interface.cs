using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SetPanelText : MonoBehaviour
{
    public GameObject panel;
    public TMP_Text panelText;
    public string textToSet = "Matriz de votações de 2022 " + "\n" + "554 deputados";

    // Start is called before the first frame update
    void Start()
    {
        // Get the Text component of the Panel GameObject
        panelText = panel.GetComponentInChildren<TMP_Text>();

        // Set the text of the Text component
        panelText.text = textToSet;
    }
}
