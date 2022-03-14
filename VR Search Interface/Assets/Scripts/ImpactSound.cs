using System.Collections;
using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Windows.Speech;
using UnityEngine.Networking;
using TMPro;
using System.Threading.Tasks;
using System.IO;
using Valve.Newtonsoft.Json;
using Valve.VR;
using Valve.VR.InteractionSystem;
using System.Diagnostics;
using Debug = UnityEngine.Debug;
using UnityEngine.UI;

public class ImpactSound : MonoBehaviour
{
    public AudioSource CollectSound;
    //Shelf and book variables
    public GameObject shelf;
    public TMP_Text Bubble;
    public RawImage image1;
    public RawImage image2;
    public RawImage image3;
    public Vector3 location;
    public float x = 0.0f;
    public float y = 7.62f;
    public float z = -0.139f;
    public bool alternate = false;
    public bool engaged = false;
    public SearchCollection searchCollection;
    public String searchQuery = "";
    public String baseUri;
    public String currentWord;
    public String API_KEY = "78xrymX61Td4MEwGhRFjSL9uDcnIUbWH";
    float timer;
    float holdDur = 2f;
    [SerializeField]
    SteamVR_Action_Boolean stvr_grip;
    //Attached SteamVR Player Component Components
    Player stvr_player;
    private KeywordRecognizer keywordRecognizer;
    private Dictionary<string, Action> words = new Dictionary<string, Action>();
    public Throwable throwable;

    private void Start()
    {
        stvr_player = GetComponent<Player>();
        string appPath = Directory.GetCurrentDirectory();
        string filePath = "Assets\\Scripts\\dictionary.txt";
        string fullpath = Path.Combine(appPath, filePath);
        string[] lines = File.ReadLines(fullpath).ToArray();
        for (int i = 0; i < lines.Length; i++)
        {
            words.Add(lines[i], AddToSearchQuery);
        }
        location = new Vector3(x, y, z);
        keywordRecognizer = new KeywordRecognizer(words.Keys.ToArray(), ConfidenceLevel.Low);
        keywordRecognizer.OnPhraseRecognized += RecognizedSpeech;
        //TestQuery();
        ExecuteQuery("Hello World");
        //CreateShelf();
    }

    private void Update()
    {
        if (engaged)
        {
            Bubble.text = searchQuery + "?";
        }
        if (stvr_grip.stateDown && engaged)
        {
            timer = Time.time;
        }
        else if (stvr_grip.state && engaged)
        {
            if (Time.time - timer > holdDur)
            {
                //by making it positive inf, we won't subsequently run this code by accident,
                //since X - +inf = -inf, which is always less than holdDur
                timer = float.PositiveInfinity;
                if (searchQuery != "")
                {
                    CollectSound.Play();
                    engaged = false;
                    MakeQuery();
                    ClearSearch();
                    keywordRecognizer.Stop();
                }
            }
        }
    }

    public async void TestQuery()
    {
        //await DownloadPDF("46713218");
        if ((File.Exists(@"C:\Users\Public\46713218.pdf")))
            {
            PdfToJpg(@"C:\Users\Public\46713218.pdf", @"C:\Users\Public\46713218.jpg");
            image1.texture = LoadJPG(@"C:\Users\Public\46713218.jpg1.jpg");
            //CreateShelf();
        }
        else
        {
            Debug.Log("File not found");
        }
    }

    private void PdfToJpg(string inputPDFFile, string outputImagesPath)
    {
        if ((!File.Exists(inputPDFFile))) Debug.Log("File not found! " + inputPDFFile);
        else
        {
            Debug.Log("File found!");
            string ghostScriptPath = @"C:\Program Files (x86)\gs\gs9.55.0\bin\gswin32.exe";
            String ars = "-dNOPAUSE -sDEVICE=jpeg -r200 -o" + outputImagesPath + "%d.jpg -sPAPERSIZE=a4 " + inputPDFFile;
            Process proc = new Process();
            proc.StartInfo.FileName = ghostScriptPath;
            proc.StartInfo.Arguments = ars;
            proc.StartInfo.CreateNoWindow = true;
            proc.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
            proc.Start();
            proc.WaitForExit();
        }
    }

    [ContextMenu("Test Get")]
    public async void MakeQuery()
    {
        ExecuteQuery(searchQuery.Remove(searchQuery.Length - 1));
    }

