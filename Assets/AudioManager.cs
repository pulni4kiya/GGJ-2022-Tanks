using UnityEngine.Audio;
using UnityEngine;

public class AudioManager : MonoBehaviour
{

    public Sound[] sounds;

    // Start is called before the first frame update
    void Awake ()
    {
        foreach (Sound s in sounds)
        {
            s.sourse = gameObject.AddComponent<AudioManager>();
            s.sourse = s.slip;

            s.sourse.valume = s.volume;
            s.sourse.pitch = s.pitch;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
