using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using Steamworks;
using UnityEngine.SceneManagement;

public class GameNetworkManager : NetworkManager
{
    [SerializeField]
    private PlayerObjectController GamePlayerPrefab;

    public MapGenerate mapGenerate;
    public InterfaceManager interfaceManager;

    public List<PlayerObjectController> GamePlayers { get; } = new List<PlayerObjectController>();

    //������ ��� �߰��� ��
    public override void OnServerAddPlayer(NetworkConnectionToClient conn)
    {
        //base.OnServerAddPlayer(conn);
        if (SceneManager.GetActiveScene().name == "Menu_Online")
        {
            Debug.Log("�÷��̾����");
            PlayerObjectController GamePlayerInstance = Instantiate(GamePlayerPrefab);
            GamePlayerInstance.ConnectionID = conn.connectionId;
            GamePlayerInstance.PlayerIdNumber = GamePlayers.Count + 1;
            GamePlayerInstance.PlayerSteamID = (ulong)SteamMatchmaking.GetLobbyMemberByIndex((CSteamID)SteamLobby.Instance.CurrentLobbyID, GamePlayers.Count);

            NetworkServer.AddPlayerForConnection(conn, GamePlayerInstance.gameObject);
        }
    }

    //�κ�� ������ ���� ���� ���� ����� ���
    public void StartGame()
    {
        //ServerChangeScene(Constant.SCENE_GAME);
    }

    //Ŭ���̾�Ʈ�� ���� ������ ��
    public override void OnClientDisconnect()
    {
        base.OnClientDisconnect();
        interfaceManager.Button_ActiveUI(true);
    }

    //������ ���� ������ ��
    public override void OnServerDisconnect(NetworkConnectionToClient conn)
    {
        base.OnServerDisconnect(conn);
        interfaceManager.Button_ActiveUI(true);
    }
}