using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class spown : MonoBehaviourPunCallbacks
{
    private void Start()
    {
        PhotonNetwork.SendRate = 20;
        PhotonNetwork.SerializationRate = 10;
    }

    private void Awake()
    {
        if (!PhotonNetwork.IsConnected && PhotonNetwork.IsMasterClient)
        {
            Debug.LogError("PhotonNetwork is not connected. Cannot instantiate objects.");
            return;
        }

        if(PhotonNetwork.LocalPlayer.ActorNumber == 1 && PhotonNetwork.IsConnected && PhotonNetwork.InRoom)
            PhotonNetwork.Instantiate("player", new Vector3(transform.position.x - 3f, transform.position.y, 0f), Quaternion.identity);        
        else if(PhotonNetwork.LocalPlayer.ActorNumber == 2 && PhotonNetwork.IsConnected && PhotonNetwork.InRoom)
            PhotonNetwork.Instantiate("player", new Vector3(transform.position.x + 3f, transform.position.y, 0f), Quaternion.identity);
    }
}
