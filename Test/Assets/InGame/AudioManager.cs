using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    [SerializeField] GeneralManager generalManager;
    [SerializeField] AudioSource audioSource;
    [SerializeField] AudioClip readySound;
    [SerializeField] AudioClip []simpleSound;
    [SerializeField] AudioClip endSound;
    
    bool isClosedEnd;
    public IEnumerator Start()
    {
        float sound = 0;
        audioSource.clip = readySound;
        audioSource.Play();
        for (int i = 0; i < 10; i++)
        {
            audioSource.volume = (sound += 0.04f);
            yield return new WaitForSeconds(0.2f);
        }
        
        yield return new WaitUntil(() => generalManager.GetIsRoomFull());

        for (int i = 0; i < 10; i++)
        {
            audioSource.volume = (sound -= 0.04f);
            yield return new WaitForSeconds(0.3f);
        }
        audioSource.loop = false;
        audioSource.Stop();
    }
    public void Update()
    {
        if (!audioSource.isPlaying)
        {
            if (generalManager.GetIsCloseEnd()) audioSource.clip = endSound; //버그 있음
            else audioSource.clip = simpleSound[Random.Range(0, simpleSound.Length)];
            StartCoroutine(nameof(SoundUp), audioSource);
        }
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
