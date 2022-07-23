using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LobbyCanvas : MonoBehaviourPunCallbacks
{
    [SerializeField]
    private MenuScene _scene;

    [SerializeField]
    private WaitingGroup _waitingGroup;

    [SerializeField]
    private CountdownGroup _countdown;

    [SerializeField]
    private Button _cancelButton;

    bool _isLoadingGameScene = false;

    private float _counter = 5;

    // Start is called before the first frame update
    void Start()
    {

        Debug.Log(PhotonNetwork.IsConnected, _countdown);
        if (!PhotonNetwork.IsConnected && _scene != null)
        {
            _scene.SetActiveMenu(_scene.MenuCanvas.name);
            return;
        }

        _isLoadingGameScene = false;

        _counter = (float)PhotonNetwork.CurrentRoom.CustomProperties.GetValueOrDefault("coundown", _counter);
        _countdown.SetText(Mathf.CeilToInt(_counter).ToString());

    }

    // Update is called once per frame
    void Update()
    {
        if (_waitingGroup == null || !PhotonNetwork.IsConnected || PhotonNetwork.CurrentRoom == null) return;

        var room = PhotonNetwork.CurrentRoom;
        int count = PhotonNetwork.PlayerList.Length;
        var roomCounter = (int)room.CustomProperties.GetValueOrDefault("countdown", (int)_counter);

        if (count == 2 && _waitingGroup.gameObject.activeSelf)
        {
            _waitingGroup.gameObject.SetActive(false);
            _countdown.gameObject.SetActive(true);
            if (_cancelButton) _cancelButton.interactable = false;
        }
        else if (count == 1 && !_waitingGroup.gameObject.activeSelf)
        {
            if (PhotonNetwork.IsMasterClient)
            {
                room.CustomProperties["countdown"] = 5;
            }

            if (_cancelButton) _cancelButton.interactable = true;

            _counter = 5f;

            _waitingGroup.gameObject.SetActive(true);
            _countdown.gameObject.SetActive(false);
        }
        else if (count == 0 && _scene != null)
        {
            _scene.ShowMenu();
        }

        if(count==2)
        {
            if (PhotonNetwork.IsMasterClient)
            {
                _counter -= Time.deltaTime;
                var counter = Mathf.CeilToInt(_counter);
                if (counter != roomCounter && counter >= 0)
                {
                    _countdown.SetText(counter.ToString());
                    room.CustomProperties["countdown"] = counter;
                    room.SetCustomProperties(room.CustomProperties);
                }
            }
            
            if(_counter <= 0)
            {
                LoadGameScene();
            }
        }
    }

    public override void OnLeftRoom()
    {
        _scene.ShowMenu();
    }

    public override void OnRoomPropertiesUpdate(ExitGames.Client.Photon.Hashtable propertiesThatChanged)
    {
        var isMaster = PhotonNetwork.IsMasterClient;

        var counter = (int)propertiesThatChanged.GetValueOrDefault("countdown", 5);
        if (_countdown) _countdown.SetText(counter.ToString());
        if (!isMaster) _counter = counter;

    }

    private void LoadGameScene()
    {
        if (_isLoadingGameScene) return;

        _isLoadingGameScene = true;
        SceneManager.LoadScene(1);
    }
}
