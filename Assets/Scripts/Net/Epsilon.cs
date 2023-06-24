using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Live2D.Cubism.Framework;
using Live2D.Cubism.Framework.MouthMovement;

public class Epsilon : MonoBehaviour
{
    private CubismMouthController target;

    private AudioSource audioSource;

    [Range(0, 1)]
    public float mouthOpen;
    [Range(0, 1)]
    public float mouthMovingTime;

    bool isOpenging;

    private float curDur;

    private Dictionary<string, AudioClip> dicAudio;

    private List<AudioClip> readyToPlay;
    // Start is called before the first frame update
    void Start()
    {
        Logger.Init();
        curDur = 0f;
        isOpenging = true;
        mouthMovingTime = 0.1f;
        mouthOpen = 0.5f;
        target = GetComponent<CubismMouthController>();
        audioSource = GetComponent<AudioSource>();
        dicAudio = new Dictionary<string, AudioClip>();
        readyToPlay = new List<AudioClip>();
    }

    private AudioClip LoadAudio(string path)
    {
        return (AudioClip)Resources.Load(path);
    }

    private AudioClip GetAudio(string path)
    {
        if (dicAudio.ContainsKey(path) == false)
        {
            dicAudio[path] = LoadAudio(path);
        }
        return dicAudio[path];
    }

    private IEnumerator OpenMouth()
    {
        while (audioSource.isPlaying)
        {
            curDur += Time.deltaTime;
            if (isOpenging)
            {
                if (curDur > mouthMovingTime)
                {
                    target.MouthOpening = Mathf.Lerp(0, mouthOpen, mouthMovingTime);
                    curDur = 0;
                    isOpenging = false;
                    mouthMovingTime = Random.Range(0.1f, 0.4f);
                    mouthOpen = mouthMovingTime + Random.Range(0f, 0.6f);
                }
                else
                {
                    target.MouthOpening = Mathf.Lerp(0, mouthOpen, curDur);
                }

            }
            else
            {
                if (curDur > mouthMovingTime)
                {
                    target.MouthOpening = Mathf.Lerp(mouthOpen, 0, mouthMovingTime);
                    curDur = 0;
                    isOpenging = true;
                    mouthMovingTime = Random.Range(0.1f, 0.4f);
                    mouthOpen = mouthMovingTime + Random.Range(0f, 0.6f);
                    //Debug.Log(mouthMovingTime);
                }
                else
                {
                    target.MouthOpening = Mathf.Lerp(mouthOpen, 0, curDur);
                }
            }
            yield return null;
        }
        curDur = 0;
        target.MouthOpening = 0;
    }

    public void StartMoveMouth()
    {
        StartCoroutine(OpenMouth());
    }

    private void BeforePlayRightNow()
    {
        if (audioSource.isPlaying == false)
        {
            audioSource.Stop();
            Logger.Log("正在播放中，已取消");
        }
        readyToPlay.Clear();
    }

    public void Greating(int MeetTime)
    {
        if (MeetTime < 1)
        {
            Speaking(AudioKeeper.AudioLibrary[AudioKeeper.AudioIndex.MIDNIGHTING]);
        }
        else if(MeetTime < 4)
        {
            Speaking(AudioKeeper.AudioLibrary[AudioKeeper.AudioIndex.DEEPNIGHTGREETING]);
        }
        else if(MeetTime < 7)
        {
            Speaking(AudioKeeper.AudioLibrary[AudioKeeper.AudioIndex.DAWNGREETING]);
        }
        else if(MeetTime < 11)
        {
            Speaking(AudioKeeper.AudioLibrary[AudioKeeper.AudioIndex.MORNINGGREETING]);
        }
        else if (MeetTime < 14)
        {
            Speaking(AudioKeeper.AudioLibrary[AudioKeeper.AudioIndex.NOONGREETING]);
        }
        else if (MeetTime < 19)
        {
            Speaking(AudioKeeper.AudioLibrary[AudioKeeper.AudioIndex.AFERNOONGREETING]);
        }
        else if (MeetTime < 23)
        {
            Speaking(AudioKeeper.AudioLibrary[AudioKeeper.AudioIndex.NIGHTGREETING]);
        }
        else
        {
            Speaking(AudioKeeper.AudioLibrary[AudioKeeper.AudioIndex.MIDNIGHTING]);
        }
    }

    public void Speaking(string audioPath, bool playRightNow = false)
    {
        readyToPlay.Add(GetAudio(audioPath));
        if (playRightNow == true)
        {
            BeforePlayRightNow();
        }
    }

    public void Speaking(AudioClip audioClip, bool playRightNow = false)
    {
        readyToPlay.Add(audioClip);
        if(playRightNow == true)
        {
            BeforePlayRightNow();
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(audioSource.isPlaying == false && readyToPlay.Count > 0)
        {
            audioSource.clip = readyToPlay[0];
            audioSource.Play();
            readyToPlay.RemoveAt(0);
            StartMoveMouth();
        }
// Debug.Log(audioSource.isPlaying);
    }
}
