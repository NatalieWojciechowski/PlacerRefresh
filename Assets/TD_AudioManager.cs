using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TD_AudioManager : MonoBehaviour
{
    AudioSource AudioSource;

    public static TD_AudioManager instance;

    private void Awake()
    {
        if (instance == null) instance = this;
        else if (instance != this) Destroy(gameObject);

        DontDestroyOnLoad(gameObject);
    }

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void PlayClip(AudioClip clip, Vector3 atPosition)
    {
        AudioSource.PlayClipAtPoint(clip, atPosition);
    }
}
