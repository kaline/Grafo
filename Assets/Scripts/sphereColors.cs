using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class sphereColors : MonoBehaviour
{
        public Scatterplot scatterplot;

    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void UpdateSphereColors()
    {
        Color color = Color.white;
        color.a = 0.42f;
        // Loop through all scatterplot points
        foreach (ScatterplotDataPoint point in scatterplot.scatterplotPoints)
        {
            Debug.Log("point 1" + point.dataClass);

            // Check if the sphere has a different siglaPartido from PT
            if (point.dataClass != "PT")
            {
                Debug.Log("point " + point.dataClass);

                // Update the sphere color
                point.GetComponent<Renderer>().material.color = color;
                point.pointColor = color;
            }
        }
    }
}
