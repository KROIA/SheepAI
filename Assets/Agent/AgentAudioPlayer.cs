using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Agent))]
[RequireComponent(typeof(AudioSource))]
public class AgentAudioPlayer : MonoBehaviour
{
    [SerializeField] List<AudioClip> availableSheepSounds = new List<AudioClip>();
    [SerializeField] List<AudioClip> killSounds = new List<AudioClip>();
    [SerializeField] List<AudioClip> eatSounds = new List<AudioClip>();
    [SerializeField] List<AudioClip> demageSounds = new List<AudioClip>();
    Agent agent;
    AudioSource audio;
    bool isAlive;
    float randomTime;
    float timeCount = 0;

    private void Awake()
    {
        randomTime = Random.Range(10, 50);
        agent = GetComponent<Agent>();
        audio = GetComponent<AudioSource>();
        agent.onEat += OnEat;
        agent.onKilled += OnKilled;
        agent.onReset += OnReset;
        agent.onTakeDemage += OnTakeDemage;
        isAlive = agent.IsAlive();
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(isAlive)
        {
            timeCount += Time.deltaTime;
            if(timeCount >= randomTime)
            {
                randomTime = Random.Range(10, 50);
                timeCount = 0;
                audio.clip = GetRandomSound(availableSheepSounds);
                SetRandomPitch();
                audio.Play();
            }
        }
    }

    void OnKilled()
    {
        audio.clip = GetRandomSound(killSounds);
        SetRandomPitch();
        audio.Play();
    }
    void OnReset()
    {
        isAlive = agent.IsAlive();
    }
    void OnEat()
    {
        audio.clip = GetRandomSound(eatSounds);
        SetRandomPitch(0.8f, 1.5f);
        audio.Play();
    }
    void OnTakeDemage()
    {
        audio.clip = GetRandomSound(demageSounds);
        SetRandomPitch(1, 1.3f);
        audio.Play();
    }

    AudioClip GetRandomSound(List<AudioClip> sounds)
    {
        return sounds[Random.Range(0, sounds.Count)];
    }
    void SetRandomPitch(float min = 0.8f, float max = 1.3f)
    {
        audio.pitch = Random.Range(min, max);
    }
}
