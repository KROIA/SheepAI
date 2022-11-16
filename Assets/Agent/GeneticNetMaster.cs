using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using NeuronalNet;
public class GeneticNetMaster : MonoBehaviour
{
    static GeneticNetMaster instance;

    [SerializeField] int agentCount = 10;
    [SerializeField] AgentAIController aiAgentPrefab;
    [SerializeField] ulong maxIterations = 1000;
    [SerializeField] ulong iterationCounter = 0;

    [SerializeField] ulong hiddenX = 1;
    [SerializeField] ulong hiddenY = 10;
    [SerializeField] Net.Activation activation;
    [SerializeField] float mutationChance = 0.3f;
    [SerializeField] float mutationFactor = 0.01f;
    [SerializeField] string netSavePath = "netSave.net";
    [SerializeField] bool loadAtStart = false;
    [SerializeField] bool saveToFile = true;
    [SerializeField] bool enableDisplayAgentSensors = false;
    bool lastEnableDisplayAgentSensors = false;

    NetSerializer netSerializer = new NetSerializer();

    // [SerializeField] bool waitUntilLastDead = false;

    List<AgentAIController> agents = new List<AgentAIController>();
    GeneticNet net;
    // Start is called before the first frame update
    void Start()
    {
        instance = this;
        net = new GeneticNet((ulong)agentCount);
        
        for (int i=0; i<agentCount; ++i)
        {
            AgentAIController controller = Instantiate(aiAgentPrefab, transform);
            controller.SetNet(net.GetNet((ulong)i));
            agents.Add(controller);
        }
        ResetAgents();

        net.SetActivation(activation);
        ulong inputs = (ulong)agents[0].GetInputCount();
        if (inputs == 0)
            Debug.LogError("Inputs must be larger than 0");
        net.SetDimensions(inputs, hiddenX, hiddenY, (ulong)AgentAIController.GetOutputCount());
        net.SetMutationChance(mutationChance);
        net.SetMutationFactor(mutationFactor);
        net.Build();
        if (loadAtStart)
        {
            LoadNet();
            if(net.GetInputCount() != inputs)
            {
                Debug.LogError("Net inputs count not compatible with the save file.\n"+
                               "The Simulation uses: "+inputs+" inputs and the save file offers: "+ net.GetInputCount());
            }
            if (net.GetOutputCount() != (ulong)AgentAIController.GetOutputCount())
            {
                Debug.LogError("Net outputs count not compatible with the save file.\n" +
                               "The Simulation uses: " + (ulong)AgentAIController.GetOutputCount() +
                               " outputs and the save file offers: " + net.GetOutputCount());
            }
            hiddenX = net.GetHiddenXCount();
            hiddenY = net.GetHiddenYCount();
        }
        SaveNet();


    }
    private void OnDestroy()
    {
        if (instance == this)
            instance = null;
    }


    // Update is called once per frame
    void Update()
    {
#if UNITY_EDITOR
        if (instance.lastEnableDisplayAgentSensors != instance.enableDisplayAgentSensors)
        {
            instance.lastEnableDisplayAgentSensors = instance.enableDisplayAgentSensors;
            Agent.EnableAllSensor(instance.enableDisplayAgentSensors);
        }
#endif
        //if (waitUntilLastDead)
        {
            bool anyAlive = false;
            foreach(AgentAIController agent in agents)
            {
                anyAlive |= agent.IsAlive();
            }
            if (!anyAlive)
            {
                Learn();
                return;
            }
        }
        //else
        {
            ++iterationCounter;
            if (iterationCounter < maxIterations / Time.timeScale)
                return;
            Learn();
        }
        
    }
    void Learn()
    {
        iterationCounter = 0;
        List<float> scores = new List<float>();
        float averageScore = 0;
        foreach (AgentAIController agent in agents)
        {
            averageScore += agent.GetScore();
            scores.Add(agent.GetScore());
        }
        averageScore /= agents.Count;
        Debug.Log("Average score: " + averageScore);
        net.Learn(scores);
        if (saveToFile)
            SaveNet();
        ResetScene();
        

    }

    public static void ToggleAgentSensorDisplay()
    {
        if (!instance) return;
        //if (instance.lastEnableDisplayAgentSensors != instance.enableDisplayAgentSensors)
        {
            instance.enableDisplayAgentSensors = !instance.enableDisplayAgentSensors;
            instance.lastEnableDisplayAgentSensors = instance.enableDisplayAgentSensors;
            Agent.EnableAllSensor(instance.enableDisplayAgentSensors);
        }
    }

    public void SaveNet()
    {
        netSerializer.SetFilePath(netSavePath);
        if (!netSerializer.SaveToFile(net))
            Debug.LogWarning("Can't save to file: " + netSavePath);
    }
    public void LoadNet()
    {
        netSerializer.SetFilePath(netSavePath);
        if (!netSerializer.ReadFromFile(net))
            Debug.LogWarning("Can't load from file: " + netSavePath);

        // After loading a file, the net pointers must be set again, because they got reinstantiated
        for (int i = 0; i < agents.Count; ++i)
        {
            agents[i].SetNet(net.GetNet((ulong)i));
        }
        //ResetScene();
    }


    public void ResetScene()
    {
        ResetAgents();
        SceneManager.ResetScene();
    }



    void ResetAgents()
    {
        foreach(AgentAIController agent in agents)
            agent.ResetAgent(GetRandomPosition(), GetRandomRotation());
    }

    Vector3 GetRandomPosition()
    {
        float size = WorldSettings.GetPlaygroundSize()*3f/7f;
        float randomAngle = Random.Range(0f, 360f);
        return (Quaternion.AngleAxis(randomAngle, Vector3.up) * transform.forward) * Mathf.Lerp(0,size, Random.Range(0f, 1f));
    }
    Vector3 GetRandomRotation()
    {
        return new Vector3(0, Random.Range(0, 360), 0);
    }
}
