using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;

public class Scatterplot : MonoBehaviour
{
    public GameObject pointPrefab;
    public List<ScatterplotDataPoint> scatterplotPoints = null;

    public float connectDistance = 0.0001f;

    public ScatterplotDataPoint selectedPoint = null;
    private bool isConnected = false;

    public string edge = "Edge";

    private bool scatterplotMoved = false;

    public TMP_Text deputadoText;
    public TMP_Text deputadoSimilar;
    public TMP_Text Label;
    public TMP_Text infoDeputado;
    public TMP_Text  partidoRanking;
    public TMP_Text LabelWomenNumber;
    public TMP_Text LabelWomenPercentage;




    List<GameObject> edges = new List<GameObject>();

    public imageDeputado imageDeputadoScript = new imageDeputado();
    //public GameObject buttonPT;

    Color newColor = new Color();





    // Use this for initialization
    void Start()
    {

        // Initialize imageDeputadoScript reference
        imageDeputadoScript = GameObject.Find("ImageObject").GetComponent<imageDeputado>();

        try
        {
            LoadPoints("Data/deputados_2022");
            showPartidoRanking("Data/deputados_ranking_2022");



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
        // Add a listener to the buttonOne
        //buttonPT.GetComponent<Button>().onClick.AddListener(UpdateSphereColors);
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
                
                if (clickedPoint != null && selectedPoint == clickedPoint) // if the same point is clicked again
                {
                    if (isConnected)
                    {
                        RemoveAllEdgesButton();
                    }
                    selectedPoint = null;
                    isConnected = false;
                }

                if (clickedPoint != null && selectedPoint == null) // if a point is clicked and no point is currently selected
                {
                    selectedPoint = clickedPoint; // select the clicked point
                }
                else if (clickedPoint != null && clickedPoint != selectedPoint && clickedPoint != selectedPoint) // if a point is clicked and another point is selected
                {
                    float distance = Vector3.Distance(clickedPoint.transform.position, selectedPoint.transform.position);

                    if (distance <= connectDistance) // if the distance between the points is less than the threshold
                    {

                        // deselect the points
                        selectedPoint = null;
                        clickedPoint = null;

                    }
                    else
                    {
                        // deselect the points
                        selectedPoint = null;
                        clickedPoint = null;
                        isConnected = false;

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
            
            ScatterplotDataPoint newDataPoint = Instantiate(pointPrefab, new Vector3(x*15, y*15, z*15), Quaternion.identity). 
                GetComponent<ScatterplotDataPoint>();
            //pointPrefab.transform.parent = transform;

            newDataPoint.transform.position += this.transform.position;
            newDataPoint.transform.parent = this.transform;
            newDataPoint.gameObject.name = csvData[i]["nome"].ToString();

            newDataPoint.dataClass = csvData[i]["siglaPartido"].ToString();

            newDataPoint.textLabel.text = csvData[i]["nome"].ToString() + " (" + newDataPoint.dataClass + ")";

            ColorUtility.TryParseHtmlString(csvData[i]["cores"].ToString(), out newColor);
            newColor.a = 1f;
            newDataPoint.GetComponent<Renderer>().material.color = newColor;
            newDataPoint.pointColor = newColor;
            // Set the image url
            newDataPoint.urlFoto = csvData[i]["urlFoto"].ToString();
            newDataPoint.siglaUf = csvData[i]["siglaUf"].ToString();
            newDataPoint.gender = csvData[i]["gender"].ToString();
            newDataPoint.scatterplot = this;

            // adding text to UI
            allDeputies += csvData[i]["nome"].ToString() + '\n';


            scatterplotPoints.Add(newDataPoint);
            // Display all the deputies' names in the UI
            deputadoText.text = allDeputies;

           // for (int j = 1; j <= 10; j++)
            //{
            //    newDataPoint.gameObject.name = csvData[i]["nome"].ToString() + '\n';
               


           // }




        }




        // Should also adjust size of scatterplot collider box here based on points positions

    }




    Dictionary<string, string> partyColors = new Dictionary<string, string>()
{
    {"UNIÃO", "#8B4513"},
    {"NOVO", "#004445"},
    {"PP", "#00BFFF"},
    {"PSDB", "#0072C6"},
    {"PT", "#FF0000"},
    {"PATRIOTA", "#4B0082"},
    {"REPUBLICANOS", "#FF7F00"},
    {"PSB", "#FFD700"},
    {"PSD", "#BDB76B"},
    {"PCdoB", "#FF69B4"},
    {"PV", "#00FF7F"},
    {"PROS", "#00CED1"},
    {"PSC", "#FF1493"},
    {"PDT", "#2E8B57"},
    {"AVANTE", "#DC143C"},
    {"CIDADANIA", "#FFC0CB"},
    {"PSOL", "#800080"},
    {"SOLIDARIEDADE", "#ADD8E6"},
    {"PL", "#8B0000"},
    {"MDB", "#006400"},
    {"PTB", "#FFDAB9"},
    {"PODE", "#228B22"},
    {"REDE", "#FFA500"},
    {"S.PART.", "#808080"}
};

    public void UpdateSphereColors(string siglaPartido)
    {
        Color newColor1 = Color.white;
        newColor1.a = 0.42f;

        // Loop through all scatterplot points
        foreach (ScatterplotDataPoint point in scatterplotPoints)
        {
            if (point.dataClass == siglaPartido)
            {
                // Update the color with the original color of siglaPartido
                string hexColor = partyColors[siglaPartido];
                Color newColor = Color.white;
                ColorUtility.TryParseHtmlString(hexColor, out newColor);
                newColor.a = 0.42f;
                point.GetComponent<Renderer>().material.color = newColor;
                point.pointColor = newColor;
            }
            else
            {
                // Update the color to white
                point.GetComponent<Renderer>().material.color = newColor1;
                point.pointColor = newColor1;
            }
        }
    }


    public void UpdateSphereColorsUF(string siglauf)
    {
        // Loop through all scatterplot points
        foreach (ScatterplotDataPoint point in scatterplotPoints)
        {
            if (point.siglaUf == siglauf)
            {
                // Reset the color to the original color
                point.GetComponent<Renderer>().material.color = point.pointColor;
            }
            else
            {
                // Update the color to white
                Color newColor = Color.white;
                newColor.a = 0.42f;
                // Update the color to white
                point.GetComponent<Renderer>().material.color = newColor;
            }
        }
    }


    private string loadedDatasetName;
    public void LoadNewDataset(string datasetName)
    {

        foreach(ScatterplotDataPoint point in scatterplotPoints)
        {
            Debug.Log("type ScatterplotDataPoint of point " + point);
            Debug.Log("type ScatterplotDataPoint of scatterplotPoints " + scatterplotPoints);


            Destroy(point.gameObject);
        }

        scatterplotPoints.Clear();
        
        // Load and display the new dataset
        LoadPoints("Data/" + "deputados_" + datasetName);
        showPartidoRanking("Data/" + "deputados_ranking_" + datasetName);

        // store the loaded dataset name in a class-lvel variable
        loadedDatasetName = datasetName;
        
  
    }



   


    public void genderColors()
    {
        Color color = Color.magenta;

      if(loadedDatasetName == "2022")
        {
            LabelWomenNumber.text = "85 mulheres";
            LabelWomenPercentage.text = "14.98%";

        }else if(loadedDatasetName == "2023")
        {
            LabelWomenNumber.text = "85 mulheres";
            LabelWomenPercentage.text = "14.98%";

        }else if(loadedDatasetName == "2021")
        {
            LabelWomenNumber.text = "85 mulheres";
            LabelWomenPercentage.text = "14.98%";
        }
           
        

        foreach(ScatterplotDataPoint point in scatterplotPoints)
        {
            Debug.Log("gender " + point.gender);

            if(point.gender == "Feminino")
            {
                point.GetComponent<Renderer>().material.color = color;
                point.pointColor = color;
            }
            else if(point.gender == "Masculino"){
                point.GetComponent<Renderer>().material.color = Color.blue;
                point.pointColor = Color.blue;
            }
            else if (point.gender == "Desconhecido")
            {
                point.GetComponent<Renderer>().material.color = Color.grey;
                point.pointColor = Color.grey;
            }
        }
    }

    public void showInfoGender()
    {
        
        List<Dictionary<string, object>> csvData = CSVReader.Read("");
        for (var i = 0; i < csvData.Count; i++)
        {
            
        }


    }

    public void showPartidoRanking(string  datapath)
    {
        string partidos = "";
        List<Dictionary<string, object>> csvData = CSVReader.Read(datapath);

        for (var i = 0; i < csvData.Count; i++)
        {
            partidos += csvData[i]["siglaPartido"].ToString() + " - " + csvData[i]["Count"].ToString() + csvData[i]["Percentage"].ToString()  + "%3" + '\n';
            partidoRanking.text = partidos;

        }


    }

    public void UpdateSphereOriginalColor()
    {
        Scene scene = SceneManager.GetActiveScene(); 
        SceneManager.LoadScene(scene.name);

    }




    //public void allEdges()
    //{

    //    // Create edges between all pairs of points
    //    for (int i = 0; i < scatterplotPoints.Count; i++)
    //    {
    //        for (int j = i + 1; j < scatterplotPoints.Count; j++)
    //        {
    //            // Calculate distance between the two points
    //            float distance = Vector3.Distance(scatterplotPoints[i].transform.position, scatterplotPoints[j].transform.position);

    //            // Create a new edge object between the two points
    //            GameObject newEdge = new GameObject("Edge " + i + "-" + j);
    //            newEdge.transform.parent = this.transform;

    //            // Add a LineRenderer component to draw the edge as a line
    //            LineRenderer lineRenderer = newEdge.AddComponent<LineRenderer>();
    //            lineRenderer.startWidth = 0.00002f;
    //            lineRenderer.endWidth = 0.00002f;
    //            lineRenderer.material = new Material(Shader.Find("Sprites/Default"));

    //            // Set the edge's start and end points to the positions of the two points
    //            lineRenderer.SetPosition(0, scatterplotPoints[i].transform.position);
    //            lineRenderer.SetPosition(1, scatterplotPoints[j].transform.position);

    //        }
    //    }
    //}

    public void allEdges()
    {
        int numPoints = scatterplotPoints.Count;

        // Create mesh for all edges
        Mesh mesh = new Mesh();
        Vector3[] vertices = new Vector3[numPoints * (numPoints - 1) / 2 * 2]; // Two vertices per edge
        int[] indices = new int[numPoints * (numPoints - 1) / 2 * 2];
        int vertexIndex = 0;
        int index = 0;

        for (int i = 0; i < numPoints; i++)
        {
            for (int j = i + 1; j < numPoints; j++)
            {
                // Calculate distance between the two points
                float distance = Vector3.Distance(scatterplotPoints[i].transform.position, scatterplotPoints[j].transform.position);

                // Add vertices for the edge
                vertices[vertexIndex] = scatterplotPoints[i].transform.position;
                vertices[vertexIndex + 1] = scatterplotPoints[j].transform.position;

                // Add indices for the edge
                indices[index] = vertexIndex;
                indices[index + 1] = vertexIndex + 1;

                // Increment counters
                vertexIndex += 2;
                index += 2;
            }
        }

        // Set up mesh and add to object
        mesh.vertices = vertices;
        mesh.SetIndices(indices, MeshTopology.Lines, 0);
        GetComponent<MeshFilter>().mesh = mesh;

        // Set up line renderer
        LineRenderer lineRenderer = gameObject.AddComponent<LineRenderer>();
        lineRenderer.startWidth = 0.00002f;
        lineRenderer.endWidth = 0.00002f;
        lineRenderer.material = new Material(Shader.Find("Sprites/Default"));
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

            if(point == selectedPoint)
            {
                Debug.Log("Selected point " + point);
                Label.text = point.name;
               
                    // get the image url for the selected point and update the image
                    string imageUrl = point.GetComponent<ScatterplotDataPoint>().urlFoto;
                    StartCoroutine(imageDeputadoScript.GetImage(imageUrl));
                

                infoDeputado.text = point.name + "\n" + point.dataClass + "\n" + point.siglaUf;


            }

            if (point != selectedPoint &&  distance < connectDistance)
            {
                closestPoints.Add(point);
                listDistances.Add(distance);
               // if (closestPoints.Count >= 10) break; // break after finding 10 closest points
            }
        }


        // Sort closestPoints based on their corresponding distances in listDistances
        closestPoints = closestPoints.OrderBy(point => listDistances[closestPoints.IndexOf(point)]).ToList();
        
        // Get the unique closest points
        List<ScatterplotDataPoint> uniqueClosestPoints = closestPoints.Distinct().ToList();


       
       
        // Clear the current text in deputadoSimilar
        deputadoSimilar.text = "";

        foreach (ScatterplotDataPoint point in uniqueClosestPoints.GetRange(0, 10))
        {
            Debug.Log("ClosestPoints when click the sphere " + point.name);
            deputadoSimilar.text += point.name + " -  " + point.dataClass + "\n";

            // Create a new edge object between the selected point and its closest points
            GameObject newEdge = new GameObject("Edge");
            newEdge.transform.parent = selectedPoint.transform;
          

            // Add the edge to the list of edges
            edges.Add(newEdge);
            Debug.Log("edges "+ edges);

            // Add a LineRenderer component to draw the edge as a line
            LineRenderer lineRenderer = newEdge.AddComponent<LineRenderer>();
            lineRenderer.material = new Material(Shader.Find("Sprites/Default"));
            lineRenderer.startColor = selectedPoint.pointColor;
            lineRenderer.endColor = point.pointColor;
            lineRenderer.startWidth = 0.0006f;
            lineRenderer.endWidth = 0.0006f;
            lineRenderer.useWorldSpace = false;


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

    public void RemoveAllEdgesButton()
    {
        Debug.Log(" RemoveAllEdgesButton ");
        foreach (GameObject edge in edges)
        {
            Destroy(edge);
        }
        edges.Clear();
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

