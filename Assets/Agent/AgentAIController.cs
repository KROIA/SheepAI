using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using NeuronalNet;
public class AgentAIController : MonoBehaviour
{
    [SerializeField] float forwardForce = 100;
    [SerializeField] float angularForce = 25;
    [SerializeField] float score = 0;
    [SerializeField] bool printNetworkInput = false;
    [SerializeField] bool printNetworkOutput = false;
    Vector3 startPos;
    Agent agent = null;
    Net net = null;

    Vector3 lastPos;

    // Start is called before the first frame update
    void Awake()
    {
        agent = GetComponent<Agent>();
    }
    
    public void SetNet(Net net)
    {
        this.net = net;
    }
    public void ResetAgent(Vector3 pos, Vector3 rotation)
    {
        score = 0;
        transform.position = pos;
        transform.eulerAngles = rotation;
        lastPos = pos;
        startPos = pos;
        agent.ResetAgent();
    }

    // Update is called once per frame
    void Update()
    {
        if (net == null || agent == null)
            return;
        if (!agent.IsAlive())
            return;

        agent.GetLidar().UpdateSensor();
        List<float> inputs = GetInputVector();
        if(printNetworkInput)
        {
            string inpString = "";
            for(int i=0; i<inputs.Count; ++i)
                inpString += inputs[i].ToString()+ ", ";
            Debug.Log(inpString);

        }
            
        net.SetInputVector(inputs);
        net.Calculate();
        
        ProcessOutputVector(net.GetOutputVector());
        score += GetDeltaScore();
        agent.UpdateAgent();



        if (printNetworkOutput)
        {
            float[] outputs = net.GetOutputVector().ToArray();
            string outString = "";
            for (int i = 0; i < outputs.Length; ++i)
                outString += outputs[i].ToString() + ", ";
            Debug.Log(outString);

        }
    }

    public bool IsAlive()
    {
        return agent.IsAlive();
    }

    List<float> GetInputVector()
    {
        List<LaserSensor> sensors = agent.GetLidar().GetLasers();
        List<float> inputs = new List<float>();
        for(int i=0; i<sensors.Count; ++i)
        {
            LaserSensor.SensorData sensorData = sensors[i].GetSensorData();
            float viewFood = 0;
            float viewPoison = 0;
            float viewKill = 0;
            float viewDistance = 0;
            if(sensorData.obj)
            {
                ObjectData objData = sensorData.obj.GetObjectData();
                viewFood = objData.foodValue;
                viewPoison = objData.poisonValue;
                viewKill = objData.doesKill? 1: 0;
                viewDistance = 1-sensorData.normalizedDistance;
                viewDistance *= viewDistance;
            }
            inputs.Add(Mathf.Clamp(viewFood * viewDistance, 0, 1));
            inputs.Add(Mathf.Clamp(viewPoison * viewDistance, 0, 1));
            inputs.Add(Mathf.Clamp(viewKill * viewDistance, 0, 1));
            inputs.Add(Mathf.Clamp(viewDistance,0,1));
        }
        return inputs;
    }
    
    void ProcessOutputVector(SignalVector signals)
    {
        agent.ForwardForce(signals.GetElement(0) * forwardForce);
        agent.AngularForce(signals.GetElement(1) * angularForce);
    }
    public static int GetInputCountPerSensor()
    {
        return 4;
    }
    public int GetInputCount()
    {
        if (!agent)
            return 0;
        if (agent.GetLidar().GetLaserCount() == 0)
            agent.GetLidar().Setup();
        return agent.GetLidar().GetLaserCount() * GetInputCountPerSensor();
    }
    public static int GetOutputCount()
    {
        return 2;
    }

    float GetDeltaScore()
    {
        return 0;

        float deltaScore = 0;
        float distance = (transform.position - lastPos).magnitude;
        lastPos = transform.position;
        deltaScore += distance;

        return deltaScore;
    }
    public float GetScore()
    {
        float absDistanceScore = (transform.position - startPos).sqrMagnitude;
        float foodScore = agent.GetFood();
        float healthScore = agent.GetHealth();
        return score + absDistanceScore + foodScore*10 + healthScore * 10; ;
    }
}
