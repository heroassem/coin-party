using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Photon.Realtime;

public class GameManagemnt : MonoBehaviourPun
{
    public int playerWener;

    int s1;
    int s2;

    [SerializeField] TextMeshProUGUI playerScore1;
    [SerializeField] TextMeshProUGUI playerScore2;
    [SerializeField] TextMeshProUGUI textWine;

    [PunRPC]
    public void AddScore(int playerId)
    {
        if (playerId == 1)
        {
            s1++;
            playerScore1.text = s1.ToString();
        }
        else if (playerId == 2)
        {
            s2++;
            playerScore2.text = s2.ToString();
        }
    }

    [PunRPC]
    public void WineAndLose(float timeRemaining)
    {
        if (timeRemaining <= 0)
        {
            if (s1 > s2)
            {
                textWine.text = "Player 1 Wins!";
                playerWener = 1;
            }
            else if (s2 > s1)
            {
                textWine.text = "Player 2 Wins!";
                playerWener = 2;
            }
            else if (s2 == s1)
            {
                textWine.text = "Drow!";
            }
        }
    }
}