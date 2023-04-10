using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ScatterplotDataPoint : MonoBehaviour
{

    public string pointName;
    public string dataClass;

    public TMP_Text textLabel;
    public Color pointColor;

    public bool showTextLabel = false;


    private float colorDelta = 0.4f;
    public Vector3 position;

    public string urlFoto;
    public string siglaUf;
    public string gender;
    public Scatterplot scatterplot;




    public void Select()
    {
        Debug.Log("scatterplot: " + scatterplot);

        if (scatterplot != null)
        {
            scatterplot.selectedPoint = this;
            scatterplot.ConnectPoints();
 
        }
        else
        {
            Debug.Log("Scatterplot is null or has been destroyed");
            Debug.LogWarning("Scatterplot is null or has been destroyed");
        }
        this.GetComponent<Renderer>().material.color = new Color(pointColor.r + colorDelta, pointColor.g + colorDelta, pointColor.b + colorDelta, pointColor.a);


    }

    public void Unselect()
    {

    }


    public void Highlight()
    {
        Debug.Log("Textlabel ", textLabel);
        if (textLabel != null)
        {
            textLabel.enabled = true;
        }
        this.GetComponent<Renderer>().material.color = new Color(pointColor.r + colorDelta, pointColor.g + colorDelta, pointColor.b + colorDelta, pointColor.a);
        //Scatterplot scatterplot = this.GetComponent<Scatterplot>();
      

    }

    public void Unhighlight()
    {
        if (textLabel != null)
        {
            textLabel.enabled = false;
        }
        this.GetComponent<Renderer>().material.color = new Color(pointColor.r, pointColor.g, pointColor.b, pointColor.a);



    }







    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (textLabel != null && textLabel.TryGetComponent(out TextMeshPro tmp))
        
       

        if (textLabel.enabled)
            transform.LookAt(Camera.main.transform); // so that the text label, if visible, always faces the camera

    }
}
