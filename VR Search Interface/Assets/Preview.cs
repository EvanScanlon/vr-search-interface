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
    public GameObject shelf;
    public Throwable throwable;
    public Rigidbody body;
    public int pageNumber = 1;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (throwable.interactable.attachedToHand != null)
        {
            if (Input.GetKeyDown(KeyCode.RightArrow))
            {
                flipPage("right");
            }
            if (Input.GetKeyDown(KeyCode.LeftArrow))
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
