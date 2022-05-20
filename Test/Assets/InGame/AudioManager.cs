using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour //¹Ì¿Ï
{
    [SerializeField] GeneralManager generalManager;
    [SerializeField] AudioSource readySound;
    [SerializeField] AudioSource []simpleSound1;
    [SerializeField] AudioSource endSound;
    AudioClip audioClip;

    bool isStart;
    bool isClosedEnd;
    public IEnumerator Start()
    {
        float sound = 0;
        readySound.Play();
        for (int i = 0; i < 10; i++)
        {
            readySound.volume = (sound += 0.04f);
            yield return new WaitForSeconds(0.2f);
        }
        
        yield return new WaitUntil(() => generalManager.GetIsRoomFull());
        for (int i = 0; i < 10; i++)
        {
            readySound.volume = (sound -= 0.04f);
            yield return new WaitForSeconds(0.3f);
        }
        readySound.Stop();
        
    }
    public void Update()
    {
        if (generalManager.GetIsCreatePlayer())
        {
            StartCoroutine(nameof(SoundUp), simpleSound1[0]);
        }
        if (isClosedEnd)
        {
            endSound.Play();
        }
    }
    public void ChangeMusic()
    {
        readySound.Stop();
        //playSound1.Play();
    }
    public IEnumerator SoundUp(AudioSource audio)
    {
        float startSound = 0;
        audio.Play();
        for (int i = 0; i < 10; i++)
        {
            audio.volume = (startSound += 0.04f);
            yield return new WaitForSeconds(0.2f);
        }
    }
    
    public IEnumerator SoundDown(AudioSource audio)
    {
        float startSound = audio.volume;
        for (int i = 0; i < 10; i++)
        {
            audio.volume = (startSound -= 0.04f);
            yield return new WaitForSeconds(0.3f);
        }
        audio.Stop();
    }
    
}
