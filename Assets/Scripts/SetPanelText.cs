using UnityEngine;
using UnityEngine.UI;

public class SetPanelTextT : MonoBehaviour
{
    public GameObject panel;
    public Text panelText;
    public string textToSet = "Matriz de votações de 2022 " + "\n" + "554 deputados";

    // Start is called before the first frame update
    void Start()
    {
        // Get the Text component of the Panel GameObject
        panelText = panel.GetComponentInChildren<Text>();

        // Set the text of the Text component
        panelText.text = textToSet;
    }
}
