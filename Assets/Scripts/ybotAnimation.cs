using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ybotAnimation : MonoBehaviour
{
    private Animator m_Animator;
    private AudioSource m_AudioSource;
    private float m_AudioClipLength;
    private float m_CurrentAudioLength;
    private AnchorAvatar m_AnchorAvatar;
    private bool m_InputReceived;
    private GameObject m_AnswerPanel;
    private int m_CurrentIndex;

    public List<AudioClip> audioClips;
    public List<AudioClip> defaultPhrases;
    
    // Start is called before the first frame update
    void Start()
    {
        m_AnswerPanel = BuildInputPanel();
        m_AnswerPanel.SetActive(false);
        m_CurrentIndex = 0;
        
        m_AnchorAvatar = gameObject.GetComponentInParent<AnchorAvatar>();
        m_AudioSource = gameObject.GetComponent<AudioSource>();
        m_AudioClipLength = m_AudioSource.clip.length;
        m_Animator = gameObject.GetComponent<Animator>();
        m_Animator.SetBool("isAvatarSelected", gameObject.GetComponentInParent<AnchorAvatar>().MIsSelected);
        m_Animator.SetBool("isSpeakButtonPressed", gameObject.GetComponentInParent<AnchorAvatar>().MIsSpeaking);
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        m_Animator.SetBool("isAvatarSelected", m_AnchorAvatar.MIsSelected);
        m_Animator.SetBool("isSpeakButtonPressed", m_AnchorAvatar.MIsSpeaking);
        m_Animator.SetBool("isWaitingForInput", m_AnchorAvatar.MWaitingForInput);

        if (m_AnchorAvatar.MIsSpeaking)
        {
            m_CurrentAudioLength += Time.deltaTime;

            if (m_CurrentAudioLength >= m_AudioClipLength)
            {
                if (m_AnchorAvatar.MWaitingForInput)
                {
                    m_AnswerPanel.SetActive(true);
                    
                }
                else
                {
                    m_Animator.SetBool("isLectureFinished", true);
                    m_AudioSource.clip = defaultPhrases[0];
                    m_AnchorAvatar.MIsSpeaking = false;
                    m_CurrentAudioLength = 0;
                    m_AnchorAvatar.MWaitingForInput = true;
                    m_AudioClipLength = m_AudioSource.clip.length;
                }
            }
        }
        else
        {
            m_CurrentAudioLength = 0;
        }
    }

    private void StayOnCurrentLecture(AnchorAvatar anchorAvatar)
    {
        Debug.Log("STAYING ON CURRENT LECTURE");
        m_AudioSource.clip = audioClips[m_CurrentIndex];
        m_AudioClipLength = m_AudioSource.clip.length;
        m_AnswerPanel.SetActive(false);
        anchorAvatar.MIsSpeaking = false;
        anchorAvatar.MWaitingForInput = false;
        m_Animator.SetBool("isLectureFinished", false);
        m_CurrentAudioLength = 0;
        m_InputReceived = true;
    }

    private void MoveToNextLecture(AnchorAvatar anchorAvatar)
    {
        if (m_CurrentIndex + 1 < audioClips.Count)
        {
            m_CurrentIndex++;
            Debug.Log("MOVING TO NEXT LECTURE");
            m_AnswerPanel.SetActive(false);
            m_AudioSource.clip = audioClips[m_CurrentIndex];
            anchorAvatar.MIsSpeaking = false;
            anchorAvatar.MWaitingForInput = false;
            m_Animator.SetBool("isLectureFinished", false);
            m_CurrentAudioLength = 0;
            m_AudioClipLength = m_AudioSource.clip.length;
            m_InputReceived = true;
        }
        else
        {
            Debug.Log("YOU REACHED THE LAST LECTURE");
            m_AnswerPanel.SetActive(false);
            m_AudioSource.clip = audioClips[m_CurrentIndex];
            anchorAvatar.MIsSpeaking = false;
            anchorAvatar.MWaitingForInput = false;
            m_Animator.SetBool("isLectureFinished", false);
            m_CurrentAudioLength = 0;
            m_AudioClipLength = m_AudioSource.clip.length;
            m_InputReceived = true;
        }
        

    }


    private GameObject BuildInputPanel()
    {
        m_InputReceived = false;
        GameObject panel = Instantiate(Resources.Load("panelAnswer")) as GameObject;
        
        panel.transform.SetParent(GameObject.Find("Canvas").transform,false);
        
        Button btnYes = GameObject.Find("btnYes").GetComponent<Button>();
        Button btnNo = GameObject.Find("btnNo").GetComponent<Button>();
        
        btnYes.onClick.AddListener(delegate { StayOnCurrentLecture(m_AnchorAvatar); });
        btnNo.onClick.AddListener(delegate { MoveToNextLecture(m_AnchorAvatar); });

        return panel;
    }
}