using UnityEngine;
using System.Collections;

public class AudioManager : MonoBehaviour {

    static AudioManager instance;

    public static AudioManager Instance
    {
        get
        {
            return instance;
        }
    }

    [SerializeField] protected GameObject oneShotAudioSource;

    public void PlayOneShotClip(AudioClip clip)
    {
        this.StartCoroutine(PlayClipThenDestroyTempSource(clip));
    }

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }

    IEnumerator PlayClipThenDestroyTempSource(AudioClip clip)
    {
        var temp = new GameObject("Temp Clip");
        temp.transform.SetParent(oneShotAudioSource.transform);
        var source = temp.AddComponent<AudioSource>();
        source.spatialBlend = 0;
        source.clip = clip;
        source.Play();

        while (source.isPlaying)
        {
            yield return null;
        }

        Destroy(temp);
    }
	
}
