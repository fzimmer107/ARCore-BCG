using System.Collections;
using System.Collections.Generic;
using GoogleARCore;
using UnityEngine;

public class AnchorManager : MonoBehaviour
{
    public GameObject anchorGameObject;
    public Camera camera;

    private GameObject m_AnchorInstance;
    private List<Anchor> m_AllAnchors = new List<Anchor>();
    private GameObject m_ActiveGameObject;
    private Anchor m_ActiveAnchor;
    private int m_ActiveAnchorIndex;
    private List<Trackable> m_allTrackables = new List<Trackable>();


    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (CheckIfAnchorIsHit())
        {
            Debug.Log("Ray hit something!");
        }
        else
        {
            spawnAnchor();
        }
    }

    void selectAnchor()
    {

    }

    void changeActiveAnchor()
    {

    }

    void spawnAnchor()
    {
        Touch touch;
        //check if the user touched the screen
        if (Input.touchCount < 1 || (touch = Input.GetTouch(0)).phase != TouchPhase.Began)
        {
            return;
        }

        TrackableHit hit;
        TrackableHitFlags raycastFilter = TrackableHitFlags.PlaneWithinBounds | TrackableHitFlags.PlaneWithinPolygon;

        if (Frame.Raycast(touch.position.x, touch.position.y, raycastFilter, out hit))
        {

            m_AnchorInstance = Instantiate(anchorGameObject, hit.Pose.position, hit.Pose.rotation);

            var anchor = Session.CreateAnchor(hit.Pose);

            m_AnchorInstance.transform.parent = anchor.transform;
        }
    }

    private bool CheckIfAnchorIsHit()
    {
        Touch touch;
        if (Input.touchCount < 1 || (touch = Input.GetTouch(0)).phase != TouchPhase.Began)
        {
            return false;
        }

        //construct ray
        RaycastHit hit;
        Ray ray = camera.ScreenPointToRay(new Vector3(touch.position.x, touch.position.y));

        if (Physics.Raycast(ray, out hit))
        {
            m_ActiveGameObject = hit.transform.gameObject;
            return true;
        }

        return false;

    }
}

/*     for (var i = 0; i < Input.touchCount; i++)
     {
         if(Input.GetTouch(i).phase == TouchPhase.Began)
         {
             //construct ray
             RaycastHit hit;
             Ray ray = camera.ScreenPointToRay((Input.GetTouch(i).position));
 
             if (Physics.Raycast(ray, out hit))
             {
                 return true;                
             }
         }
     }
                
     return false;  
 } */
    

