using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnchorAvatar : MonoBehaviour
{
    private GameObject m_AnchorAvatarInstance;
    private int m_AnchorAvatarIndex;
    private float m_ModelRotation = 180.0f;

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

    public void SpawnAvatar(GameObject avatar, Transform parentTransform, int avatarIndex = 0)
    {
        Debug.Log("Creating new Avatar Object");
        Debug.Log("position: " + parentTransform.position);
        Debug.Log("rotation; " + parentTransform.rotation);
        m_AnchorAvatarInstance = Instantiate(avatar, parentTransform.position, parentTransform.rotation, parentTransform);
        m_AnchorAvatarInstance.transform.Rotate(0, m_ModelRotation, 0, Space.Self);
        m_AnchorAvatarIndex = avatarIndex; 
    }

}
