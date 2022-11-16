using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Agent : MonoBehaviour
{
    [SerializeField] float maxVelocity = 10;
    [SerializeField] float maxAngularVelocity = 10;
    [SerializeField] float neededFoodPerSpeed = 0.001f;
    [SerializeField] float starvingHealthLoss = 0.01f;
    [SerializeField] float foodPerHealth = 1;

    [SerializeField] float health = 10;
    float startHealth;
    [SerializeField] float food = 10;
    float startFood;
    [SerializeField] bool isDeath = false;
    [SerializeField] bool m_godMode = false;
    bool wasGod = false;
    [SerializeField] bool m_displaySensors;
    bool lastDisplaySensors;
    
    [SerializeField] Color deadColor;
    Color healthyColor;
    [SerializeField] Color m_godModeColor;
    

    [SerializeField] MeshRenderer bodyRenderer;
    Material bodyMaterial;

    Rigidbody rigidBody;
    [SerializeField] Transform body;
    [SerializeField] Lidar lidar;
    [SerializeField] BoxCollider bodyCollider;
    Vector3 bodyColliderOrigninalSize;

    
    Vector3 deltaV;
    Vector3 deltaAV;
    bool colliding = false;

    static List<Agent> allAgents = new List<Agent>();

    public System.Action onKilled;
    public System.Action onReset;
    public System.Action onEat;
    public System.Action onTakeDemage;

    private void Awake()
    {
        allAgents.Add(this);
        startFood = food;
        startHealth = health;
        bodyColliderOrigninalSize = bodyCollider.size;

        // bodyRenderer.material = new Material(bodyRenderer.material);
        bodyMaterial = bodyRenderer.material;
        healthyColor = bodyMaterial.color;
        rigidBody = GetComponent<Rigidbody>();
        deltaV = Vector3.zero;
        deltaAV = Vector3.zero;
    }
    private void OnDestroy()
    {
        allAgents.Remove(this);
    }

    void Start()
    {
        
       
        EnableGodMode(m_godMode);
        EnableDisplaySensors(m_displaySensors);
    }

    // Update is called once per frame
    void Update()
    {


        if (isDeath)
            return;
        if(transform.eulerAngles.x != 0 || transform.eulerAngles.z != 0)
        {
            Vector3 rot = transform.eulerAngles;
            rot.x = 0;
            rot.z = 0;
            transform.eulerAngles = rot;
        }
        if(transform.position.y > 0.0)
        {
            Vector3 pos = transform.position;
            pos.y = 0.00f;
            transform.position = pos;
        }
    }
    public void UpdateAgent()
    {
#if UNITY_EDITOR
        if (m_godMode != wasGod)
            EnableGodMode(m_godMode);
        if (m_displaySensors != lastDisplaySensors)
            EnableDisplaySensors(m_displaySensors);
#endif
        if (isDeath)
            return;
        ApplyNewVelocity();

        if (health <= 0)
        {
            Kill();
        }
        if (WorldSettings.agentVisualEffects)
            AjustBodyVisuals();
    }
    private void OnCollisionEnter(Collision collision)
    {
        
        Interactable interactable = collision.collider.GetComponent<Interactable>();
        if(interactable)
        {
            //colliding = true;
            ObjectData data = interactable.GetObjectData();
            if (!m_godMode)
            {
                if(health < startHealth)
                {
                    float deltaFood = (startHealth - health) * foodPerHealth;
                    if (deltaFood > data.foodValue)
                        deltaFood = data.foodValue;
                    health += deltaFood / foodPerHealth;
                    data.foodValue -= deltaFood;
                }
                if (data.foodValue > 0 && onEat != null)
                    onEat();
                food += data.foodValue;
                if (data.poisonValue > 0 && onTakeDemage != null)
                    onTakeDemage();

                health -= data.poisonValue;
                if (data.doesKill)
                {
                    Kill();
                }
            }
            if(data.consumable)
                Destroy(interactable.gameObject);

        }
        
    }
    private void OnCollisionExit(Collision collision)
    {
       // colliding = false;

    }

    public void Kill()
    {
        if (m_godMode)
            return;
        bodyMaterial.color = deadColor;
        isDeath = true;
        health = 0;
        bodyCollider.material.dynamicFriction = 2;
        rigidBody.constraints = 0;
        lidar.enabled = false;

        if (onKilled != null)
            onKilled();


    }
    public bool IsAlive()
    {
        return !isDeath;
    }
    public void ResetAgent()
    {
        rigidBody.velocity = Vector3.zero;
        rigidBody.angularVelocity = Vector3.zero;
        
        rigidBody.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
        
        isDeath = false;
        health = startHealth;
        food = startFood;
        deltaV = Vector3.zero;
        deltaAV = Vector3.zero;
        lidar.enabled = true;

        if (onReset != null)
            onReset();
    }
    public void EnableGodMode(bool enable)
    {
        m_godMode = enable;
        bodyCollider.material.dynamicFriction = 0;
        isDeath = false;
        food = startFood;
        health = startHealth;
        bodyMaterial.color = m_godModeColor;
        wasGod = m_godMode;
    }
    public bool godMode
    {
        get
        {
            return m_godMode;
        }
        set
        {
            EnableGodMode(value);
        }
    }
    public void EnableDisplaySensors(bool enable)
    {
        m_displaySensors = enable;
        lidar.EnableSensorDisplay(m_displaySensors);

        lastDisplaySensors = m_displaySensors;
    }
    public bool displaySensors
    {
        get
        {
            return m_displaySensors;
        }
        set
        {
            EnableDisplaySensors(value);
        }
    }
    public void ForwardForce(float force)
    {
        AddForce(transform.forward * force);
    }
    public void AngularForce(float force)
    {
        AddTorque(transform.up * force);
    }
    public void AddForce(Vector3 force)
    {
        //rigidBody.AddForce(force);
        deltaV += force;
    }
    public void AddTorque(Vector3 torque)
    {
        deltaAV += torque;
        //rigidBody.AddTorque(torque);

    }
    void AjustBodyVisuals()
    {
        float scale = Mathf.Lerp(0.1f, 1, food/startFood);
        if (scale > 1)
            scale = 1;
        if (scale < 0.1f)
            scale = 0.1f;
        Vector3 sc = body.localScale;
        sc.x = scale;
        body.localScale = sc;
        bodyCollider.size = new Vector3(bodyColliderOrigninalSize.x * scale,
                                        bodyColliderOrigninalSize.y,
                                        bodyColliderOrigninalSize.z);

        if (!m_godMode)
        {
            Color bodyColor = Color.Lerp(deadColor, healthyColor, health / startHealth);
            bodyMaterial.color = bodyColor;
        }
    }
    void ApplyNewVelocity()
    {
        if(colliding)
        {
            deltaAV = Vector3.zero;
            deltaV = Vector3.zero;
            return;
        }
        rigidBody.velocity += deltaV * Time.deltaTime;
       // Vector3 deltaPos = (deltaV + rigidBody.velocity) * WorldSettings.GetDeltaT();
       // Vector3 deltaAngle = (deltaAV + rigidBody.angularVelocity ) * WorldSettings.GetDeltaT();
        rigidBody.angularVelocity += deltaAV * Time.deltaTime;
        //float dotP = Vector3.Dot(deltaPos, transform.forward);
        float dotP = Vector3.Dot(rigidBody.velocity, transform.forward);

        //float vel = deltaPos.magnitude;
        float vel = rigidBody.velocity.magnitude;
        if (vel > maxVelocity)
            vel = maxVelocity;
        //rigidBody.velocity = rigidBody.velocity * maxVelocity / vel;

        float sumVel = vel;

        if (dotP >= 0)
            rigidBody.velocity = transform.forward * vel;
        else
            rigidBody.velocity = transform.forward * -vel;

        //vel = rigidBody.angularVelocity.magnitude;
        vel = rigidBody.angularVelocity.magnitude;
        
        if (vel > maxAngularVelocity)
        {
            //rigidBody.angularVelocity = rigidBody.angularVelocity * maxAngularVelocity / vel;
            rigidBody.angularVelocity = rigidBody.angularVelocity * maxAngularVelocity / vel;
            vel = maxAngularVelocity;
        }

        
        sumVel += vel;
        //transform.position = transform.position + deltaPos;
        //transform.eulerAngles = transform.eulerAngles + deltaAngle;

        if (!m_godMode)
        {
            if (food > 0)
                food -= sumVel * neededFoodPerSpeed;
            else
            {
                health -= (sumVel+0.1f)*starvingHealthLoss * Time.deltaTime;
            }
        }

        deltaAV = Vector3.zero;
        deltaV = Vector3.zero;
        //rigidBody.velocity = Vector3.zero;
        //rigidBody.angularVelocity = Vector3.zero;
    }
    public Lidar GetLidar()
    {
        return lidar;
    }

    public float GetFood()
    {
        return food;
    }
    public float GetHealth()
    {
        return health;
    }

    public static void EnableAllSensor(bool enable)
    {
        foreach (Agent agent in allAgents)
        {
            if (enable && agent.isDeath)
                continue;
            agent.EnableDisplaySensors(enable);
        }
    }
}
