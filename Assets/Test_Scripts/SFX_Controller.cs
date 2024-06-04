using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/* ESTA CLASE NO SE ESTÁ UTILIZANDO ACTUALMENTE. SON LOS PROPIOS PERSONAJES LOS QUE REPRODUCEN ESTOS SONIDOS
 - VER PLAYER.CS - */


public class SFX_Controller : MonoBehaviour
{

    [SerializeField] private AudioClip goingUp;
    [SerializeField] private AudioClip goingDown;
    

    public static SFX_Controller instance;

    public enum Actions  { Subiendo, Bajando }

    private AudioSource _audioSource;

    private bool isAudioPlaying = false;
    private void Awake()
    {
        instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        _audioSource = GetComponent<AudioSource>();
    }

    public void PlayAudio(Actions act)
    {
        if (!isAudioPlaying)
        {
            AudioClip audioToPlay = null;
            switch (act)
            {
                case Actions.Subiendo:
                    audioToPlay = goingUp;
                    break;
                case Actions.Bajando:
                    audioToPlay = goingDown;
                    break;
            }

            StartCoroutine(PlaysAudioControlledly(audioToPlay));
        }
    }

    public void PlayAudio(AudioClip _clip)
    {
        _audioSource.PlayOneShot(_clip);
    }

    IEnumerator PlaysAudioControlledly(AudioClip _audio)
    {
        isAudioPlaying = true;
        _audioSource.PlayOneShot(_audio);
        yield return new WaitForSeconds(0.85f);
        isAudioPlaying = false;
    }
}
