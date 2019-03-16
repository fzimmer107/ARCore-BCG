using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEditor;
using UnityEngine;

public class AnchorAvatar : MonoBehaviour
{
    private GameObject m_AnchorAvatarInstance;
    private int m_AnchorAvatarIndex;
    private float m_ModelRotation = 180.0f;
    private Vector3 m_OriginalScale;
    private float m_ScaleFactor = 1.0f;
    
    private bool m_IsSelected;
    private bool m_IsSpeaking;
    private bool m_WaitingForInput;
    
    public bool MIsSelected
    {
        get { return m_IsSelected; }
        set { m_IsSelected = value; }
    }

    public bool MIsSpeaking
    {
        get { return m_IsSpeaking; }
        set { m_IsSpeaking = value; }
    }

    public bool MWaitingForInput
    {
        get { return m_WaitingForInput; }
        set { m_WaitingForInput = value; }
    }
    
    public int MAnchorAvatarIndex
    {
        get { return m_AnchorAvatarIndex; }
        set { m_AnchorAvatarIndex = value; }
    }

    public GameObject MAnchorAvatarInstance
    {
        get { return m_AnchorAvatarInstance;}
        set { m_AnchorAvatarInstance = value; }
    }

    public Vector3 MOriginalScale
    {
        get { return m_OriginalScale; }
        set { m_OriginalScale = value; }
    }

    public float MScaleFactor
    {
        get { return m_ScaleFactor; }
        set { m_ScaleFactor = value; }
    }

    public void SpawnAvatar(GameObject avatar, Transform parentTransform, int avatarIndex)
    {
        Debug.Log("Creating new Avatar Object");
        Debug.Log("position: " + parentTransform.position);
        Debug.Log("rotation; " + parentTransform.rotation);
        m_AnchorAvatarInstance = Instantiate(avatar, parentTransform.position, parentTransform.rotation, parentTransform);
        m_AnchorAvatarInstance.transform.Rotate(0, m_ModelRotation, 0, Space.Self);
        m_AnchorAvatarIndex = avatarIndex;
        m_OriginalScale = avatar.transform.localScale;
        m_IsSelected = true;
    }
    
    
    
    
}
