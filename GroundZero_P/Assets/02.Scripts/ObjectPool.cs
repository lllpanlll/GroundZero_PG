using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ObjectPool : MonoBehaviour{

    GameObject[] objectPool;
    int objectNum;
    public int ObjectNum { get{ return objectNum; } }

    public void CreatePool(GameObject original, int num)
    {
        Dispose();

        objectPool = new GameObject[num];
        objectNum = num;

        for (int i = 0; i < objectNum; i++)
        {
            objectPool[i] = GameObject.Instantiate(original);
            objectPool[i].SetActive(false);
        }
    }

    void OnDestory()
    {
        Dispose();
    } 

    public GameObject UseObject()
    {
        if (objectPool == null)
            return null;

        for (int i = 0; i < objectNum; i++)
        {
            if (objectPool[i].activeSelf == false)
            {
                objectPool[i].SetActive(true);

                return objectPool[i];
            }
        }
        return null;
    }
      
    public void RemoveObject(GameObject gameObject)
    {
        if (objectPool == null || gameObject == null)
            return;

        for (int i = 0; i < objectNum; i++)
        {
            if (objectPool[i].gameObject == gameObject)
            {
                objectPool[i].SetActive(false);
                break;
            }
        }
    }

    public GameObject DetectiveAllObject(int Index)
    {
        if (Index > objectNum && Index < 0)
            return null;

        if (objectPool[Index].activeSelf == false)
            return null;

        return objectPool[Index];
    }


    public void AllDeActiveObject()
    {
        if (objectPool == null)
            return;

        for (int i = 0; i < objectNum; i++)
        {
            if (objectPool != null && objectPool[i].activeSelf == true)
                objectPool[i].SetActive(false);
        }
    }

    public void Dispose()
    {
        if (objectPool == null)
            return;

        for (int i = 0; i < objectNum; i++)
        {
            Destroy(objectPool[i].gameObject);
        }
        objectPool = null;
    }

    public bool FullActiveCheck()
    {
        for (int i = 0; i < objectNum; i++)
        {
            if (objectPool[i].activeSelf == false)
            {
                return false;
            }
        }
        return true;
    }
}
