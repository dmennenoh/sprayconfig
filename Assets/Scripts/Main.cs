using UnityEngine;
using UnityEngine.UI;

public class Main : MonoBehaviour
{
    private GameObject screenCam;
    private Material mat;
    private Layers layers;//ref to the layers class attached to Main go
    private GameObject hitParent;
    private FileScript fileScript;
    private Vector3 norml;
    private Vector3 midPoint;
    private Mesh mesh;
    private Vector3[] pointData;

    void Start ()
    {
        screenCam = GameObject.Find("ScreenCam");

        mat = Resources.Load("GridMat") as Material;

        layers = GetComponent<Layers>();

        hitParent = new GameObject("hitParent");

        fileScript = GetComponent<FileScript>();
        
        buildScreen(fileScript.readConfigFile());
        GameObject.Find("langText").GetComponent<Text>().text = fileScript.spanish ? "spanish" : "english";
    }

    //called from controllerInput.update if the controller system button is pressed
    public void toggleLang()
    {
        fileScript.toggleLang();
        GameObject.Find("langText").GetComponent<Text>().text = fileScript.spanish ? "spanish" : "english";
    }

    /**
     * Builds the screen mesh given the four corner points
     * called from Start() using config file from disk
     *  when called from Start() the array will have 5 entries - 4 points, ui scale
     *  
     * Called from controllerInput.update once all four corners are found
     *  when called from controller the array will have just the 4 corner points
     */
    public void buildScreen(Vector3[] clickPoints)
    {
        mesh = new Mesh();

        GetComponent<MeshFilter>().mesh = mesh;

        pointData = new Vector3[4];
        pointData[0] = clickPoints[0];
        pointData[1] = clickPoints[1];
        pointData[2] = clickPoints[2];
        pointData[3] = clickPoints[3];        

        mesh.vertices = pointData;

        mesh.triangles = new int[] 
        {
            0, 1, 3,
            1, 2, 3
        };

        mesh.uv = new Vector2[4]
        {
            new Vector2(0,0),
            new Vector2(1,0),
            new Vector2(1,1),
            new Vector2(0,1)
        };        

        GetComponent<MeshCollider>().sharedMesh = mesh;
        
        Renderer r = GetComponent<Renderer>();
        r.material = mat;

        //use topleft and topRight corners to calculate the mid point vector
        Vector3 topMid = clickPoints[0] + ((clickPoints[1] - clickPoints[0]) * .5f);

        //and then move down to midway between topRight and bottomRight
        midPoint = topMid + ((clickPoints[2] - clickPoints[1]) * .5f);

        //calculate normal vector to the surface
        Vector3 c1 = clickPoints[1] - clickPoints[0];
        Vector3 c2 = clickPoints[2] - clickPoints[0];
        norml = Vector3.Cross(c1, c2).normalized;

        //set the forward vector in layers - for layer offsets
        layers.forward = norml;

        //calculate the aspect ratio based on the rect size
        float wide = Vector3.Distance(clickPoints[0], clickPoints[1]);
        float high = Vector3.Distance(clickPoints[1], clickPoints[2]);
        float asp = wide / high;

        screenCam.GetComponent<Camera>().aspect = asp;

        //Move camera away from surface by some arbitrary amount...
        Vector3 result = midPoint + layers.forward * 3f;        
        screenCam.transform.position = result;

        //Point the camera at center sceen
        screenCam.transform.LookAt(midPoint);

        //Calculate required FOV to make the mesh full screen
        Vector3 from = (result - midPoint).normalized;
        Vector3 to = (result - topMid).normalized;
        float ang = Vector3.Angle(from, to);
        screenCam.GetComponent<Camera>().fieldOfView = ang * 2;       

        //Move UI
        GameObject t = GameObject.Find("UIScaler");
        Vector3 uip = midPoint + layers.ui;
        t.transform.position = uip;
        t.transform.rotation = Quaternion.LookRotation(layers.forward);

        //from file load instead of clicking if length > 4
        if (clickPoints.Length > 4)
        {            
            t.transform.localScale = clickPoints[4];
        }
        
    }


    private void Update()
    {
        Debug.DrawRay(midPoint, norml);
    }


    public void doReset()
    {
        Destroy(mesh);
    }

    //Called from ControllerInput when touchpad is used to scale the UI
    public void scaleUI(float xs, float ys)
    {
        Transform t = GameObject.Find("UIScaler").transform;
        Vector3 sc = t.localScale;
        sc.x = sc.x + xs;
        sc.y = sc.y + ys;
        t.localScale = sc;
    }
    

    //Called from ControllerInput when applicationMenu button on controller is pressed
    public void save()
    {
        Transform t = GameObject.Find("UIScaler").transform;
        Vector3 sc = t.localScale;
        fileScript.writeFile("screenLoc.txt", pointData, sc, fileScript.spanish);

        Application.Quit();
    }
}
