using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : UnitySingleton<AudioManager>
{
    private AudioSource _musicAudio = null;
    private int _maxEffects = 10;//允许最多同时播放10个音效

    private AudioClip _nowMusicClip = null;
    private bool _nowMusicLoop = true;
    
    private Queue<AudioSource> _effectAudiosQue=new Queue<AudioSource>();


    public override void Awake()
    {
        base.Awake();
        this._musicAudio = gameObject.AddComponent<AudioSource>();

        for (int i = 0; i < _maxEffects; i++)
        {
            AudioSource source = gameObject.AddComponent<AudioSource>();
            this._effectAudiosQue.Enqueue(source);
        }

    }

    public void PlayMusic(AudioClip clip,bool loop=true)
    {
        if (this._musicAudio==null||clip==null||
            (this._musicAudio.clip!=null&&
             this._musicAudio.clip.name==clip.name))
        {
            return;
        }

        this._nowMusicClip = clip;
        this._nowMusicLoop = loop;
        
        this._musicAudio.clip = clip;
        this._musicAudio.loop = loop;
        this._musicAudio.volume = 1.0f;
        this._musicAudio.Play();
    }

    public void StopMusic()
    {
        if (this._musicAudio==null
            ||this._musicAudio.clip==null)
        {
            return;
        }
        this._musicAudio.Stop();
    }

    public AudioSource PlayEffect(AudioClip clip)
    {
        AudioSource source = this._effectAudiosQue.Dequeue();
        
        source.clip = clip;
        source.volume = 1.0f;
        source.Play();
        
        this._effectAudiosQue.Enqueue(source);
        return null;
    }

    public void EnableMusic(bool enable)
    {
        if (this._musicAudio==null||this._musicAudio.enabled==enable)
        {
            return;
        }
        this._musicAudio.enabled = enable;
        if (enable)
        {
            this._musicAudio.clip = this._nowMusicClip;
            this._musicAudio.loop = this._nowMusicLoop;
            this._musicAudio.Play();
        }
    }

    public void EnableEffect(bool enable)
    {
        AudioSource[] effectSet = this._effectAudiosQue.ToArray();
        for (int i = 0; i < this._effectAudiosQue.Count; i++)
        {
            if (effectSet[i].enabled==enable)
            {
                continue;
            }

            effectSet[i].enabled = enable;
            if (enable)
            {
                effectSet[i].clip = null;
            }
        }
    }

}
