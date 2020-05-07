using UnityEngine;
using UnityEngine.UI;

//attached to all brushes

public class UIBrush : MonoBehaviour
{
    //set in Editor for each brush
    public Sprite baseImage;
    public Sprite hoverImage;


    private void Start()
    {
        doBase();
    }


    public void doBase()
    {
        GetComponent<Image>().sprite = baseImage;
    }


    public void doHover()
    {
        GetComponent<Image>().sprite = hoverImage;
    }
	
}
