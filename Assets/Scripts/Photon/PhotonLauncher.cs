using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

public class PhotonLauncher : MonoBehaviourPunCallbacks
{

    public delegate void JoinRoomHandler(bool isSuccess);

    public delegate void EstablisedConnectionHandler();

    public event JoinRoomHandler OnJoinEvent;

    public event EstablisedConnectionHandler OnEstablisedConnection;

    private bool _isConnecting = false;

    public bool IsConnecting { get { return _isConnecting; } }

    public bool IsConnected { get { return PhotonNetwork.IsConnected; } }

    [SerializeField]
    string gameVersion = "1";

    private void Awake()
    {
        _isConnecting = false;
        PhotonNetwork.AutomaticallySyncScene = true;
    }

    public void Connect()
    {
        if(PhotonNetwork.IsConnected)
        {
            return;
        }


        PhotonNetwork.ConnectUsingSettings();
        PhotonNetwork.GameVersion = gameVersion;
    }

    public override void OnConnectedToMaster()
    {
        Debug.Log("Photon is connected");
        _isConnecting = true;

        if(OnEstablisedConnection != null)
        {
            OnEstablisedConnection();
        }
    }

    public override void OnDisconnected(DisconnectCause cause)
    {
        _isConnecting = false;
        Debug.LogError("Photon disconnected: " + cause);
    }

    public void SetNickname(string nickname)
    {
        PhotonNetwork.NickName = nickname;
    }

    public string GetNickname()
    {
        return PhotonNetwork.NickName;
    }

    public void JoinRandomRoom()
    {
        if (!PhotonNetwork.IsConnected || !_isConnecting)
        {
            FailedJoin();
            return;
        };

        PhotonNetwork.JoinRandomRoom();
    }

    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        Debug.Log("Failed Join Random " + returnCode.ToString() + ": " + message);

        RoomOptions options = new RoomOptions();
        options.MaxPlayers = 2;
        options.BroadcastPropsChangeToAll = true;
        options.CleanupCacheOnLeave = true;
        options.CustomRoomProperties = new ExitGames.Client.Photon.Hashtable();
        options.CustomRoomProperties.Add("countdown", 5);
        PhotonNetwork.CreateRoom("room-" + Random.Range(0, 9999), options);
    }

    public override void OnJoinRoomFailed(short returnCode, string message)
    {
        Debug.Log("Failed Join " + returnCode.ToString() + ": " + message);
        FailedJoin();
    }

    public override void OnJoinedRoom()
    {
        Debug.Log("Joined Room");

        var property = new ExitGames.Client.Photon.Hashtable();
        property["index"] = PhotonNetwork.CurrentRoom.PlayerCount;
        property["isReady"] = false;
        property["choose"] = "";
        property["choose"] = "";
        PhotonNetwork.LocalPlayer.SetCustomProperties(property);
        SuccessJoin();
    }
    

    private void SuccessJoin()
    {
        if (OnJoinEvent == null) return;
        OnJoinEvent(true);
    }


    private void FailedJoin()
    {
        if (OnJoinEvent == null) return;
        OnJoinEvent(false);
    }

    public void LeaveRoom()
    {
        PhotonNetwork.LeaveRoom();
    }


}
