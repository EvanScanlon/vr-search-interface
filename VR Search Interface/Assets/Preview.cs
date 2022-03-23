using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using Valve.VR;
using Valve.VR.InteractionSystem;
using TMPro;
public class Preview : MonoBehaviour
{
    public AudioSource CollectSound;
    public RawImage image;
    public RawImage printedImage;
    public Throwable throwable;
    [SerializeField]
    Collider collider;
    public int pageNumber = 1;
    [SerializeField]
    SteamVR_Action_Boolean stvr_right;
    [SerializeField]
    SteamVR_Action_Boolean stvr_left;
    [SerializeField]
    SteamVR_Action_Boolean stvr_grip;
    [SerializeField]
    SteamVR_Action_Boolean stvr_south;
    public ImpactSound controller;
    public GameObject paper;
    public GameObject instantiatedPaper;
    public Vector3 location;
    public float x = -1.0243f;
    public float y = 1.1222f;
    public float z = 0.2877f;

    public TMP_Text Documents;
    public TMP_Text Pictures;

    // Start is called before the first frame update
    void Start()
    {
        location = new Vector3(x, y, z);
    }

    // Update is called once per frame
    void Update()
    {
        if (throwable.interactable.attachedToHand != null)
        {
            if (stvr_right.stateDown)
            {
                flipPage("right");
            }
            if (stvr_left.stateDown)
            {
                flipPage("left");
            }
            if (stvr_grip.stateDown)
            {
                CollectSound.Play();
                PrintImage(throwable.interactable.name);
            }
            if (stvr_south.stateDown)
            {
                //Send object to shelf
                Debug.Log("down");
            }
        }
    }

    void flipPage(string input)
    {
        if (input == "right")
        {
            if(File.Exists(@"C:\Users\Public\" + throwable.interactable.name + @".jpg" + pageNumber + ".jpg"))
            {
               pageNumber++;
            }
            else
            {
                Debug.Log("End of document!");
            }
            
        }
        if (input == "left")
        {
            if (pageNumber <= 1) pageNumber = 1;
            else pageNumber--;
        }
        image.texture = LoadJPG(@"C:\Users\Public\" + throwable.interactable.name + @".jpg" + pageNumber + ".jpg");
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

    void PrintImage(string name)
    {
        printedImage.texture = image.texture;
        instantiatedPaper = Instantiate(paper,location,Quaternion.identity);
        instantiatedPaper.transform.Rotate(75.0f, 0.0f, 0.0f, Space.Self);
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "ALLOW_MANIPULATION")
        {
            Physics.IgnoreCollision(collision.collider, collider);
            Debug.Log("Collision ignored between "+collision.collider.name+" and "+collider.name);
        }

    }
    }
