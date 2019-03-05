using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Internal.Experimental.UIElements;
using UnityEngine.UI;

public class avatarAuwaehlen : MonoBehaviour 
{
    public GameObject panel;

    public Button getBtn;

    //public InputField mainInputField;

    public BaseEventData m_BaseEventData;
     
    public void tooglePanel(){
        
        getBtn = GameObject.Find("btnAvatarAuswahlen").GetComponent<Button>();
        
        Debug.Log("button aktiv?"+getBtn.IsActive());
                
        if (panel!= null && getBtn.IsActive())
        {
            bool isActiv = panel.activeSelf;
            panel.SetActive(!isActiv);
            //ChangeSelectionColor();
            //Update();
            
            Debug.Log("panel aktiv? "+ panel.activeSelf);
            if (panel.activeSelf)
            {
                
               
            }

            Debug.Log(" isInteractable? "+getBtn.IsInteractable());
            
         
        }
       
    }
    
    /*
    public  void ChangeSelectionColor()
    {
        mainInputField.selectionColor = Color.red;
    }
    
    BaseEventData m_BaseEvent;

    void Update()
    {       
        //Check if the GameObject is being highlighted
        if (IsHighlighted(m_BaseEvent) == true)
        {
            //Output that the GameObject was highlighted, or do something else
            Debug.Log("Selectable is Highlighted");
        }
        else
        {
            Debug.Log("nicht highlighted");
        }
    }*/

}
