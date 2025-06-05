using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;
using Mirror;
using UnityEngine.AI;
using Unity.AI.Navigation;
using Unity.VisualScripting;

public class RoomPrefabData : MonoBehaviour
{
    //x=10 y=7.5 z = 10

    public List<RoomPrefabConnect> connectRoom;
    public Transform[] roomArray;
    public bool isEdge = false;

    private void Start()
    {
        if(this.TryGetComponent<NavMeshSurface>(out NavMeshSurface navMesh))
        {
            navMesh.BuildNavMesh();
        }
    }

    public void Active_Door()
    {
        var activeDoorList = connectRoom.Where(door => !door.isBuild).ToList();
        
        for(int i = 0; i< activeDoorList.Count; i++)
        {
            if (activeDoorList[i].doorObject.Count > 0)
            {
                GameObject active = RandomActive(activeDoorList[i].doorObject);
                active.SetActive(true);
            }
        }
    }
    GameObject RandomActive(List<GameObject> doorList)
    {
        int index = UnityEngine.Random.Range(0, doorList.Count);
        return doorList[index];
    }
}



[System.Serializable]
public class RoomPrefabConnect
{
    public List<GameObject> doorObject;
    public Transform point;
    public bool isBuild;
    public bool pX;
    public bool pZ;
    public bool mX;
    public bool mZ;
}
