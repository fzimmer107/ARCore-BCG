using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using GoogleARCore;
using GoogleARCoreInternal;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class AnchorManager : MonoBehaviour
{

    private DetectedPlane m_SelectedPlane;
    private AnchorAvatar m_SelectedAnchorAvatar;     
    private Anchor m_ActiveAnchor;
    private float m_HoldTime = 0.8f;
    private float m_AcumTime; //default zero
    private bool m_AnchorMoveMode; //default false
    private bool m_IsHold; //default false

    
    public Camera firstPersonCamera;
    public Button firstButton, secondButton;
    public GameObject[] avatarModels;

 

    // Start is called before the first frame update
    void Start()
    {
        firstButton.onClick.AddListener(delegate { ChangeAvatar(m_SelectedAnchorAvatar,1);});
        secondButton.onClick.AddListener(delegate { ChangeAvatar(m_SelectedAnchorAvatar,2); });
       
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.touchCount > 0)
        {            
            m_AcumTime += Input.GetTouch(0).deltaTime;
            
            if (m_AcumTime >= m_HoldTime && m_IsHold == false)
            {          
                m_IsHold = true;
                Debug.Log("Setting m_IsHold to " + m_IsHold);
                return;
            }

            if (Input.GetTouch(0).phase == TouchPhase.Ended)
            {
                if(EventSystem.current.IsPointerOverGameObject(Input.GetTouch(0).fingerId)
                    || EventSystem.current.currentSelectedGameObject != null)
                {
                    Debug.Log(EventSystem.current.currentSelectedGameObject);
                    return;
                }
                
                if (m_AnchorMoveMode)
                {
                    TrackableHit trackableHit;
                    TrackableHitFlags raycastFilter =
                        TrackableHitFlags.PlaneWithinBounds | TrackableHitFlags.PlaneWithinPolygon;
                    
                    Debug.Log("Anchor position before Change: x: "+ m_ActiveAnchor.transform.position.x + 
                              " y: " + m_ActiveAnchor.gameObject.transform.position.y + "z: " + m_ActiveAnchor.gameObject.transform.position.z);
                    
                    if (Frame.Raycast(Input.GetTouch(0).position.x, Input.GetTouch(0).position.y, raycastFilter,
                        out trackableHit))
                    {
                        m_ActiveAnchor.transform.position = trackableHit.Pose.position;
                        //m_SelectedAnchorAvatar.MAnchorAvatarInstance.transform.position = trackableHit.Pose.position;
                        m_SelectedAnchorAvatar.MAnchorAvatarInstance.SetActive(true);
                        
                        Debug.Log("Anchor position after Change: x: "+ m_ActiveAnchor.gameObject.transform.position.x + 
                                  " y: " + m_ActiveAnchor.gameObject.transform.position.y + "z: " + m_ActiveAnchor.gameObject.transform.position.z);
                        
                        m_AnchorMoveMode = false;
                    }
                }
                else
                {
                    RaycastHit hit;
                    if (m_IsHold)
                    {
                        if (CheckIfAnchorIsHit(firstPersonCamera, Input.GetTouch(0), out hit))
                        {
                            Debug.Log("long tap hit something!");
                            m_SelectedAnchorAvatar = hit.transform.parent.GetComponent<AnchorAvatar>();
                            m_ActiveAnchor = hit.transform.parent.GetComponent<Anchor>();
                            m_SelectedAnchorAvatar.MAnchorAvatarInstance.SetActive(false);
                            m_AnchorMoveMode = true;
                        }

                        m_IsHold = false;
                    }
                    else
                    {
                        if (CheckIfAnchorIsHit(firstPersonCamera, Input.GetTouch(0), out hit))
                        {
                            Debug.Log("tap hit something!");
                            m_SelectedAnchorAvatar = hit.transform.parent.GetComponent<AnchorAvatar>();
                            m_ActiveAnchor = hit.transform.parent.GetComponent<Anchor>();
                        }

                        else
                        {
                            SpawnAnchor(Input.GetTouch(0));
                        }

                    }
                    m_AcumTime = 0;
                }
            }
        }      
    }
     
    public void SpawnAnchor(Touch touch)
    {
    /*    Touch touch;
        //check if the user touched the screen
        if (Input.touchCount < 1 || (touch = Input.GetTouch(0)).phase != TouchPhase.Began)
        {
            return;
        } */

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

  /*  public bool CheckIfAnchorIsHit(Camera camera)
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
    }*/

    public bool CheckIfAnchorIsHit(Camera camera, Touch touch, out RaycastHit raycastHit)
    {
        Ray ray = camera.ScreenPointToRay(new Vector3(touch.position.x, touch.position.y));
        return Physics.Raycast(ray, out raycastHit);
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
    