    public async void ExecuteQuery(String searchQuery)
    {
        var url = "https://api.core.ac.uk/v3/search/works/";
        var request = url + "?q=" + searchQuery + "&api_key=78xrymX61Td4MEwGhRFjSL9uDcnIUbWH";
        UnityEngine.Debug.Log(request);
        using var www = UnityWebRequest.Get(request);
        //using var www = UnityWebRequest.Get("https://api.core.ac.uk/#operation/null/v3/labs/outputs/dedup");
        www.SetRequestHeader("Content-Type", "application/json");
        //www.SetRequestHeader("Authorization", "Bearer " + API_KEY);
        var operation = www.SendWebRequest();
        while (!operation.isDone)
        {
            await Task.Yield();
        }
        if (www.result == UnityWebRequest.Result.Success)
        {
            UnityEngine.Debug.Log($"Success: {www.downloadHandler.text}");
            //File.WriteAllText(@"C:\Users\Public\path.txt", www.downloadHandler.text);
            SearchResponse searchResult = JsonConvert.DeserializeObject<SearchResponse>(www.downloadHandler.text);
            //ExtractIdentifier(searchResult.results[0].downloadUrl);
            await HandleSearchResultsAsync(searchResult);

            CreateShelf(searchResult);
        }
        else
            UnityEngine.Debug.Log($"Failed: {www.error}");

    }
    void CreateShelf(SearchResponse searchResult)
    {
        GameObject searchResults = Instantiate(shelf, location, Quaternion.identity);
        searchResults.transform.GetChild(0).name = ExtractIdentifier(searchResult.results[0].downloadUrl);
        searchResults.transform.GetChild(1).name = ExtractIdentifier(searchResult.results[1].downloadUrl);
        searchResults.transform.GetChild(2).name = ExtractIdentifier(searchResult.results[2].downloadUrl);
    }

    async Task HandleSearchResultsAsync(SearchResponse searchResult)
    {
        Texture2D[] textures = null;
        for (int i = 0; i < 3; i++)
        {
            String identifier = ExtractIdentifier(searchResult.results[i].downloadUrl);
            if(!File.Exists(@"C:\Users\Public\" + identifier + ".pdf")) await DownloadPDF(identifier); 
            PdfToJpg(@"C:\Users\Public\" + identifier + ".pdf", @"C:\Users\Public\" + identifier + ".jpg");
            if(i == 0)image1.texture = LoadJPG(@"C:\Users\Public\" + identifier + @".jpg1.jpg");
            if (i == 1) image2.texture = LoadJPG(@"C:\Users\Public\" + identifier + @".jpg1.jpg");
            if (i == 2) image3.texture = LoadJPG(@"C:\Users\Public\" + identifier + @".jpg1.jpg");
        }
    }

    public static Texture2D LoadJPG(string filePath)
    {

        Texture2D tex = null;
        byte[] fileData;

        if (File.Exists(filePath))
        {
            Debug.Log(filePath + " found!");
            fileData = File.ReadAllBytes(filePath);
            tex = new Texture2D(2, 2);
            tex.LoadImage(fileData); //..this will auto-resize the texture dimensions.
        }
        else
        {
            Debug.Log(filePath + " not found!");
        }
        return tex;
    }

    public async Task DownloadPDF(String identifier)
    {
        //var identifier = "186632692.pdf";
        //var request = "https://api.core.ac.uk/v3/outputs/" + identifier + "/download" + "&api_key=78xrymX61Td4MEwGhRFjSL9uDcnIUbWH";
        //var request = "https://core.ac.uk//download//" + identifier + ".pdf";
        var request = "https://core.ac.uk//download//" + identifier + ".pdf";
        using var www = UnityWebRequest.Get(request);
        www.SetRequestHeader("Content-Type", "application/pdf");
        var operation = www.SendWebRequest();
        while (!operation.isDone)
        {
            await Task.Yield();
        }
        if (www.result == UnityWebRequest.Result.Success)
        {
            UnityEngine.Debug.Log($"Success: {www.downloadHandler.text}");
            Texture2D page = new Texture2D(2, 2);
            string inBase64 = Convert.ToBase64String(www.downloadHandler.data);
            if (!File.Exists(@"C:\Users\Public\" + ExtractIdentifier(request) + ".pdf"))
            {
                System.IO.FileStream stream = new FileStream(@"C:\Users\Public\" + ExtractIdentifier(request) + ".pdf", FileMode.CreateNew);
                System.IO.BinaryWriter writer =
                new BinaryWriter(stream);
                writer.Write(www.downloadHandler.data, 0, www.downloadHandler.data.Length);
                writer.Close();
            }
        }
        else
            UnityEngine.Debug.Log($"Failed: {www.error}");
    }

    private void RecognizedSpeech(PhraseRecognizedEventArgs speech)
    {
        currentWord = speech.text;
        UnityEngine.Debug.Log(speech.text);
        words[speech.text].Invoke();
    }

    private void AddToSearchQuery()
    {
        searchQuery += currentWord + " ";
        UnityEngine.Debug.Log("Current search query: " + searchQuery);
    }

    private void ClearSearch()
    {
        searchQuery = "";
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            CollectSound.Play();
            engaged = true;
            keywordRecognizer.Start();
        }
    }

    String ExtractIdentifier(String downloadUrl)
    {
        UnityEngine.Debug.Log("Download URL: " + downloadUrl);
        String identifier;
        if (downloadUrl.Contains("/download/pdf/"))
        {
            identifier = downloadUrl.Substring(32);
        }
        else if ((downloadUrl.Contains("//download//")))
        {
            identifier = downloadUrl.Substring(30);
        }
        else
        {
            identifier = downloadUrl.Substring(28);
        }
        identifier = identifier.Substring(0, identifier.Length - 4);
        UnityEngine.Debug.Log("Extracted Identifier: " + identifier);
        return identifier;
    }

    void ToggleEngaged()
    {
        engaged = !engaged;
    }

}

