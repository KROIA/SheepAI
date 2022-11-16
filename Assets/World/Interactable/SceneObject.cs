using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SaveLoadSystem;

[RequireComponent(typeof(SaveableEntity))]
[RequireComponent(typeof(Interactable))]
public class SceneObject : MonoBehaviour, ISaveable
{
    Interactable interactable;
    private void Awake()
    {
        interactable = GetComponent<Interactable>();
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }



    //------------------------------------
    // ISaveable implementation...
    //------------------------------------

    // Create a Serializable struct which contains all sorable data:
    // You don't need to save the location, rotation and scale, this will be done behind the scenes ;)
    [System.Serializable]
    struct InteractableData
    {
        public ObjectData data;
    }

    public object SaveState()
    {
        return new InteractableData()
        {
            data = interactable.GetObjectData()
        };
    }
    public void LoadState(object state)
    {
        InteractableData data = (InteractableData)state;
        interactable.SetObjectData(data.data);
    }

    public bool NeedsToBeSaved()
    {
        return true;
    }
    public bool NeedsReinstantiation()
    {
        return true;
    }

    public void PostInstantiation(object state)
    {
        //BlockData data = (BlockData)state;

    }
    public void GotAddedAsChild(GameObject obj, GameObject hisParent)
    {

    }
}
