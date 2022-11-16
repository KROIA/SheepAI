using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserSensor : MonoBehaviour
{
    [SerializeField] float m_sensorViewDistance = 100;

    public struct SensorData
    {
        public Interactable obj;
        public float distance;
        public float normalizedDistance;
    }

    Material material;
    [SerializeField] Color defaultColor;
    SensorData sensorData;
    [SerializeField] MeshRenderer renderer;
    bool noDisplay = false;

    

    // Start is called before the first frame update
    void Start()
    {
        
        MeshRenderer renderer = GetComponentInChildren<MeshRenderer>();
        material = new Material(renderer.material);
        renderer.material = material;
    }

    // Update is called once per frame
    void Update()
    {

        
    }
    public void UpdateSensor()
    {
        sensorData = GetNearestObject();
        if (noDisplay)
            return;
        if (sensorData.obj)
        {
            SetSensorLength(sensorData.distance);
        }
        else
        {
            SetSensorLength(m_sensorViewDistance);
        }
        SetColor(sensorData.obj);
    }
    public float sensorViewDistance
    {
        get
        {
            return m_sensorViewDistance;
        }
        set
        {

            m_sensorViewDistance = value;
            if (m_sensorViewDistance < 0.1f)
                m_sensorViewDistance = 0.1f;
        }
    }
    void SetSensorLength(float l)
    {
        Vector3 scale = transform.localScale;
        scale.z = l;
        transform.localScale = scale;
    }
    SensorData GetNearestObject()
    {
        SensorData info;
        info.obj = null;
        info.distance = 0;
        info.normalizedDistance = 0;
        if (Interactable.interactableObjects.Count == 0)
            return info;
        Ray ray = new Ray(transform.position, transform.forward);
        RaycastHit hit;
        float minDistance = m_sensorViewDistance;

        foreach(Interactable obj in Interactable.interactableObjects)
        {
            if (obj.GetCollider().Raycast(ray, out hit, m_sensorViewDistance))
            {

                if(minDistance > hit.distance)
                {
                    minDistance = hit.distance;
                    info.obj = obj;
                    info.distance = minDistance;
                }
            }
        }
        info.normalizedDistance = info.distance / m_sensorViewDistance;
        return info;
    }
    void SetColor(Interactable obj)
    {
        if (obj)
            material.color = obj.GetColor();
        else
            material.color = defaultColor;
    }

    public SensorData GetSensorData()
    {
        return sensorData;
    }

    public void EnableSensorDisplay(bool enable)
    {
        renderer.enabled = enable;
        noDisplay = !enable;
    }
    public bool SensorDisplayIsEnabled()
    {
        return !noDisplay;
    }
}
