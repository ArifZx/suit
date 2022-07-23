using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System;
using TMPro;

public class GameScene : MonoBehaviourPunCallbacks
{

    [SerializeField]
    private Button[] _buttons;

    [SerializeField]
    private FightUI[] _playersUI;

    [SerializeField]
    private TMP_Text _textInfo;

    [SerializeField]
    private TMP_Text _enemyName;

    private bool _isFighting = false;

    private bool _isLeaving = false;


    public bool IsReadyToFight()
    {
        var list = PhotonNetwork.PlayerList;
        for (var i = 0; i < list.Length; i++)
        {
            var isReady = GetReady(list[i]);
            if (!isReady)
            {
                return false;
            }
        }

        return true;
    }

    public void InitializeGame()
    {
        for (var i = 0; i < _playersUI.Length; i++)
        {
            _playersUI[i].ChooseRPS("");
            _playersUI[i].Stars.SetStar(0);
        }

        _isFighting = false;
        _isLeaving = false;
        SetTextInfo("");
        SetEnemyName("");
    }

    public void SetTextInfo(string text)
    {
        if (_textInfo == null) return;
        _textInfo.text = text;
    }

    public void SetEnemyName(string text)
    {
        if (_enemyName == null) return;
        _enemyName.text = text;
    }

    public void InitChoose()
    {
        for (var i = 0; i < _playersUI.Length; i++)
        {
            _playersUI[i].ChooseRPS("");
        }
    }

    private void Awake()
    {
        if (!PhotonNetwork.IsConnected)
        {
            SceneManager.LoadScene(0);
            return;
        }

        SetReady(false);

        InitializeGame();

        SetWin(0);
        SyncLocal();
    }

    private void Start()
    {
        var enemy = GetEnemy();
        Debug.Log(enemy);
        if(enemy != null)
        {
            SetEnemyName(enemy.NickName);
        }
    }

    private void StartFight()
    {
        if (_isFighting) return;
        _isFighting = true;

        SetReady(false);

        StartCoroutine(Fighting());
    }

    private Photon.Realtime.Player GetEnemy()
    {
        Photon.Realtime.Player enemy = null;
        var players = PhotonNetwork.PlayerList;
        for (var i = 0; i < players.Length; i++)
        {
            if (players[i] != PhotonNetwork.LocalPlayer)
            {
                enemy = players[i];
            }
        }

        return enemy;
    }

    private IEnumerator Fighting()
    {
        yield return new WaitForEndOfFrame();

        var player = PhotonNetwork.LocalPlayer;
        var enemy = GetEnemy();

        var playerChoose = GetChoose(player);
        var enemyChoose = GetChoose(enemy);

        _playersUI[1].ChooseRPS(enemyChoose);

        yield return new WaitForSeconds(1);

        var winner = solve(playerChoose, enemyChoose);
        if (winner == 1)
        {
            // p1
            player.CustomProperties["win"] = GetWin(player) + 1;
            SetTextInfo("WIN");
        }
        else if (winner == -1)
        {
            // p2
            enemy.CustomProperties["win"] = GetWin(enemy) + 1;
            SetTextInfo("LOSE");
        } else
        {
            SetTextInfo("DRAW");
        }

        yield return new WaitForSeconds(1f);

        ChooseRPS("", false);
        InitChoose();
        SyncLocal();
        SetTextInfo("");

        yield return new WaitForSeconds(1f);
        _isFighting = false;
        EnableButtons(true);

    }

    private int solve(string p1, string p2)
    {
        if (p1 == p2)
        {
            return 0; // Tie
        }

        if (p1 == "rock")
        {
            if (p2 == "scissor")
                return 1;
            else
                return -1;
        }
        if (p1 == "paper")
        {
            if (p2 == "rock")
                return 1;
            else
                return -1;
        }
        if (p1 == "scissor")
        {
            if (p2 == "paper")
                return 1;
            else
                return -1;
        }

        return -1;
    }


    private string GetChoose(Player targetPlayer)
    {
        try
        {
            return (string)targetPlayer.CustomProperties.GetValueOrDefault("choose", "");
        }
        catch (Exception e)
        {
            Debug.LogError(e);
            return "";
        }
    }

    private int GetWin(Player targetPlayer)
    {
        try
        {
            return (int)targetPlayer.CustomProperties.GetValueOrDefault("win", 0);
        }
        catch (Exception e)
        {
            Debug.LogError(e);
            return 0;
        }
    }

    private bool GetReady(Player targetPlayer)
    {
        try
        {
            return (bool)targetPlayer.CustomProperties.GetValueOrDefault("isReady", false);
        }
        catch (Exception e)
        {
            Debug.LogError(e);
            return false;
        }
    }

    public void OnChooseRock()
    {
        ChooseRPS("rock");
        SetReady(true);
        SyncLocal();
    }

    public void OnChoosePaper()
    {
        ChooseRPS("paper");
        SetReady(true);
        SyncLocal();
    }

    public void OnChooseScissor()
    {
        ChooseRPS("scissor");
        SetReady(true);
        SyncLocal();
    }


    private void ChooseRPS(string name, bool ready = true)
    {
        PhotonNetwork.LocalPlayer.CustomProperties["choose"] = name;
        _playersUI[0].ChooseRPS(name);
        EnableButtons(false);
    }


    private void SetReady(bool value)
    {
        PhotonNetwork.LocalPlayer.CustomProperties["isReady"] = value;
    }

    private void SetWin(int number)
    {
        PhotonNetwork.LocalPlayer.CustomProperties["win"] = number;
    }

    private void SyncLocal()
    {
        PhotonNetwork.LocalPlayer.SetCustomProperties(PhotonNetwork.LocalPlayer.CustomProperties);
    }

    public void EnableButtons(bool active)
    {
        for (var i = 0; i < _buttons.Length; i++)
        {
            _buttons[i].interactable = active;
        }
    }

    public override void OnPlayerPropertiesUpdate(Photon.Realtime.Player targetPlayer, ExitGames.Client.Photon.Hashtable changedProps)
    {
        var isLocalPlayer = PhotonNetwork.LocalPlayer == targetPlayer;
        var index = isLocalPlayer ? 0 : 1;

        var win = (int)targetPlayer.CustomProperties.GetValueOrDefault("win", 0);
        var choose = (string)targetPlayer.CustomProperties.GetValueOrDefault("choose", "");
        var isReady = (bool)targetPlayer.CustomProperties.GetValueOrDefault("isReady", false);

        var playerUI = _playersUI[index];
        playerUI.Stars.SetStar(win);


        if (IsReadyToFight()  && win < 3)
        {
            StartFight();
        } else if (win >= 3)
        {
            SetTextInfo(index == 0 ? "You WIN" : "You LOSE");
            LeaveRoom();
        }
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        SetTextInfo("You WIN DC");
        LeaveRoom();
    }


    public void LeaveRoom()
    {
        if (_isLeaving) return;
        _isLeaving = true;

        StartCoroutine(LeavingRoom());
    }


    IEnumerator LeavingRoom()
    {
        yield return new WaitForSeconds(2f);
        if (PhotonNetwork.IsConnected) PhotonNetwork.LeaveRoom();
    }

    public override void OnLeftRoom()
    {
        SceneManager.LoadScene(0);
    }

}
