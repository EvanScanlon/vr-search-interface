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

public class ImpactSound : MonoBehaviour
{
    
    public AudioSource CollectSound;
    //Shelf and book variables
    public GameObject shelf;
    public TMP_Text Bubble;
    public TMP_Text BookCover;
    public TMP_Text BookCover2;
    public TMP_Text BookCover3;
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

    private void Start()
    {
        stvr_player = GetComponent<Player>();
        string appPath = Directory.GetCurrentDirectory();
        string filePath = "Assets\\Scripts\\dictionary.txt";
        string fullpath = Path.Combine(appPath, filePath);
        string[] lines = File.ReadLines(fullpath).ToArray();
        for(int i = 0; i < lines.Length; i++)
        {
            words.Add(lines[i],AddToSearchQuery);
        }
        location = new Vector3(x,y,z);
        keywordRecognizer = new KeywordRecognizer(words.Keys.ToArray(),ConfidenceLevel.Low);
        keywordRecognizer.OnPhraseRecognized += RecognizedSpeech;
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
                    ExecuteQuery();
                    ClearSearch();
                    keywordRecognizer.Stop();
                }
            }
        }
        
    }

    [ContextMenu("Test Get")]
    public async void ExecuteQuery()
    {
        var url = "https://api.core.ac.uk/v3/search/works/";
        //searchQuery = searchQuery.Replace(" ", "+");
        searchQuery = searchQuery.Remove(searchQuery.Length - 1);
        var request = url + "?q=" + searchQuery + "&api_key=78xrymX61Td4MEwGhRFjSL9uDcnIUbWH";
        Debug.Log(request);
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
            Debug.Log($"Success: {www.downloadHandler.text}");
            File.WriteAllText(@"C:\Users\Public\path.txt", www.downloadHandler.text);
            SearchResponse searchResult = JsonConvert.DeserializeObject<SearchResponse>(www.downloadHandler.text);
            PopulateShelfAsync(searchResult);
            CreateShelf();
        }
        else
            Debug.Log($"Failed: {www.error}");
    }

    async Task PopulateShelfAsync(SearchResponse searchResult)
    {
        //var ID = searchResult.results[0].id;
        var ID = "148922778";
        var request = "https://api.core.ac.uk/v3/works/" + ID + "/download?api_key=78xrymX61Td4MEwGhRFjSL9uDcnIUbWH";
        using var www = UnityWebRequest.Get(request);
        var operation = www.SendWebRequest();
        while (!operation.isDone)
        {
            await Task.Yield();
        }
        if (www.result == UnityWebRequest.Result.Success)
        {
            Debug.Log($"Success: {www.downloadHandler.text}");
        }
        else
            Debug.Log($"Failed: {www.error}");
        //populate shelf with RetrievedData
        String title = searchResult.results[0].title;
        BookCover.text = title;
        title = searchResult.results[1].title;
        BookCover2.text = title;
        title = searchResult.results[2].title;
        BookCover3.text = title;
    }

    private void RecognizedSpeech(PhraseRecognizedEventArgs speech)
    {
        currentWord = speech.text;
        Debug.Log(speech.text);
        words[speech.text].Invoke();
    }

    private void AddToSearchQuery()
    {
        searchQuery += currentWord + " ";
        Debug.Log("Current search query: " +searchQuery);
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

    void ToggleEngaged()
    {
        engaged = !engaged;
    }

    void CreateShelf()
    {
        Instantiate(shelf, location, Quaternion.identity);
        /*if (alternate)
        {
            x -= 5; //CHANGED THIS
            location = new Vector3(x, y, z);
            alternate = !alternate;
        }
        else
        {
            z -= 5;
            location = new Vector3(x, y, z);
            z += 5;
            alternate = !alternate;
        }*/
    }

}

