using TMPro;
using DG.Tweening;
using UnityEngine;
using Unity.Collections;
using UnityEngine.UI;

public class MenuScene : MonoBehaviour
{

    [SerializeField]
    private PhotonLauncher _launcher;

    [SerializeField]
    private GameObject _menuCanvas;
    public GameObject MenuCanvas { get { return _menuCanvas; } }

    [SerializeField]
    private GameObject _lobbyCanvas;
    public GameObject LobbyCanvas { get { return _lobbyCanvas; } }

    [SerializeField]
    private Button _playButton;

    [SerializeField]
    private TMP_InputField _inputField;

    void Start()
    {
        DOTween.Init();

        if (_launcher != null)
        {
            _launcher.Connect();

            if (_playButton && !_launcher.IsConnecting)
            {
                _playButton.interactable = false;
                _launcher.OnEstablisedConnection += OnEstablisedConnection;
            }

            var nickname = _launcher.GetNickname();
            if (_inputField != null && nickname != null && nickname.Length > 0)
            {
                _inputField.text = nickname;
            } else
            {
                _inputField.text = "Player-" + Random.Range(0, 99999);
            }
        }
        ShowMenu();

    }

    public void SetActiveMenu(string name)
    {
        SetActiveObject(_menuCanvas, name);
        SetActiveObject(_lobbyCanvas, name);
    }

    private void SetActiveObject(GameObject obj, string name)
    {
        if (obj == null) return;
        obj.SetActive(name.Equals(obj.name));
    }

    public void ShowMenu()
    {
        SetActiveMenu(_menuCanvas.name);
    }

    public void ShowLobby()
    {
        SetActiveMenu(_lobbyCanvas.name);
    }

    public void OnStartPlay()
    {
        if (_launcher == null || !_launcher.IsConnecting || !_launcher.IsConnected || _inputField == null || _inputField.text == "") return;

        _launcher.SetNickname(_inputField.text);
        _launcher.JoinRandomRoom();

        if (_playButton) _playButton.interactable = false;
        _launcher.OnJoinEvent += OnJoinEvent;

    }

    public void OnCancelPlay()
    {
        if (_launcher == null) return;

        _launcher.LeaveRoom();
        ShowMenu();
    }

    private void OnJoinEvent(bool isSuccess)
    {
        _launcher.OnJoinEvent -= OnJoinEvent;
        if (_playButton) _playButton.interactable = true;

        if (!isSuccess)
        {
            ShowMenu();
            return;
        }

        ShowLobby();
    }

    private void OnEstablisedConnection()
    {
        if (_playButton == null && _launcher == null) return;
        _launcher.OnEstablisedConnection -= OnEstablisedConnection;
        _playButton.interactable = true;
    }

}
