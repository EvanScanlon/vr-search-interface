using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using Valve.VR;
using Valve.VR.InteractionSystem;
public class Preview : MonoBehaviour
{
    public RawImage image;
    public Throwable throwable;
    public int pageNumber = 1;
    [SerializeField]
    SteamVR_Action_Boolean stvr_right;
    [SerializeField]
    SteamVR_Action_Boolean stvr_left;
    // Start is called before the first frame update
    void Start()
    {
        
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

}
