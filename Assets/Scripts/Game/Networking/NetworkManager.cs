using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class NetworkManager : MonoBehaviourPunCallbacks
{
    public int numPlayers;

    public static NetworkManager instance;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        PhotonNetwork.ConnectUsingSettings();
    }

    public override void OnConnectedToMaster()
    {
        PhotonNetwork.JoinLobby();
        Debug.Log("You joined master server!");
    }

//#if UNITY_EDITOR_WIN
//    public override void OnJoinedLobby()
//    {
//        CreateRooms("Test Room");
//    }

//    public override void OnJoinedRoom()
//    {
//        Debug.Log("You joined to room " + PhotonNetwork.CurrentRoom.Name);
//    }
//#endif

    public void CreateRooms(string roomName)
    {
        RoomOptions options = new RoomOptions();
        options.MaxPlayers = (byte)numPlayers;

        PhotonNetwork.CreateRoom(roomName, options);
        Debug.Log("Success creating a room!");
    }

    public void JoinRoom(string roomName)
    {
        PhotonNetwork.JoinRoom(roomName);
    }

    [PunRPC]
    public void ChangeScene(string sceneName)
    {
        PhotonNetwork.LoadLevel(sceneName);
    }
}
