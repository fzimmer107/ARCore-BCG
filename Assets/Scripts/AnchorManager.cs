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
    public Button firstButton, secondButton, speakButton;
    
    public GameObject[] avatarModels;

 

    // Start is called before the first frame update
    void Start()
    {
        
        firstButton.onClick.AddListener(delegate { ChangeAvatar(m_SelectedAnchorAvatar,1);});
        secondButton.onClick.AddListener(delegate { ChangeAvatar(m_SelectedAnchorAvatar,2); });
        speakButton.onClick.AddListener(delegate { ToggleSpeaking(m_SelectedAnchorAvatar);});
        
       
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.touchCount == 2)
        {       
            //store the touches
            Touch touchZero = Input.GetTouch(0);
            Touch touchOne = Input.GetTouch(1);
            
            Vector2 touchZeroPrevPos = touchZero.position - touchZero.deltaPosition;
            Vector2 touchOnePrevPos = touchOne.position - touchOne.deltaPosition;
            
            float prevTouchDeltaMag = (touchZeroPrevPos - touchOnePrevPos).magnitude;
            float touchDeltaMag = (touchZero.position - touchOne.position).magnitude;
            
            float deltaMagnitudeDiff = prevTouchDeltaMag - touchDeltaMag;
            
            float v = m_SelectedAnchorAvatar.MScaleFactor * 100f;

            // negative delta is stretching, positive is pinching
            v -= deltaMagnitudeDiff * 1f;

            m_SelectedAnchorAvatar.MScaleFactor = Mathf.Clamp(v, 1f, 1000f) / 100f;
            
            m_SelectedAnchorAvatar.transform.localScale = m_SelectedAnchorAvatar.MOriginalScale *  m_SelectedAnchorAvatar.MScaleFactor;
        }
        
        
        if (Input.touchCount == 1)
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
                    
                    
                    if (Frame.Raycast(Input.GetTouch(0).position.x, Input.GetTouch(0).position.y, raycastFilter,
                        out trackableHit))
                    {
                        int oldAvatarIndex = m_SelectedAnchorAvatar.MAnchorAvatarIndex;
                        DestroyAnchor();
                        SpawnAnchor(Input.GetTouch(0),oldAvatarIndex); 
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
                            m_SelectedAnchorAvatar.MIsSelected = false;
                            Debug.Log("tap hit something!");
                            m_SelectedAnchorAvatar = hit.transform.parent.GetComponent<AnchorAvatar>();
                            m_SelectedAnchorAvatar.MIsSelected = true;
                            m_ActiveAnchor = hit.transform.parent.GetComponent<Anchor>();
                        }

                        else
                        {
                            if (m_SelectedAnchorAvatar != null)
                            {
                                Debug.Log("ich bin hier im != null segment");
                                m_SelectedAnchorAvatar.MIsSelected = false;
                                SpawnAnchor(Input.GetTouch(0));
                            }
                            if(m_SelectedAnchorAvatar == null)
                            {
                                Debug.Log("ich bin um == null segment");
                                SpawnAnchor(Input.GetTouch(0));    
                            }
                            
                        }

                    }
                    m_AcumTime = 0;
                }
            }
        }      
    }
     
    
    public void SpawnAnchor(Touch touch, int index = 0)
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
            m_SelectedAnchorAvatar.SpawnAvatar(avatarModels[index], m_ActiveAnchor.transform, index);  
        }
    }


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

    public void ToggleSpeaking(AnchorAvatar selectedAnchorAvatar)
    {
        Debug.Log("vor dem Touch war isSpeaking:" + selectedAnchorAvatar.MIsSpeaking );
        
        if (selectedAnchorAvatar.MIsSpeaking == false)
        {
            selectedAnchorAvatar.MIsSpeaking = !selectedAnchorAvatar.MIsSpeaking;
            selectedAnchorAvatar.MAnchorAvatarInstance.GetComponent<AudioSource>().Play();
        }
        else
        {
            selectedAnchorAvatar.MIsSpeaking = !selectedAnchorAvatar.MIsSpeaking;
            selectedAnchorAvatar.MAnchorAvatarInstance.GetComponent<AudioSource>().Stop();
        }
                
        Debug.Log("nach dem Touch ist isSpeaking:" + selectedAnchorAvatar.MIsSpeaking );
    }
    
    
}
   
    

