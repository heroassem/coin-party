using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
using System.Linq.Expressions;

public class RoomManagemnt : MonoBehaviourPunCallbacks
{
    [SerializeField]
    TMP_InputField roomNameInput;

    public void CreateRoom()
    {
        if (string.IsNullOrEmpty(roomNameInput.text))
        {
            char[] chars = new char[8];
            for (int i = 0; i < chars.Length; i++)
            {
                int rand = Random.Range(64, 70);
                chars[i] = (char)(rand);
            }

            string s = roomNameInput.text;

            foreach(char c in chars)
                s += c;

            roomNameInput.text = s;
            
            return;
        }
        RoomOptions roomOptions = new RoomOptions() { IsOpen = true, MaxPlayers = 2 };

        PhotonNetwork.CreateRoom(roomNameInput.text, roomOptions);
    }

    public void JoinRoom()
    {
        if (string.IsNullOrEmpty(roomNameInput.text))
        {
            char[] chars = new char[8];
            for (int i = 0; i < chars.Length; i++)
            {
                int rand = Random.Range(64, 70);
                chars[i] = (char)(rand);
            }

            string s = roomNameInput.text;

            foreach (char c in chars)
                s += c;

            roomNameInput.text = s;

            return;
        }

        PhotonNetwork.JoinRoom(roomNameInput.text);
    }

    public override void OnCreatedRoom()
    {
        PhotonNetwork.LoadLevel("party");
        Debug.Log("Room created successfully: " + PhotonNetwork.CurrentRoom.Name);
        Debug.Log("Room Players : " + PhotonNetwork.CurrentRoom.PlayerCount);
    }
    public override void OnJoinedRoom()
    {
        PhotonNetwork.LoadLevel("party");
        Debug.Log("Room joinet successfully: " + PhotonNetwork.CurrentRoom.Name);
        Debug.Log("Room Players : " + PhotonNetwork.CurrentRoom.PlayerCount);
    }
}
