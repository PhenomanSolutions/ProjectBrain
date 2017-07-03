using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class MaskLoadScript : MonoBehaviour
{

    public GameObject MaskObject;
    public string TriggerName;
    public float Duration = 0.2f;
    public string SceneToLoad;

	// Use this for initialization
	void Start () {
        var btn = GetComponent<Button>();
        btn.onClick.AddListener(OnClickHandler);
    }
	
	// Update is called once per frame
	void Update ()
	{

	}

    void OnClickHandler()
    {
        var animator = MaskObject.GetComponent<Animator>();
        if (animator)
        {
            animator.SetTrigger(TriggerName);
            if (!string.IsNullOrEmpty(SceneToLoad))
            {
                Invoke("LoadTargetScene", Duration);
            }
        }
    }

    void LoadTargetScene()
    {
        Debug.Log("CALLED !!!");
    }
}
