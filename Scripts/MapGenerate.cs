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
        Debug.Log("�ʻ��� ����");

        //roomParent.GetComponent<NavMeshSurface>().BuildNavMesh();
    }
    public void Room_Instantiate(RoomPrefabData roomData, Transform parent)
    {
        ///
        /// �׸��� ��ǥ�� ���� ��ȭ �ʿ�
        /// ���� 10x8x10�� �������� �׸���ȭ ������.
        /// �� room������ ���� �����ϴ� �׸��� ��ǥ�� �̸� �ľ��Ͽ� �� ��ġ���� �����ϱ�.
        ///

        var connectRoomList = roomData.connectRoom;//roomPrefabList.Select((room) => room.connectRoom. == true);
        
        List<RoomPrefabData> addRoomList = new List<RoomPrefabData>();

        for (int i = 0; i< connectRoomList.Count; i++)
        {
            var gridBuildPos = connectRoomList[i].point.position;
            Debug.Log("Grid��ǥ �������� : "+ gridBuildPos + gridVectorList.Contains(gridBuildPos));
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
                    else //�׸��带 ��� �����̱� ������ "Edge"Ÿ���� ���๰ ��ġ
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
                    else //�׸��带 ��� �����̱� ������ "Edge"Ÿ���� ���๰ ��ġ
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
                    else //�׸��带 ��� �����̱� ������ "Edge"Ÿ���� ���๰ ��ġ
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
                        //newRoomData.connectRoom.Where(x => x.mX).ToList().ForEach(x=>x.isBuild = true); //���� ��ġ�� �ٸ��� ���� mX���ִٸ� ���������������¿����� �����߻� �������.
                        Room_Instantiate(newRoomData, roomParent);

                        NetworkServer.Spawn(room);
                    }
                    else //�׸��带 ��� �����̱� ������ "Edge"Ÿ���� ���๰ ��ġ
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
                Debug.Log("��ġ�� ���� �̹� �������� ��ġ�Ǿ�����.");
            }
        }

        /*if (connectCount <= 1)
        {
            //edge Room ����
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
                // ��ġ�� ��ǥ�� �߰ߵǸ� false ��ȯ
                Debug.Log("��ġ�� ��ǥ�� �߰ߵǸ� false ��ȯ : "+ usedCoord.position + addGird);
                return false;
            }
        }
        // �ݺ��� ���� ������ �ߺ��� ���ٸ� true ��ȯ
        Debug.Log("�ݺ��� ���� ������ �ߺ��� ���ٸ� true ��ȯ");
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
