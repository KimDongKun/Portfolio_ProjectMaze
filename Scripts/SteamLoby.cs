using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using Steamworks;
using UnityEngine.UI;
using System;

public class SteamLobby : MonoBehaviour
{
    public static SteamLobby Instance;

    protected Callback<LobbyCreated_t> LobbyCreated;
    protected Callback<GameLobbyJoinRequested_t> JoinRequest;
    protected Callback<LobbyEnter_t> LobbyEntered;
    protected Callback<LobbyMatchList_t> LobbyMatchList;

    public ulong CurrentLobbyID;
    private const string HostAddressKey = "CustomHostAddress";
    private GameNetworkManager manager;
    public InterfaceManager interfaceManager;

    private void Start()
    {
        if (!SteamManager.Initialized)
            return;

        if (Instance == null)
            Instance = this;

        manager = GetComponent<GameNetworkManager>();
        Cursor.lockState = CursorLockMode.None;


        LobbyCreated = Callback<LobbyCreated_t>.Create(OnLobbyCreated);
        JoinRequest = Callback<GameLobbyJoinRequested_t>.Create(OnJoinRequest);
        LobbyEntered = Callback<LobbyEnter_t>.Create(OnLobbyEntered);
        LobbyMatchList = Callback<LobbyMatchList_t>.Create(OnLobbyMatchList);
    }

    //버튼 등으로 로비 호스트할 때 실행
    public void HostLobby()
    {
        //manager.mapGenerate.MapGenerate_Start();
        var onlyFriends = interfaceManager.input_HostData.isOnlyFriends_Toggle.isOn;

        ELobbyType lobbyType = onlyFriends ? ELobbyType.k_ELobbyTypeFriendsOnly : ELobbyType.k_ELobbyTypePublic;
        SteamMatchmaking.CreateLobby(lobbyType, manager.maxConnections);
    }

    //로비가 생성되었을 때 콜백
    private void OnLobbyCreated(LobbyCreated_t callback)
    {
        if (callback.m_eResult != EResult.k_EResultOK)
            return;

        Debug.Log("로비 생성 성공");

        manager.StartHost();
        interfaceManager.Button_ActiveUI(false);

        string lobbyName = interfaceManager.input_HostData.hostName_InputField.text;

        SteamMatchmaking.SetLobbyData(new CSteamID(callback.m_ulSteamIDLobby), HostAddressKey, SteamUser.GetSteamID().ToString());
        SteamMatchmaking.SetLobbyData(new CSteamID(callback.m_ulSteamIDLobby), "name", lobbyName);

    }

    //로비 참여 시 콜백
    private void OnJoinRequest(GameLobbyJoinRequested_t callback)
    {
        Debug.Log("로비 참여 요청");
        SteamMatchmaking.JoinLobby(callback.m_steamIDLobby);
    }

    //로비 입장 시 
    private void OnLobbyEntered(LobbyEnter_t callback)
    {

        CurrentLobbyID = callback.m_ulSteamIDLobby;

        if (NetworkServer.active)
            return;

        manager.networkAddress = SteamMatchmaking.GetLobbyData(new CSteamID(callback.m_ulSteamIDLobby), HostAddressKey);

        manager.StartClient();
        interfaceManager.Button_ActiveUI(false);
    }
    //서버에서 로비(게임방) 리스트 검색.
    public void OnLobbyMatchList(LobbyMatchList_t callback)
    {
        Debug.Log($"로비 검색 결과: {callback.m_nLobbiesMatching}개 로비 발견");
        interfaceManager.Button_LoadRoomList(callback);
        
    }

    
}