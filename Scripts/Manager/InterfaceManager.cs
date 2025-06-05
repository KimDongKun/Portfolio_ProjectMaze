using UnityEngine;
using UnityEngine.UI;
using Steamworks;
public class InterfaceManager : MonoBehaviour
{
    public Transform roomListContent;
    public GameObject buttonPrefab_Room;
    public GameObject mainLobby_UI;
    public GameObject playerSetting_UI;

    public HostData input_HostData;
    public void Button_ActiveUI(bool isAct)
    {
        mainLobby_UI.SetActive(isAct);
    }
    public void Remove_RoomList()
    {
        for(int i = 0; i<roomListContent.childCount; i++)
        {
            Destroy(roomListContent.GetChild(i).gameObject);
        }
    }
    public void Button_LoadRoomList(LobbyMatchList_t callback)
    {
        Remove_RoomList();
        for (int i = 0; i < callback.m_nLobbiesMatching; i++)
        {
            CSteamID lobbyId = SteamMatchmaking.GetLobbyByIndex(i);
            string roomName = SteamMatchmaking.GetLobbyData(lobbyId, "name");
            int playerValue = SteamMatchmaking.GetNumLobbyMembers(lobbyId);
            Debug.Log($"로비 ID: {lobbyId}, 방 이름: {roomName}");

            GameObject roomPanel = Instantiate(buttonPrefab_Room, roomListContent);
            var roomPrefab = roomPanel.GetComponent<RoomLobbyPrefab>();

            roomPrefab.roomName.text = roomName;
            roomPrefab.roomPlayerValue.text = playerValue + "/4";
            roomPrefab.joinButton.onClick.RemoveAllListeners();
            roomPrefab.joinButton.onClick.AddListener( delegate { SteamMatchmaking.JoinLobby(lobbyId); });
        }
    }
    
    public void Load_LobbyData()
    {
        SteamMatchmaking.AddRequestLobbyListDistanceFilter(ELobbyDistanceFilter.k_ELobbyDistanceFilterClose);
        SteamMatchmaking.RequestLobbyList();
    }
}
[System.Serializable]
public class HostData
{
    public InputField hostName_InputField;
    public Toggle isOnlyFriends_Toggle;
}
