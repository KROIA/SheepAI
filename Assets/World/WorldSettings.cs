using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldSettings : MonoBehaviour
{
    [SerializeField] float timeScale = 1f;
    float oldTimeScale = 0;
    [SerializeField] float playGroundSize = 100;
    [SerializeField] bool m_agentVisualEffects = true;
    


    static WorldSettings instance = null;
    WorldSettings()
    {
        instance = this;
        
    }
    private void Awake()
    {
        Time.timeScale = timeScale;
    }

    private void Update()
    {
#if UNITY_EDITOR
        if(oldTimeScale != timeScale)
        {
            oldTimeScale = timeScale;
            Time.timeScale = timeScale;
        }
#endif
    }



    public static float GetPlaygroundSize()
    {
        if (!instance)
            return 0;
        return instance.playGroundSize;
    }

    public static bool agentVisualEffects
    {
        get
        {
            if (!instance)
                return false;
            return instance.m_agentVisualEffects;
        }
    }

}
