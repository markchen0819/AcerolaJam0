using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameController : MonoBehaviour
{
    public AudioSource audioSource;
    public SceneEventSequencer sceneEventSequencer;
    public Player player;
    public GameUI gameUI;

    public GameObject TrackBase; // parent
    public GameObject[] starts;
    public GameObject[] targets;

    public int activeNoteCount = 20;

    // Song Data
    public float bpm = 170.0f;
    public float beatTempo = 2 / 4;
    public float songTotalSeconds = 124.0f;
    public float tolleranceOffset = 0.20f;
    public int beatToSkipBegin = 0;
    public int beatToSkipEnd = 0;

    [SerializeField]
    private int beatCount = 0;
    [SerializeField]
    private float secondsPerBeat = 0.0f;
    public float GetSecondsPerBeat() { return secondsPerBeat; }
    public int GetBeatToSkipBegin() { return beatToSkipBegin; }

    // Note Generation
    public GameObject NormalNotePrefab;
    public GameObject FlashNotePrefab;
    public GameObject BouncyNotePrefab;
    public GameObject DragNotePrefab;
    public GameObject SinNotePrefab;

    // Gameplay
    [SerializeField]
    private float timeline = 0.0f; // this gets sync with audio
    [SerializeField]
    private List<NoteBase> noteList = new List<NoteBase>();
    [SerializeField]
    private List<GameObject> noteGobjList = new List<GameObject>();
    [SerializeField]
    private int countNote = 0;
    [SerializeField]
    private float nextNoteTiming = 0.0f;
    private int previousRndNote = 0;
    private float endTimeCounter = 0.0f;
    private float endTime = 0.0f;
    private int hitCount = 0;

    private List<NoteBase> timingGroup1 = new List<NoteBase>();
    private List<NoteBase> timingGroup2 = new List<NoteBase>();
    void Awake()
    {
        Application.targetFrameRate = 144;
               PlayerPrefs.SetInt("HitCount", hitCount);
        // GenerateNote Info
        secondsPerBeat = 60f / (bpm);
        secondsPerBeat = secondsPerBeat / beatTempo;
        songTotalSeconds = audioSource.clip.length;
        beatCount = (int)(songTotalSeconds / secondsPerBeat) - beatToSkipBegin - beatToSkipEnd;
        GenerateNotes();
        endTime = secondsPerBeat * 6;

        // Track note count
        nextNoteTiming = (beatToSkipBegin+1) * secondsPerBeat;

        sceneEventSequencer.Init(this);
        sceneEventSequencer.CreateSceneEvents();

        player.Init(this);
    }

    private void Start()
    {
        PlayerPrefs.SetInt("HitCount", 0);
        audioSource.Play();
    }

    void Update()
    {
        float dt = Time.deltaTime;
        timeline += dt;
        SyncWithAudio();
        UpdateNoteIndex();
        CheckGameEnd();

        if(Input.GetKeyDown(KeyCode.F5))
        {
            SceneManager.LoadScene("Game");
        }
        if (Input.GetKeyDown(KeyCode.Backspace))
        {
            SceneManager.LoadScene("Result");
        }
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Application.Quit();
        }
    }
    public float GetTime()
    {
        return timeline;
    }
    void SyncWithAudio()
    {
        if(!audioSource.isPlaying)
        {
            // don't sync
        }
        else if(Mathf.Abs(audioSource.time - timeline) > tolleranceOffset)
        {
            // Stupid simple sync assuming no big offset
            timeline = (audioSource.time + timeline) * 0.5f;
        }
    }
    void GenerateNotes()
    {
        int noteIndex = 0;

        while (noteIndex < beatCount)
        {
            ++noteIndex;

            float beatTime = (noteIndex + beatToSkipBegin) * secondsPerBeat;
            int rnd = Random.Range(0, 3);
            Vector3 startPos = starts[rnd].transform.localPosition;
            Vector3 targetPos = targets[rnd].transform.localPosition;


            GameObject note = null;
            // Hardcode which beat num to change type
            bool mixMode = false;
            if (noteIndex >= 226)
            {
                note = Instantiate(NormalNotePrefab, transform.position, Quaternion.identity, TrackBase.transform);
            }
            else if (noteIndex >= 177)
            {
                mixMode = true;
                int rndNote = Random.Range(1, 5);
                while(rndNote == previousRndNote)
                {
                    rndNote = Random.Range(1, 5);
                }
                previousRndNote = rndNote;
                switch (rndNote)
                {
                    // Too boring to use normal note
                    //case 0:
                    //    note = Instantiate(NormalNotePrefab, transform.position, Quaternion.identity, TrackBase.transform);
                    //    break;
                    case 1:
                        note = Instantiate(DragNotePrefab, transform.position, Quaternion.identity, TrackBase.transform);
                        break;
                    case 2:
                        note = Instantiate(BouncyNotePrefab, transform.position, Quaternion.identity, TrackBase.transform);
                        break;
                    case 3:
                        note = Instantiate(FlashNotePrefab, transform.position, Quaternion.identity, TrackBase.transform);
                        break;
                    case 4:
                        note = Instantiate(SinNotePrefab, transform.position, Quaternion.identity, TrackBase.transform);
                        break;
                }
            }
            else if (noteIndex >= 144)
            {
                note = Instantiate(SinNotePrefab, transform.position, Quaternion.identity, TrackBase.transform);
            }
            else if (noteIndex >= 113)
            {
                note = Instantiate(FlashNotePrefab, transform.position, Quaternion.identity, TrackBase.transform);
            }
            else if (noteIndex >= 80)
            {
                note = Instantiate(BouncyNotePrefab, transform.position, Quaternion.identity, TrackBase.transform);
            }
            else if (noteIndex >= 48)
            {
                note = Instantiate(DragNotePrefab, transform.position, Quaternion.identity, TrackBase.transform);
            }
            else if (noteIndex >= 0)
            {
                note = Instantiate(NormalNotePrefab, transform.position, Quaternion.identity, TrackBase.transform);
            }

            note.name = "Note_" + (noteIndex - beatToSkipBegin + 1).ToString();
            note.transform.localPosition = startPos;
            noteGobjList.Add(note);

            NoteBase component = note.GetComponent<NoteBase>();
            noteList.Add(component);
            component.Init(this);

            if(mixMode)
            {
                //if (component is NormalNote)
                //{
                //    component.SetNoteData(beatTime, constSpeed, startPos, targetPos, secondsPerBeat);
                //}
                if (component is FlashNote fn)
                {
                    fn.SetNoteData(beatTime, 0.4f, startPos, targetPos, secondsPerBeat);
                    fn.CalculateFlashData();
                }
                if (component is BouncyNote bn)
                {
                    bn.SetNoteData(beatTime, 0.5f, startPos, targetPos, secondsPerBeat);
                    bn.CalculateBounceData();
                }
                if (component is DragNote dn)
                {
                    int secondRnd = Random.Range(0, 3);
                    while (secondRnd == rnd)
                    {
                        secondRnd = Random.Range(0, 3);
                    }
                    Vector3 overrideTargetPos = targets[secondRnd].transform.localPosition;
                    dn.SetNoteData(beatTime, 1.2f, startPos, overrideTargetPos, secondsPerBeat);
                }
                if (component is SinNote sn)
                {
                    sn.SetNoteData(beatTime, 0.6f, startPos, targetPos, secondsPerBeat);
                    sn.CalculateSinData();
                }
            }
            else
            {
                if (component is NormalNote)
                {
                    component.SetNoteData(beatTime, 0.5f, startPos, targetPos, secondsPerBeat);

                    if (noteIndex >= 226) 
                    {
                        timingGroup2.Add(component);
                    }
                }
                if (component is FlashNote fn)
                {
                    fn.SetNoteData(beatTime, 0.4f, startPos, targetPos, secondsPerBeat);
                    fn.CalculateFlashData();
                }
                if (component is BouncyNote bn)
                {
                    bn.SetNoteData(beatTime, 0.2f, startPos, targetPos, secondsPerBeat);
                    bn.CalculateBounceData();
                    if (noteIndex < 100) // 80~100
                    {
                        timingGroup1.Add(bn);
                    }
                }
                if (component is DragNote dn)
                {
                    int secondRnd = Random.Range(0, 3);
                    while (secondRnd == rnd)
                    {
                        secondRnd = Random.Range(0, 3);
                    }
                    Vector3 overrideTargetPos = targets[secondRnd].transform.localPosition;
                    dn.SetNoteData(beatTime, 1.0f, startPos, overrideTargetPos, secondsPerBeat);
                }
                if (component is SinNote sn)
                {
                    sn.SetNoteData(beatTime, 0.3f, startPos, targetPos, secondsPerBeat);
                    sn.CalculateSinData();

                }
            }
            note.SetActive(false);
        }

        for(int i=0; i<activeNoteCount;++i)
        {
            noteGobjList[i].SetActive(true);
        }
    }
    void UpdateNoteIndex()
    {
        if (timeline > nextNoteTiming)
        {
            ++countNote;
            if(countNote > beatCount)
            {
                countNote = beatCount - 1;
            }
            nextNoteTiming = (countNote + beatToSkipBegin - 1) * secondsPerBeat;

            int nextNoteToActiveIndex = activeNoteCount - 2 + countNote;
            if (nextNoteToActiveIndex < beatCount)
            {
                noteGobjList[nextNoteToActiveIndex].SetActive(true);
            }
        }
    }

    void CheckGameEnd()
    {
        if(!audioSource.isPlaying)
        {
            float dt = Time.deltaTime;
            endTimeCounter += dt;
        }
        if(endTimeCounter > endTime)
        {
            PlayerPrefs.SetInt("HitCount", hitCount);
            SceneManager.LoadScene("Result");
        }
    }
    public void TriggerHit(int hitCount)
    {
        this.hitCount = hitCount;
        gameUI.PlayHitUI(hitCount);
    }

    public List<NoteBase> GetTimingGroupOne()
    {
        return timingGroup1;
    }

    public List<NoteBase> GetTimingGroupTwo()
    {
        return timingGroup2;
    }


}
