using UnityEngine;
using UnityEngine.UI;

public class ChangeSphereColor : MonoBehaviour
{
    public GameObject sphere;
    public Button colorButton;
    public Color newColor;

    // Start is called before the first frame update
    void Start()
    {
        // Add a listener to the color button that calls the ChangeColor method
        colorButton.onClick.AddListener(ChangeColor);
    }

    // Method called when the color button is clicked
    void ChangeColor()
    {
        // Get the Renderer component of the sphere GameObject
        Renderer sphereRenderer = sphere.GetComponent<Renderer>();

        // Set the color of the sphere to the new color
        sphereRenderer.material.color = newColor;
    }
}
