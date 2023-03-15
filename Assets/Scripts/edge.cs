using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;
using Microsoft.MixedReality.Toolkit.UI;

public class ScatterplotMRTK : MonoBehaviour
{
    public GameObject pointPrefab;
    public float connectDistance = 2.0f;
    public int maxConnections = 10;

    private List<ScatterplotDataPoint> scatterplotPoints = new List<ScatterplotDataPoint>();
    private List<ScatterplotDataPoint> connectedPoints = new List<ScatterplotDataPoint>();
    private LineRenderer lineRenderer;

    private void Start()
    {
        LoadPoints("Data/deputadosColorsHex");
        lineRenderer = gameObject.AddComponent<LineRenderer>();
        lineRenderer.material = new Material(Shader.Find("Sprites/Default"));
        lineRenderer.startWidth = 0.02f;
        lineRenderer.endWidth = 0.02f;
    }

    private void LoadPoints(string datasetPath)
    {
        List<Dictionary<string, object>> csvData = CSVReader.Read(datasetPath);

        foreach (Dictionary<string, object> data in csvData)
        {
            float x = 0.05f * (float)data["x"];
            float y = 0.05f * (float)data["y"];
            float z = 0.05f * (float)data["z"];

            ScatterplotDataPoint newDataPoint = Instantiate(pointPrefab, new Vector3(x, y, z), Quaternion.identity).GetComponent<ScatterplotDataPoint>();
            newDataPoint.transform.parent = transform;
            newDataPoint.gameObject.name = data["nome"].ToString();
            newDataPoint.dataClass = data["siglaPartido"].ToString();
            newDataPoint.textLabel.text = data["nome"].ToString() + " (" + newDataPoint.dataClass + ")";

            Color newColor;
            ColorUtility.TryParseHtmlString(data["cores"].ToString(), out newColor);
            newColor.a = 1f;
            newDataPoint.GetComponent<Renderer>().material.color = newColor;
            newDataPoint.pointColor = newColor;

            scatterplotPoints.Add(newDataPoint);
        }
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            ConnectClosestPoints();
        }
    }

    private void ConnectClosestPoints()
    {
        if (connectedPoints.Count >= maxConnections) return;

        ScatterplotDataPoint closestPoint = null;
        float closestDistance = float.MaxValue;

        foreach (ScatterplotDataPoint point in scatterplotPoints)
        {
            if (connectedPoints.Contains(point)) continue;

            float distance = Vector3.Distance(point.transform.position, transform.position);
            if (distance < closestDistance)
            {
                closestPoint = point;
                closestDistance = distance;
            }
        }

        if (closestPoint != null && closestDistance < connectDistance)
        {
            connectedPoints.Add(closestPoint);
            DrawLine();
        }
    }

    private void DrawLine()
    {
        lineRenderer.positionCount = connectedPoints.Count;
        for (int i = 0; i < connectedPoints.Count; i++)
        {
            lineRenderer.SetPosition(i, connectedPoints[i].transform.position);
        }
    }

    public void ClearConnections()
    {
        connectedPoints.Clear();
        lineRenderer.positionCount = 0;
    }
}
