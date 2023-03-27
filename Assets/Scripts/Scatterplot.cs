using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class Scatterplot : MonoBehaviour
{

    public GameObject pointPrefab;
    List<ScatterplotDataPoint> scatterplotPoints = null;

    public float connectDistance = 0.2f;

    private ScatterplotDataPoint selectedPoint = null;
    private bool isConnected = false;

    public string edge = "Edge";



    // Use this for initialization
    void Start()
    {
        LoadPoints("Data/deputados_valid_result");

        // Find all ScatterplotDataPoints in the scene
        ScatterplotDataPoint[] points = FindObjectsOfType<ScatterplotDataPoint>();
        foreach (ScatterplotDataPoint point in points)
        {
            scatterplotPoints.Add(point);
        }
    }




    void Update()
    {
        if (Input.GetMouseButtonDown(0)) // check for left mouse button click
        {            

            ConnectPoints();

            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit)) // check if mouse click hit a collider
            {
                ScatterplotDataPoint clickedPoint = hit.collider.GetComponent<ScatterplotDataPoint>();

                if (clickedPoint != null && selectedPoint == null) // if a point is clicked and no point is currently selected
                {
                    selectedPoint = clickedPoint; // select the clicked point
                }
                else if (clickedPoint != null && clickedPoint != selectedPoint) // if a point is clicked and another point is selected
                {
                    float distance = Vector3.Distance(clickedPoint.transform.position, selectedPoint.transform.position);

                    if (distance <= connectDistance) // if the distance between the points is less than the threshold
                    {

                        // deselect the points
                        selectedPoint = null;
                        clickedPoint = null;

                        isConnected = true;
                    }
                }
            }
        }

    }



    public void LoadPoints(string datasetPath)
    {
        //if(scatterplotPoints != null)
           // DumpPoints();

        scatterplotPoints = new List<ScatterplotDataPoint>();

        List<Dictionary<string, object>> csvData = CSVReader.Read(datasetPath);


        for (var i = 0; i < csvData.Count; i++)
        {
  
            float x = 0.05f * System.Convert.ToSingle(csvData[i]["x"]);

            float y = 0.05f * System.Convert.ToSingle(csvData[i]["y"]);

            float z = 0.05f * System.Convert.ToSingle(csvData[i]["z"]);
            
            ScatterplotDataPoint newDataPoint = Instantiate(pointPrefab, new Vector3(x*6, y*6, z*6), Quaternion.identity).
                GetComponent<ScatterplotDataPoint>();

            newDataPoint.transform.position += this.transform.position;
            newDataPoint.transform.parent = this.transform;
            newDataPoint.gameObject.name = csvData[i]["nome"].ToString();

            newDataPoint.dataClass = csvData[i]["siglaPartido"].ToString();

            newDataPoint.textLabel.text = csvData[i]["nome"].ToString() + " (" + newDataPoint.dataClass + ")";

            Color newColor = new Color();
            ColorUtility.TryParseHtmlString(csvData[i]["cores"].ToString(), out newColor);
            newColor.a = 1f;
            newDataPoint.GetComponent<Renderer>().material.color = newColor;
            newDataPoint.pointColor = newColor;

            scatterplotPoints.Add(newDataPoint);




        }

        // Create edges between all pairs of points
      //  for (int i = 0; i < scatterplotPoints.Count; i++)
        //{
        //    for (int j = i + 1; j < scatterplotPoints.Count; j++)
          //  {
                // Calculate distance between the two points
             //   float distance = Vector3.Distance(scatterplotPoints[i].transform.position, scatterplotPoints[j].transform.position);

                // Create a new edge object between the two points
              //  GameObject newEdge = new GameObject("Edge " + i + "-" + j);
              //  newEdge.transform.parent = this.transform;

                // Add a LineRenderer component to draw the edge as a line
              //  LineRenderer lineRenderer = newEdge.AddComponent<LineRenderer>();
              //  lineRenderer.startWidth = 0.00002f;
              //  lineRenderer.endWidth = 0.00002f;
              //  lineRenderer.material = new Material(Shader.Find("Sprites/Default"));

                // Set the edge's start and end points to the positions of the two points
             //   lineRenderer.SetPosition(0, scatterplotPoints[i].transform.position);
              //  lineRenderer.SetPosition(1, scatterplotPoints[j].transform.position);
                
           // }
       // }



        // Should also adjust size of scatterplot collider box here based on points positions

    }






    public void ConnectPoints()
    {
        if (selectedPoint == null) return;

        List<ScatterplotDataPoint> closestPoints = new List<ScatterplotDataPoint>();
        foreach (ScatterplotDataPoint point in scatterplotPoints)
        {
            if (point != selectedPoint && Vector3.Distance(point.transform.position, selectedPoint.transform.position) < connectDistance)
            {
                closestPoints.Add(point);
                if (closestPoints.Count >= 10) break; // break after finding 10 closest points
            }
        }

        foreach (ScatterplotDataPoint point in closestPoints)
        {
            GameObject newLine = new GameObject(edge);
            newLine.transform.parent = selectedPoint.transform;
            LineRenderer lineRenderer = newLine.AddComponent<LineRenderer>();
            lineRenderer.material = new Material(Shader.Find("Sprites/Default"));
            lineRenderer.startColor = selectedPoint.pointColor;
            lineRenderer.endColor = point.pointColor;
            lineRenderer.startWidth = 0.0006f;
            lineRenderer.endWidth = 0.0006f;
            lineRenderer.SetPosition(0, selectedPoint.transform.position);
            lineRenderer.SetPosition(1, point.transform.position);
        }

        isConnected = true;

      
    }



}

