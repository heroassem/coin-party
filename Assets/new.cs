using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class Spowne: MonoBehaviourPunCallbacks
{
    private void Start()
    {
        PhotonNetwork.Instantiate("Square", new Vector3(0f, 0f, Random.Range(-3f, 3f)), Quaternion.identity);
    }
}
