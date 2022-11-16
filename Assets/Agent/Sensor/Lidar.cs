using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lidar : MonoBehaviour
{
    [SerializeField] LaserSensor laserPrefab;
    [SerializeField] int laserCount = 4;
    [SerializeField] float radius = 0.5f;
    [SerializeField] float laserViewDistance = 25;
    [SerializeField] float vieldOfView = 360;

    bool displaySensors = true;
    List<LaserSensor> lasers = new List<LaserSensor>();
    // Start is called before the first frame update
    void Awake()
    {
        Setup();
    }

    public void Setup()
    {
        InstantiateLasers();
        ArrangeInCircle();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void UpdateSensor()
    {
        foreach (LaserSensor sensor in lasers)
            sensor.UpdateSensor();
    }
    private void OnDisable()
    {
        displaySensors = lasers[0].SensorDisplayIsEnabled();
        for (int i = 0; i < lasers.Count; ++i)
        {
            lasers[i].EnableSensorDisplay(false);
        }
    }
    private void OnEnable()
    {
        if(lasers.Count > 0)
            EnableSensorDisplay(displaySensors);
    }

    public float sensorViewDistance
    {
        get
        {
            return laserViewDistance;
        }
        set
        {

            laserViewDistance = value;
            if (laserViewDistance < 0.1f)
                laserViewDistance = 0.1f;
            foreach (LaserSensor sensor in lasers)
                sensor.sensorViewDistance = laserViewDistance;
        }
    }
    void InstantiateLasers()
    {
        if (lasers.Count > 0)
            return;
        if (laserCount <= 0)
            laserCount = 1;
        for (int i=0; i<laserCount; ++i)
        {
            LaserSensor laser = Instantiate(laserPrefab, transform);
            laser.sensorViewDistance = laserViewDistance;
            lasers.Add(laser);
        }
    }
    void ArrangeInCircle()
    {
        float deltaAngle;
        if (lasers.Count == 1)
            deltaAngle = 0;
        else
            deltaAngle = vieldOfView / (lasers.Count - 1);
        float angleOffset = -vieldOfView/2f;
        for (int i = 0; i < lasers.Count; ++i)
        {
            float angle = deltaAngle * i + angleOffset;
            //alternator *= -1;
            LaserSensor laser = lasers[i];
            Vector3 offset = (Quaternion.AngleAxis(angle, Vector3.up) * transform.forward) * radius;
            Vector3 rot = laser.transform.eulerAngles;
            rot.Set(rot.x, angle, rot.z);
            laser.transform.eulerAngles = rot;
            laser.transform.position = transform.position + offset;
        }
    }

    public int GetLaserCount()
    {
        return lasers.Count;
    }
    public LaserSensor GetLaser(int index)
    {
        if (index >= lasers.Count)
            return null;
        return lasers[index];
    }
    public List<LaserSensor> GetLasers()
    {
        return lasers;
    }
    public void EnableSensorDisplay(bool enable)
    {
        displaySensors = enable;
        for (int i = 0; i < lasers.Count; ++i)
        {
            lasers[i].EnableSensorDisplay(enable);
        }
    }
}
