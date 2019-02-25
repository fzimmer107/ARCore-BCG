using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using GoogleARCore;
using TMPro;
using Anchor = GoogleARCore.Anchor;
using Debug = UnityEngine.Debug;
using Trackable = GoogleARCore.Trackable;

public class SceneController : MonoBehaviour
{
    public Camera firstPersonCamera;
    public GameObject avatar;


    private GameObject m_AvatarInstance;
    private List<DetectedPlane> m_AllDetectedPlanes = new List<DetectedPlane>();

    
    // Start is called before the first frame update
    void Start()
    {
        QuitOnConnectionErrors();
    }

    // Update is called once per frame
    void Update()
    {
        
        if (Session.Status == SessionStatus.None)
        {
            Debug.Log("Can not find Session");
        }
        
        //ARCore is not tracking...
        else if (Session.Status == SessionStatus.NotTracking)
        {
            //...set a sleep timer for the screen            
            int lostTrackingSleepTimeout = 15;
            Screen.sleepTimeout = lostTrackingSleepTimeout;
            return;
        }
        else if (Session.Status == SessionStatus.Tracking)
        {
            //ARCore is tracking, dont allow sleeping
            Screen.sleepTimeout = SleepTimeout.NeverSleep;
        }

        
        ProcessTouches(); 
    }

    /*
     * check if permission to camera is granted and ARCore can connect to ARCore services
     */
    void QuitOnConnectionErrors()
    {
        if (Session.Status == SessionStatus.ErrorPermissionNotGranted)
        {
            Debug.Log("Camera permission is not granted");
        }
        else if(Session.Status.IsError())
        {
            Debug.Log("An error has occured. Please restart the app");
        }
    }

    void ProcessTouches()
    {
        Touch touch;
        //check if the user touched the screen
        if(Input.touchCount < 1 || (touch = Input.GetTouch(0)).phase != TouchPhase.Began)
        {
            return;
        }

        TrackableHit hit;
        TrackableHitFlags raycastFilter = TrackableHitFlags.PlaneWithinBounds | TrackableHitFlags.PlaneWithinPolygon;

        if (Frame.Raycast(touch.position.x, touch.position.y, raycastFilter, out hit))
        {
            
            m_AvatarInstance = Instantiate(avatar, hit.Pose.position, hit.Pose.rotation);
            
            var anchor = Session.CreateAnchor(hit.Pose);

            m_AvatarInstance.transform.parent = anchor.transform;
        }
    }

    public void ClearArCoreSession()
    {
         StartCoroutine(DeleteArCoreSession());
         StartCoroutine(AddNewArCoreSession());
    }

    IEnumerator DeleteArCoreSession()
    {
        ARCoreSession arCoreSession = GameObject.Find("ARCore Device").GetComponent<ARCoreSession>();
        Debug.Log(arCoreSession);
       
        Session.GetTrackables(m_AllDetectedPlanes);
        
        Debug.LogWarning("trackable count is: " + m_AllDetectedPlanes.Count);
        
        m_AllDetectedPlanes.Clear();
        DestroyImmediate(arCoreSession);
 
        yield break;
    }

    IEnumerator AddNewArCoreSession()
    {
           ARCoreSession arCoreSession = GameObject.Find("ARCore Device").AddComponent<ARCoreSession>();
           arCoreSession.SessionConfig = Resources.Load<ARCoreSessionConfig>("DefaultSessionConfig");
           arCoreSession.enabled = true;
           
           Session.GetTrackables(m_AllDetectedPlanes);
           Debug.LogWarning("trackable count after destroy is: " + m_AllDetectedPlanes.Count);
           yield break;
    }

}
