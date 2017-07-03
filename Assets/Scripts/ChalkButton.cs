using UnityEngine;
using UnityEngine.UI;

public class ChalkButton : MonoBehaviour
{
    public AudioClip audioClip;
    public AudioSource audioSource;
    
    void Awake()
    {
        
    }

    void Start()
    {
        var btn = GetComponent<Button>();
        btn.onClick.AddListener(TaskOnClick);
    }

    void TaskOnClick()
    {
        if (audioSource)
        {
            if (audioClip && audioSource.clip != audioClip)
            {
                audioSource.clip = audioClip;
            }
            if (audioSource.clip)
            {
                audioSource.Play();
            }
        }
    }
}
