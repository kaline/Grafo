using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using TMPro;

public class Scatterplot : MonoBehaviour
{
    public GameObject pointPrefab;
    List<ScatterplotDataPoint> scatterplotPoints = null;

    public float connectDistance = 0.0001f;

    private ScatterplotDataPoint selectedPoint = null;
    private bool isConnected = false;

    public string edge = "Edge";

    private bool scatterplotMoved = false;

    public TMP_Text deputadoText;



    // Use this for initialization
    void Start()
    {

        try
        {
            LoadPoints("Data/deputados_valid_result");

            // Find all ScatterplotDataPoints in the scene
            ScatterplotDataPoint[] points = FindObjectsOfType<ScatterplotDataPoint>();
            foreach (ScatterplotDataPoint point in points)
            {
                scatterplotPoints.Add(point);
            }
        }
        catch (DirectoryNotFoundException e)
        {
            Debug.LogError("The directory was not found. " + e.Message);
        }
        catch (FileNotFoundException e)
        {
            Debug.LogError("The file was not found. " + e.Message);
        }
        catch (IOException e)
        {
            Debug.LogError("An IO error occurred while reading the file. " + e.Message);
        }


    }




    void Update()
    {
        if (Input.GetMouseButtonDown(0)) // check for left mouse button click
        {

            if (scatterplotPointsMoved())
            {
                scatterplotMoved = true;
            }

            //RemoveAllEdges();
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
        string allDeputies = "";



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



            // adding text to UI
            allDeputies += csvData[i]["nome"].ToString() + '\n';




            // similar deputies columns


            for (int j = 1; j <= 10; j++)
                {
                    newDataPoint.gameObject.name = csvData[i]["nome"].ToString(); csvData[i]["top_" + j + "_deputy"].ToString();
                   
                }

        


            scatterplotPoints.Add(newDataPoint);
            // Display all the deputies' names in the UI
            deputadoText.text = allDeputies;




        }




        // Should also adjust size of scatterplot collider box here based on points positions

    }

    public void allEdges()
    {

        // Create edges between all pairs of points
        for (int i = 0; i < scatterplotPoints.Count; i++)
        {
            for (int j = i + 1; j < scatterplotPoints.Count; j++)
            {
                // Calculate distance between the two points
                float distance = Vector3.Distance(scatterplotPoints[i].transform.position, scatterplotPoints[j].transform.position);

                // Create a new edge object between the two points
                GameObject newEdge = new GameObject("Edge " + i + "-" + j);
                newEdge.transform.parent = this.transform;

                // Add a LineRenderer component to draw the edge as a line
                LineRenderer lineRenderer = newEdge.AddComponent<LineRenderer>();
                lineRenderer.startWidth = 0.00002f;
                lineRenderer.endWidth = 0.00002f;
                lineRenderer.material = new Material(Shader.Find("Sprites/Default"));

                // Set the edge's start and end points to the positions of the two points
                lineRenderer.SetPosition(0, scatterplotPoints[i].transform.position);
                lineRenderer.SetPosition(1, scatterplotPoints[j].transform.position);

            }
        }
    }






    public void ConnectPoints()
    {
        if (selectedPoint == null) return;

        // Remove all edges if the scatterplot has moved
        if (scatterplotMoved)
        {
            Debug.Log("scatterplotMoved  if");
            RemoveAllEdges();
            scatterplotMoved = false;
        }

        List<ScatterplotDataPoint> closestPoints = new List<ScatterplotDataPoint>();
        List<float> listDistances = new List<float>(); 
        foreach (ScatterplotDataPoint point in scatterplotPoints)
        {
            float distance = Vector3.Distance(point.transform.position, selectedPoint.transform.position);

            if (point != selectedPoint &&  distance < connectDistance)
            {
                closestPoints.Add(point);
                listDistances.Add(distance);
               // if (closestPoints.Count >= 10) break; // break after finding 10 closest points
            }
        }


        // Sort closestPoints based on their corresponding distances in listDistances
        closestPoints = closestPoints.OrderBy(point => listDistances[closestPoints.IndexOf(point)]).ToList();

        IList list = closestPoints;
        for (int i = 0; i < closestPoints.Count; i++)
        {
            Debug.Log("closestPoints " + closestPoints[i]);

        }

        foreach (ScatterplotDataPoint point in closestPoints.GetRange(0, 20))
        {
            Debug.Log("ClosestPoints when click the sphere " + point);

            // Create a new edge object between the selected point and its closest points
            GameObject newEdge = new GameObject(edge);
            newEdge.transform.parent = selectedPoint.transform;

            // Add a LineRenderer component to draw the edge as a line
            LineRenderer lineRenderer = newEdge.AddComponent<LineRenderer>();
            lineRenderer.material = new Material(Shader.Find("Sprites/Default"));
            lineRenderer.startColor = selectedPoint.pointColor;
            lineRenderer.endColor = point.pointColor;
            lineRenderer.startWidth = 0.0006f;
            lineRenderer.endWidth = 0.0006f;

            // Set the edge's start and end points to the positions of the selected point and its closest points
            lineRenderer.SetPosition(0, selectedPoint.transform.position);
            lineRenderer.SetPosition(1, point.transform.position);
        }

        // Deselect the points
        selectedPoint = null;
        isConnected = true;

      
    }

    public void RemoveAllEdges()
    {
        foreach (ScatterplotDataPoint point in scatterplotPoints)
        {
            int numEdges = point.transform.childCount;
            for (int i = numEdges - 1; i >= 0; i--)
            {
                Destroy(point.transform.GetChild(i).gameObject);
            }
        }
    }

    private bool scatterplotPointsMoved()
    {
        foreach(ScatterplotDataPoint point in scatterplotPoints)
        {
            if (point.transform.hasChanged)
            {
                Debug.Log(" scatterplotPointsMoved true");
                return true;
            }
        }
        Debug.Log("scatterplotPointsMoved false");

        return false;
    }



}

