using UnityEngine;
using Mirror;
using Steamworks;

public class PlayerObjectController : NetworkBehaviour
{
    #region MEMBER_BASE
    [SyncVar]
    public int ConnectionID;
    [SyncVar]
    public int PlayerIdNumber;
    [SyncVar]
    public ulong PlayerSteamID;
    [SyncVar(hook = nameof(PlayerNameUpdate))]
    public string PlayerName;

    private PlayerMovementController playerMovementController;
    private GameNetworkManager manager;
    private GameNetworkManager Manager
    {
        get
        {
            if (manager != null)
            {
                return manager;
            }
            return manager = GameNetworkManager.singleton as GameNetworkManager;
        }
    }

    #endregion


    
    private void Start()
    {
        DontDestroyOnLoad(this.gameObject);
        playerMovementController = this.GetComponent<PlayerMovementController>();
        playerMovementController.settingUI = manager.interfaceManager.playerSetting_UI;
    }

    #region ON_CREATE

    public override void OnStartAuthority()
    {
        CmdSetPlayerName(SteamFriends.GetPersonaName().ToString());
        gameObject.name = "LocalGamePlayer";
    }

    public override void OnStartClient()
    {
        Manager.GamePlayers.Add(this);
    }

    public override void OnStopClient()
    {
        Manager.GamePlayers.Remove(this);
    }

    [Command]
    private void CmdSetPlayerName(string playerName)
    {
        this.PlayerNameUpdate(this.PlayerName, playerName);
    }

    public void PlayerNameUpdate(string OldValue, string NewValue)
    {
        if (isServer)
        {
            this.PlayerName = NewValue;
        }
        if (isClient)
        {

        }
    }



    #endregion

}