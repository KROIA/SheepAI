using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Interactable : MonoBehaviour
{
    [SerializeField] MeshRenderer colorParent;
    Collider collider;


    [SerializeField] protected ObjectData objectData;
    [SerializeField] public static List<Interactable> interactableObjects = null;


    private void Awake()
    {
        if (interactableObjects == null)
        {
            interactableObjects = new List<Interactable>();
        }
        interactableObjects.Add(this);
        collider = GetComponent<Collider>();
    }
    // Start is called before the first frame update
    protected virtual void Start()
    {
        
        //objectData = new ObjectData();
        if(!colorParent)
            colorParent = GetComponent<MeshRenderer>();
        if(!colorParent)
        {
            colorParent = GetComponentInChildren<MeshRenderer>();
        }
    }
    private void OnDestroy()
    {
        interactableObjects.Remove(this);
    }

    // Update is called once per frame
    protected virtual void Update()
    {
        
    }
    public Collider GetCollider()
    {
        if (collider == null)
            Debug.LogError("Collider is null");
        return collider;
    }
    public Color GetColor()
    {
        if(colorParent)
            return colorParent.material.color;
        return Color.white;
    }

    public ObjectData GetObjectData()
    {
        return objectData;
    }
    public void SetObjectData(ObjectData data)
    {
        objectData = data;
    }


}
