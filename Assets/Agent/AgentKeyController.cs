using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AgentKeyController : MonoBehaviour
{
    [SerializeField] float forwardForce = 100;
    [SerializeField] float angularForce = 25;
    Agent agent;
    Vector3 startPos;
    Vector3 startRot;
    // Start is called before the first frame update
    void Start()
    {
        agent = GetComponent<Agent>();
        startPos = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey(KeyCode.W))
        {
            agent.ForwardForce(forwardForce);
        }
        else if (Input.GetKey(KeyCode.S))
        {
            agent.ForwardForce(-forwardForce);
        }
        if (Input.GetKey(KeyCode.A))
        {
            agent.AngularForce(-angularForce);
        }
        else if (Input.GetKey(KeyCode.D))
        {
            agent.AngularForce(angularForce);
        }
        if(Input.GetKeyDown(KeyCode.G))
        {
            agent.godMode = !agent.godMode;
        }
        if (Input.GetKeyDown(KeyCode.H))
        {
            agent.displaySensors = !agent.displaySensors;
        }
        if (Input.GetKeyDown(KeyCode.Z))
        {
            GeneticNetMaster.ToggleAgentSensorDisplay();
        }
        if (Input.GetKeyDown(KeyCode.K))
        {
            agent.Kill();
        }
        if (Input.GetKeyDown(KeyCode.R))
        {
            transform.position = startPos;
            transform.eulerAngles = startRot;
            agent.ResetAgent();
        }
        agent.GetLidar().UpdateSensor();
        agent.UpdateAgent();
    }
    
    
}
