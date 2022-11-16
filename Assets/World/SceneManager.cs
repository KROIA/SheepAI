using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneManager : MonoBehaviour
{
    static SceneManager instance = null;

    SceneManager()
    {
        instance = this;
    }
    ~SceneManager()
    {
        if (instance == this)
            instance = null;
    }
    void Start()
    {
        SaveLoadSystem.SaveLoadSystem.SaveNew();
       /* SearchRecursive(currentlyInScene);
        for(int i=0; i<currentlyInScene.Count; ++i)
        {
            Interactable inter = Instantiate(currentlyInScene[i], backupParent.transform);
            inter.enabled = false;
            interactables.Add(inter);
        }*/
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public static void ResetScene()
    {
        SaveLoadSystem.SaveLoadSystem.Load();
        /*if (!instance)
            return;

        instance.SearchRecursive(instance.currentlyInScene);
        for (int i = 0; i < instance.currentlyInScene.Count; ++i)
        {
            Destroy(instance.currentlyInScene[i].gameObject);
        }
        for (int i = 0; i < instance.interactables.Count; ++i)
        {
            Interactable inter = Instantiate(instance.interactables[i], instance.sceneParent.transform);
            inter.enabled = true;
        }*/
    }
    /*void SearchRecursive(List<Interactable> saveIn)
    {
        saveIn.Clear();
        SearchRecursiveInObject(sceneParent, saveIn);
    }
    void SearchRecursiveInObject(GameObject obj, List<Interactable> saveIn)
    {
        Interactable[] list = obj.GetComponents<Interactable>();
        for(int i=0; i<list.Length; ++i)
        {
            //Interactable inter = Instantiate(list[i]);
            //inter.enabled = false;
            //interactables.Add(inter);
            saveIn.Add(list[i]);
        }
        int childCount = obj.transform.childCount;
        for(int i=0; i<childCount; ++i)
        {
            SearchRecursiveInObject(obj.transform.GetChild(i).gameObject, saveIn);
        }
    }*/
}
