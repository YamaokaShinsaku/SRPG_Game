using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    [SerializeField]
    private AudioSource audioSource;    //AudioSource
    // AudioClip
    [SerializeField]
    private AudioClip gameBGM;
    [SerializeField]
    private AudioClip cancel;
    [SerializeField]
    private AudioClip buttonMove;
    [SerializeField]
    private AudioClip damage;
    [SerializeField]
    private AudioClip diceid;
    [SerializeField]
    private AudioClip attack;

    public void GameBGM()
    {
        audioSource.PlayOneShot(gameBGM);
    }

    public void Cancel()
    {
        audioSource.PlayOneShot(cancel);
    }

    public void ButtonMove()
    {
        audioSource.PlayOneShot(buttonMove);
    }

    public void Damage()
    {
        audioSource.PlayOneShot(damage);
    }

    public void Diceid()
    {
        audioSource.PlayOneShot(diceid);
    }

    public void Attack()
    {
        audioSource.PlayOneShot(attack);
    }
}
