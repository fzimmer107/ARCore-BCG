using System;
using System.Collections;
using System.Collections.Generic;
using GoogleARCore;
using GoogleARCoreInternal;
using UnityEngine;
using UnityEngine.UI;

public class AnchorManager : MonoBehaviour
{

    private DetectedPlane m_SelectedPlane;
    private AnchorAvatar m_SelectedAnchorAvatar;    
    private GameObject m_AvatarInstance;
    private int m_CurrentAvatarIndex;
    private Anchor m_ActiveAnchor;
    private GameObject m_CurrentGameObject;

    public Button firstButton, secondButton, destroyButton;
    public GameObject[] avatarModels;

 

    // Start is called before the first frame update
    void Start()
    {
        firstButton.onClick.AddListener(delegate { ChangeAvatar(m_SelectedAnchorAvatar,1);});
        secondButton.onClick.AddListener(delegate { ChangeAvatar(m_SelectedAnchorAvatar,2); });
        destroyButton.onClick.AddListener(delegate {ChangeAvatar(m_SelectedAnchorAvatar);});
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void spawnAnchor()
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
            m_SelectedPlane = hit.Trackable as DetectedPlane;
            m_ActiveAnchor  = m_SelectedPlane.CreateAnchor(hit.Pose);
            m_SelectedAnchorAvatar = m_ActiveAnchor.gameObject.AddComponent<AnchorAvatar>();
            m_SelectedAnchorAvatar.SpawnAvatar(avatarModels[0], m_ActiveAnchor.transform);
            
        }
    }

    public bool CheckIfAnchorIsHit(Camera camera)
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
            m_SelectedAnchorAvatar = hit.transform.parent.GetComponent<AnchorAvatar>();
            m_ActiveAnchor = hit.transform.parent.GetComponent<Anchor>();
            return true;
            
        }

        return false;
    }
    
    public void ChangeAvatar(AnchorAvatar anchorAvatar, int avatarIndex = 0)
    {
        
        if (avatarIndex >= avatarModels.Length)
        {
            Debug.Log("avatarIndex ist: " + avatarIndex + "; array-length ist: " + avatarModels.Length);
        }

        if (anchorAvatar == null)
        {
            Debug.Log("Übergebener Anchor ist null");
        }

        else
        {
            Debug.Log("Wechsle Avatar");
            Destroy(anchorAvatar.MAnchorAvatarInstance);
            anchorAvatar.SpawnAvatar(avatarModels[avatarIndex], m_ActiveAnchor.transform, avatarIndex);
        }   
    }

    public void DestroyAnchor()
    {        
        DestroyImmediate(m_ActiveAnchor.gameObject);
        m_SelectedAnchorAvatar = null;
            
       /* 
        List<Anchor> beforeDetach = new List<Anchor>();
        List<Anchor> afterDetach = new List<Anchor>();
        
        m_SelectedPlane.GetAllAnchors(beforeDetach);
        

        Debug.Log("Vor dem detach ist count = " + beforeDetach.Count);
               
        //beforeDetach[0].m_NativeSession.AnchorApi.Detach(beforeDetach[0].m_NativeHandle); 
        
        DestroyImmediate(beforeDetach[0].gameObject);
        
        m_SelectedPlane.GetAllAnchors(afterDetach);
        
        Debug.Log("nach dem detach ist count = " + afterDetach.Count); */
    }
    
}
    

