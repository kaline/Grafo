using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;




public class imageDeputado : MonoBehaviour
{

    private Dictionary<string, string> imageUrlsDict = new Dictionary<string, string>();

    // Start is called before the first frame update
    void Start()
    {

    // Load image URLs from CSV file
    List<Dictionary<string, object>> csvData = CSVReader.Read("Data/deputados_valid_result_colors");
        for (var i = 0; i < csvData.Count; i++)
        {
            string id = csvData[i]["id"].ToString();
            string urlFoto = csvData[i]["urlFoto"].ToString();
            imageUrlsDict.Add(id, urlFoto);
        }

    }

    // Update is called once per frame
    void Update()
    {
        
    }

     
    

    public void ShowImage(string id)
    {
        string url;
        if (imageUrlsDict.TryGetValue(id, out url))
        {
            StartCoroutine(GetImage(url));
        }
    }

    public IEnumerator GetImage(string imageUrl)
    {
        UnityWebRequest request = UnityWebRequestTexture.GetTexture(imageUrl);
        yield return request.SendWebRequest();
        if(request.isNetworkError || request.isHttpError)
        {

            Debug.Log(request.error);
        }
        else
        {
            Texture2D downloadtexture = DownloadHandlerTexture.GetContent(request) as Texture2D;
            GetComponent<Image>().sprite = Sprite.Create(downloadtexture, new Rect(0, 0, downloadtexture.width, downloadtexture.height), new Vector2(0, 0));
        }

    }
}
