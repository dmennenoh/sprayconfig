using UnityEngine;
using UnityEngine.SceneManagement;


public class ControllerInput : MonoBehaviour
{
    private SteamVR_Controller.Device controller;
    private SteamVR_TrackedObject trackedObj;
    private int clickCount;
    private Vector3[] clickPoints;
    private Main mainRef;
    private Quaternion modAngle;
    private bool isPainting;
   
    public GameObject cursor;   

    private Layers layers;

    private bool isMarkingCorners;

    private Color tintColor;
   // private CameraMover camMover;
    private bool camTriggerUp;
   

    void Start()
    {
        isMarkingCorners = false;

        clickCount = 0;//when this reaches 3 the corners are sent to Main
        clickPoints = new Vector3[4];

        trackedObj = GetComponent<SteamVR_TrackedObject>();
        controller = SteamVR_Controller.Input((int)trackedObj.index);

        mainRef = GameObject.Find("Main").GetComponent<Main>();
        layers = GameObject.Find("Main").GetComponent<Layers>();
        isPainting = false;        

        //modifies the forward vector of the controller
        modAngle = Quaternion.Euler(60, 0, 0);

        //camMover = GameObject.Find("ScreenCam").GetComponent<CameraMover>();
    }


    public Vector3[] points
    {
        set
        {
            clickPoints = value;
        }
    }


    void Update()
    {
        
        if (controller == null)
        {
            return;
        }

        if (isMarkingCorners && controller.GetHairTriggerDown())
        {
            if (clickCount < 3)
            {
                //locating monitor corners
                Vector3 cp = trackedObj.transform.position;
                clickPoints[clickCount] = cp;
                clickCount++;

                if (clickCount == 3)
                {
                    isMarkingCorners = false;

                    //fix so y vale of 0 and 1 are identical - take the avg
                    float newVal = (clickPoints[0].y + clickPoints[1].y) * .5f;
                    clickPoints[0].y = newVal;
                    clickPoints[1].y = newVal;

                    //find fourth point
                    //vector from upper right - back to upper left
                    Vector3 vBack = clickPoints[1] - clickPoints[0];
                    //lower right minus vBack to get lower left
                    clickPoints[3] = clickPoints[2] - vBack;

                    mainRef.buildScreen(clickPoints);
                }
            }
        }
        /*
        if (!isMarkingCorners && controller.GetHairTriggerDown())
        {
            if(camTriggerUp)
            {
                camTriggerUp = false;
                camMover.doReset();
            }
            camMover.okToMove = true;
        }
        if (!isMarkingCorners && controller.GetHairTriggerUp())
        {
            camTriggerUp = true;
            camMover.okToMove = false;
        }
        */
        Vector3 forward = transform.TransformDirection(modAngle * Vector3.forward);

        Ray theRay = new Ray(transform.position, forward);
        RaycastHit hit;
        bool didHit = Physics.Raycast(theRay, out hit, 6f);

        if (hit.collider != null)
        {
            //hit the main area - move the cursor
            float s = hit.distance * .25f;
            cursor.transform.localScale = new Vector3(s, s, s);
            cursor.transform.position = hit.point + layers.cursor;//Add the layer vector
            cursor.transform.rotation = Quaternion.LookRotation(layers.forward);
        }

        //either grip / side button
        if (controller.GetPress(Valve.VR.EVRButtonId.k_EButton_Grip))
        {
            //redo - start over
            //SceneManager.LoadScene(SceneManager.GetActiveScene().name);
            isMarkingCorners = true;
            clickCount = 0;//when this reaches 3 the corners are sent to Main
            clickPoints = new Vector3[4];
            mainRef.doReset();
        }

        //button above the d-pad
        if (controller.GetPress(Valve.VR.EVRButtonId.k_EButton_ApplicationMenu))
        {
            if (!isMarkingCorners)
            {
                mainRef.save();
            }
        }

        if (!isMarkingCorners && controller.GetHairTriggerDown())
        {
            //if (controller.GetPress(Valve.VR.EVRButtonId.k_EButton_System))
           
            mainRef.toggleLang();
        }

        if (!isMarkingCorners && controller.GetPress(Valve.VR.EVRButtonId.k_EButton_SteamVR_Touchpad))
        {
            float scaleAmount = .000002f;

            Vector2 touch = controller.GetAxis();
            //only use one axis for scaling
           if(Mathf.Abs(touch.x) > Mathf.Abs(touch.y))
            {
                if(touch.x < 0)
                {
                    scaleAmount *= -1;
                }
                mainRef.scaleUI(scaleAmount, 0f);
            }
            else
            {
                if (touch.y < 0)
                {
                    scaleAmount *= -1;
                }
                mainRef.scaleUI(0f, scaleAmount);
            }

        }

    }//Update()
}