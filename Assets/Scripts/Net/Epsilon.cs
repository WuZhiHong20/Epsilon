using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Live2D.Cubism.Framework;
using Live2D.Cubism.Framework.MouthMovement;

public class Epsilon : MonoBehaviour
{
    private CubismMouthController target;

    public AudioSource audioSource;

    [Range(0, 1)]
    public float mouthOpen;
    [Range(0, 1)]
    public float mouthMovingTime;

    bool isOpenging;

    private float curDur;
    // Start is called before the first frame update
    void Start()
    {
        curDur = 0f;
        isOpenging = true;
        mouthMovingTime = 0.1f;
        mouthOpen = 0.5f;
        target = GetComponent<CubismMouthController>();
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

    public void Greating(int MeetTime)
    {
        if (MeetTime < 1)
        {

        }
        if(MeetTime < 11)
        {

        }
    }

    public void Speaking(string audioPath)
    {
        ;
    }

    public void Speaking(AudioClip audioClip)
    {
        ;
    }

    // Update is called once per frame
    void Update()
    {

    }
}
