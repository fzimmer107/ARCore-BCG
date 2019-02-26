using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GoogleARCore;

public class AvatarController : MonoBehaviour
{
    private GameObject m_AvatarInstance;
    private int currentAvatarID;
    
    
    public GameObject[] avatarModels;
    

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }



    public void ChangeAvatar()
    {
        if (currentAvatarID < avatarModels.Length)
        {
            currentAvatarID++;
        }
        else
        {
            Debug.Log("Can't change avatarInstance");
        }
        
    }
    
}
