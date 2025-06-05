using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;
using Random = UnityEngine.Random;
using UnityEngine.UIElements;
using Unity.VisualScripting;
using Mirror;
using System.Net.NetworkInformation;
using Steamworks;
using UnityEngine.AI;
using Unity.AI.Navigation;
public class MapGenerate : NetworkBehaviour
{
    public GameObject roomNavSurface;
    public Transform roomParent;
    public GridVector gridVector;
    public List<RoomPrefabData> roomPrefabList;
    public Transform mapTransform;
    public int buildRoomValue = 10;


    public List<Vector3> gridVectorList;
    public List<Vector3> gridBuildList;

    [Server]
    public void MapGenerate_Start()
    {
        if (mapTransform == null)
        {
            mapTransform = this.transform;
            gridVectorList = Get_GridVectorList(gridVector);
        }

        GameObject roomParentObj = Instantiate(roomNavSurface, Vector3.zero, Quaternion.identity);
        roomParent = roomParentObj.transform;

        int randomMapCode = Random.Range(0, roomPrefabList.Count);

        //roomPrefabList[0] => Start Room
        GameObject room = Instantiate(roomPrefabList[0].gameObject, mapTransform.position, Quaternion.identity, roomParent);
        
        gridBuildList.Add(mapTransform.position);
        //gridVectorList.Remove(mapTransform.position);
        var roomData = room.GetComponent<RoomPrefabData>();
        //roomData.Active_Door();
        NetworkServer.Spawn(room);

        Room_Instantiate(roomData, roomParent);
        Debug.Log("맵생성 종료");

        //roomParent.GetComponent<NavMeshSurface>().BuildNavMesh();
    }
    public void Room_Instantiate(RoomPrefabData roomData, Transform parent)
    {
        ///
        /// 그리드 좌표에 대한 고도화 필요
        /// 현재 10x8x10을 기준으로 그리드화 진행중.
        /// 각 room프리팹 마다 차지하는 그리드 좌표를 미리 파악하여 맵 배치전에 검토하기.
        ///

        var connectRoomList = roomData.connectRoom;//roomPrefabList.Select((room) => room.connectRoom. == true);
        
        List<RoomPrefabData> addRoomList = new List<RoomPrefabData>();

        for (int i = 0; i< connectRoomList.Count; i++)
        {
            var gridBuildPos = connectRoomList[i].point.position;
            Debug.Log("Grid좌표 존재유무 : "+ gridBuildPos + gridVectorList.Contains(gridBuildPos));
            if (!connectRoomList[i].isBuild && !gridBuildList.Contains(gridBuildPos)/* && nowBuildValue < buildRoomValue*/)
            {
                if (connectRoomList[i].pX)
                {
                   if (gridVectorList.Contains(gridBuildPos))
                    {
                        addRoomList = roomPrefabList.Where(room => !room.isEdge && Check_GridVectorList(room.roomArray.ToList(),gridBuildPos) && room.connectRoom.Any(connect => connect.mX)).ToList();
                        connectRoomList[i].isBuild = true;
                        int randomIndex = Random.Range(0, addRoomList.Count);
                        var room = Instantiate(addRoomList[randomIndex].gameObject, gridBuildPos, Quaternion.identity, parent);
                        var newRoomData = room.GetComponent<RoomPrefabData>();
                        newRoomData.Active_Door();
                        Add_GridVectorList(newRoomData.roomArray.ToList());
                        Room_Instantiate(newRoomData, roomParent);

                        NetworkServer.Spawn(room);
                    }
                    else //그리드를 벗어난 상태이기 때문에 "Edge"타입의 건축물 배치
                    {
                        gridBuildList.Add(gridBuildPos);

                        addRoomList = roomPrefabList.Where(room => room.isEdge && room.connectRoom.Any(connect => connect.mX)).ToList();
                        connectRoomList[i].isBuild = true;
                        int randomIndex = Random.Range(0, addRoomList.Count);
                        var room = Instantiate(addRoomList[randomIndex].gameObject, gridBuildPos, Quaternion.identity, parent);

                        NetworkServer.Spawn(room);
                    }

                    
                }
                else if (connectRoomList[i].mX)
                {
                    if (gridVectorList.Contains(gridBuildPos))
                    {
                        addRoomList = roomPrefabList.Where(room => !room.isEdge && Check_GridVectorList(room.roomArray.ToList(), gridBuildPos) && room.connectRoom.Any(connect => connect.pX)).ToList();

                        connectRoomList[i].isBuild = true;
                        int randomIndex = Random.Range(0, addRoomList.Count);
                        var room = Instantiate(addRoomList[randomIndex].gameObject, gridBuildPos, Quaternion.identity, parent);
                        var newRoomData = room.GetComponent<RoomPrefabData>();
                        newRoomData.Active_Door();
                        Add_GridVectorList(newRoomData.roomArray.ToList());
                        Room_Instantiate(newRoomData, roomParent);

                        NetworkServer.Spawn(room);
                    }
                    else //그리드를 벗어난 상태이기 때문에 "Edge"타입의 건축물 배치
                    {
                        gridBuildList.Add(gridBuildPos);

                        addRoomList = roomPrefabList.Where(room => room.isEdge && room.connectRoom.Any(connect => connect.pX)).ToList();
                        connectRoomList[i].isBuild = true;
                        int randomIndex = Random.Range(0, addRoomList.Count);
                        var room = Instantiate(addRoomList[randomIndex].gameObject, gridBuildPos, Quaternion.identity, parent);

                        NetworkServer.Spawn(room);
                    }
                }
                else if (connectRoomList[i].pZ)
                {
                    if (gridVectorList.Contains(gridBuildPos))
                    {
                        addRoomList = roomPrefabList.Where(room => !room.isEdge && Check_GridVectorList(room.roomArray.ToList(), gridBuildPos) && room.connectRoom.Any(connect => connect.mZ)).ToList();

                        connectRoomList[i].isBuild = true;
                        int randomIndex = Random.Range(0, addRoomList.Count);
                        var room = Instantiate(addRoomList[randomIndex].gameObject, gridBuildPos, Quaternion.identity, parent);
                        var newRoomData = room.GetComponent<RoomPrefabData>();
                        newRoomData.Active_Door();
                        Add_GridVectorList(newRoomData.roomArray.ToList());
                        Room_Instantiate(newRoomData, roomParent);

                        NetworkServer.Spawn(room);
                    }
                    else //그리드를 벗어난 상태이기 때문에 "Edge"타입의 건축물 배치
                    {
                        gridBuildList.Add(gridBuildPos);

                        addRoomList = roomPrefabList.Where(room => room.isEdge && room.connectRoom.Any(connect => connect.mZ)).ToList();
                        connectRoomList[i].isBuild = true;
                        int randomIndex = Random.Range(0, addRoomList.Count);
                        var room = Instantiate(addRoomList[randomIndex].gameObject, gridBuildPos, Quaternion.identity, parent);

                        NetworkServer.Spawn(room);
                    }
                }
                else if (connectRoomList[i].mZ)
                {
                    if (gridVectorList.Contains(gridBuildPos))
                    {
                        addRoomList = roomPrefabList.Where(room => !room.isEdge && Check_GridVectorList(room.roomArray.ToList(), gridBuildPos) && room.connectRoom.Any(connect => connect.pZ)).ToList();

                        connectRoomList[i].isBuild = true;
                        int randomIndex = Random.Range(0, addRoomList.Count);
                        var room = Instantiate(addRoomList[randomIndex].gameObject, gridBuildPos, Quaternion.identity, parent);
                        var newRoomData = room.GetComponent<RoomPrefabData>();
                        newRoomData.Active_Door();
                        Add_GridVectorList(newRoomData.roomArray.ToList());
                        //newRoomData.connectRoom.Where(x => x.mX).ToList().ForEach(x=>x.isBuild = true); //만약 위치만 다르고 같은 mX가있다면 생성되지않은상태에서도 문제발생 여지고려.
                        Room_Instantiate(newRoomData, roomParent);

                        NetworkServer.Spawn(room);
                    }
                    else //그리드를 벗어난 상태이기 때문에 "Edge"타입의 건축물 배치
                    {
                        gridBuildList.Add(gridBuildPos);

                        addRoomList = roomPrefabList.Where(room => room.isEdge && room.connectRoom.Any(connect => connect.pZ)).ToList();
                        connectRoomList[i].isBuild = true;
                        int randomIndex = Random.Range(0, addRoomList.Count);
                        var room = Instantiate(addRoomList[randomIndex].gameObject, gridBuildPos, Quaternion.identity, parent);

                        NetworkServer.Spawn(room);
                    }
                }
            }
            else
            {
                Debug.Log("설치할 곳에 이미 프리팹이 설치되어있음.");
            }
        }

        /*if (connectCount <= 1)
        {
            //edge Room 생성
        }*/
       
    }
    void Add_GridVectorList(List<Transform> usedGrid)
    {
        foreach (Transform usedCoord in usedGrid)
        {
            gridBuildList.Add(usedCoord.position);
        }
    }
    bool Check_GridVectorList(List<Transform> usedGrid, Vector3 addGird)
    {
        foreach (Transform usedCoord in usedGrid)
        {
            if (gridBuildList.Contains(usedCoord.position + addGird))
            {
                // 겹치는 좌표가 발견되면 false 반환
                Debug.Log("겹치는 좌표가 발견되면 false 반환 : "+ usedCoord.position + addGird);
                return false;
            }
        }
        // 반복이 끝날 때까지 중복이 없다면 true 반환
        Debug.Log("반복이 끝날 때까지 중복이 없다면 true 반환");
        return true;
    }
    private List<Vector3> Get_GridVectorList(GridVector gridData)
    {
        var startPos = mapTransform.position;
        var gridCoordinates = new List<Vector3>();
        for (int x = -(int)gridData.limit.x+1; x < gridData.limit.x; x++)
        {
            for (int y = 0; y < gridData.limit.y; y++)
            {
                for (int z = 0; z < gridData.limit.z; z++)
                {
                    Vector3 position = new Vector3(
                        startPos.x + (x * gridData.width),
                        startPos.y + (y * gridData.height),
                        startPos.z - (z * gridData.depth)
                    );
                    gridCoordinates.Add(position);
                }
            }
        }

        return gridCoordinates;
    }
    

}
[System.Serializable]
public class GridVector
{
    public Vector3 limit;
    public float width = 10;
    public float height = 8.5f;
    public float depth = 10;
}
