using Photon.Pun.Demo.PunBasics;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using Unity.Burst.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering.VirtualTexturing;
using Photon.Pun;
using Photon.Realtime;
using TMPro;
using System.Linq;
using System;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Photon.Pun.UtilityScripts;

public class playerControler : MonoBehaviour, IPunObservable
{
    Rigidbody2D rb;
    SpriteRenderer spriteR;
    Animator animator;
    Ray groundRay;
    GameManagemnt gameManagmnt;

    Vector3 neworkPosition;
    Quaternion neworkRotetion;
    Vector2 neworkVelocity;

    bool IsOnGround;
    bool IsOnWalRight;
    bool IsOnWalLeft;
    bool isJumping;
    bool IsWolking;
    bool IsFlooring;

    public bool IsStarted;

    [SerializeField] float moveSpeed = 5f;
    [SerializeField] float jumpForce = 5f;

    [SerializeField] TextMeshProUGUI timeText;

    [SerializeField] Transform gamePoint;
    [SerializeField] LayerMask Layer;
    [SerializeField] Transform RightRay;
    [SerializeField] Transform LeftRay;
    [SerializeField] PhotonView viwe;
    [SerializeField] SpriteRenderer Marck;
    [SerializeField] ParticleSystem paricle;

    float moveHorizontal;
    float lag;
    float speedLerp = 10f;
    float timeRemaining = 120f;

    int minutes, seconds;

    int playerID1;
    int playerID2;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        spriteR = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        if (PhotonNetwork.IsMasterClient && Input.GetKey(KeyCode.S))
        {
            viwe.RPC("startGame", RpcTarget.All);
        }

        if (viwe.IsMine)
        {
            if (!IsStarted)
            {
                animator.SetInteger("anime", 5);
                return;
            }

            if(PhotonNetwork.LocalPlayer.ActorNumber == 1)
            {
                playerID1 = viwe.ViewID;
            }
            else if (PhotonNetwork.LocalPlayer.ActorNumber == 2)
            {
                playerID2 = viwe.ViewID;
            }

            moveHorizontal = Input.GetAxis("Horizontal");
            Flip(); Jump();
            WallJump();
            GameTime();
            PlayerDropingToTheVoid(10f);

            gameManagmnt = FindAnyObjectByType<GameManagemnt>();

            if (moveHorizontal != 0f && ChekeGraound() == true && rb.velocity != Vector2.zero && isJumping == false)
                animator.SetInteger("anime", 1); IsWolking = true;

            if (rb.velocity.y == 0 && moveHorizontal == 0f && ChekeGraound() == true && rb.velocity == Vector2.zero)
                animator.SetInteger("anime", 0); isJumping = false; IsWolking = false;

            if (ChekeGraound() == true && IsWolking == true && animator.GetInteger("anime") == 3)
                animator.SetInteger("anime", 1); isJumping = false;

            Marck.enabled = true;

            Move(moveHorizontal, moveSpeed);
        }
        else
        {
            Marck.enabled = false;
            timeText.enabled = false;
            transform.position = Vector3.Lerp(transform.position, neworkPosition, Time.deltaTime * speedLerp);
            rb.velocity = Vector2.Lerp(rb.velocity, neworkVelocity, Time.deltaTime * speedLerp);
        }
    }

    public bool ChekeGraound()
    {
        RaycastHit2D hit = Physics2D.Raycast(gamePoint.position, Vector2.down, 0.3f);

        if (hit.collider != null)
        {
            IsOnGround = true;
        }
        else
        {
            IsOnGround = false;
        }

        return IsOnGround;
    }

    public void Move(float moveHorizontal, float speed)
    {
        Vector2 movement = new Vector2(moveHorizontal * speed, rb.velocity.y);
        rb.velocity = movement;
    }

    public void Flip()
    {
        Vector2 movement = rb.velocity;

        if (movement != Vector2.zero)
        {
            if (moveHorizontal > 0)
            {
                spriteR.flipX = false;
            }
            else if (moveHorizontal < 0)
            {
                spriteR.flipX = true;
            }
        }
    }

    [PunRPC]
    public void startGame()
    {
        IsStarted = true;
    }

    public void Jump()
    {

        if (Input.GetButtonDown("Jump") && ChekeGraound())
        {
            animator.SetInteger("anime", 2);
            rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
            isJumping = true;
        }

        if (rb.velocity.y <= rb.gravityScale && ChekeGraound() == false && !IsWaleOnWall())
        {
            animator.SetInteger("anime", 3);
            IsFlooring = true;
        }

        if (ChekeGraound() && IsFlooring == true)
        {
            viwe.RPC("PlayParticle", RpcTarget.All);
            IsFlooring = false;
        }
    }

    public bool IsWaleOnRight()
    {
        RaycastHit2D RightHite = Physics2D.Raycast(RightRay.position, Vector2.right, 0.3f);

        if (RightHite.collider != null)
        {
            IsOnWalRight = true;
        }
        else
        {
            IsOnWalRight = false;
        }

        return IsOnWalRight;
    }
    public bool IsWaleOnLeft()
    {
        RaycastHit2D LeftHite = Physics2D.Raycast(LeftRay.position, Vector2.left, 0.3f);

        if (LeftHite.collider != null)
        {
            IsOnWalLeft = true;
        }
        else
        {
            IsOnWalLeft = false;
        }

        return IsOnWalLeft;
    }

    bool IsWaleOnWall()
    {
        return IsWaleOnLeft() || IsWaleOnRight();
    }

    public void WallJump()
    {
        if (rb.velocity.y <= rb.gravityScale && ChekeGraound() == false && IsWaleOnRight())
        {
            animator.SetInteger("anime", 4);
            spriteR.flipX = false;
            rb.gravityScale = rb.gravityScale / 2;

            if (Input.GetButtonDown("Jump"))
            {
                animator.SetInteger("anime", 2);
                Vector2 froce = new Vector2((-jumpForce), 1).normalized;
                rb.AddForce(froce * 7, ForceMode2D.Impulse);
                isJumping = true;
            }
        }
        else if (rb.velocity.y <= rb.gravityScale && ChekeGraound() == false && IsWaleOnLeft())
        {
            animator.SetInteger("anime", 4);
            spriteR.flipX = true;
            rb.gravityScale = rb.gravityScale / 2;

            if (Input.GetButtonDown("Jump"))
            {
                animator.SetInteger("anime", 2);
                Vector2 froce = new Vector2((jumpForce), 1).normalized;
                rb.AddForce(froce * 7, ForceMode2D.Impulse);
                isJumping = true;
            }
        }

        else
        {
            rb.gravityScale = 1;
        }
    }

    public void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("coin"))
        {
            if (viwe.IsMine)
            {
                PhotonView targetView = collision.GetComponent<PhotonView>();
                int id = targetView.ViewID;
                viwe.RPC("DestroyObject", RpcTarget.All, id);

                GameObject gameManagemnt = GameObject.Find("GamePartyManagemnt");
                PhotonView gameManagemntView = gameManagemnt.GetComponent<PhotonView>();

                gameManagemntView.RPC("AddScore", RpcTarget.All, PhotonNetwork.LocalPlayer.ActorNumber);
            }
        }
    }

    [PunRPC]
    public void DestroyObject(int Object)
    {
        PhotonNetwork.Destroy(PhotonView.Find(Object));
    }

    [PunRPC]
    public void PlayParticle()
    {
        paricle.Play();
    }

    public void GameTime()
    {
        GameObject gameManagemnt = GameObject.Find("GamePartyManagemnt");
        PhotonView gameManagemntView = gameManagemnt.GetComponent<PhotonView>();

        if (timeRemaining > 0)
        {
            timeRemaining -= Time.deltaTime;

            minutes = Mathf.FloorToInt(timeRemaining / 60);
            seconds = Mathf.FloorToInt(timeRemaining % 60);

            timeText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
        }

        else
        {
            timeText.text = "00:00";
            gameManagemntView.RPC("WineAndLose", RpcTarget.All, timeRemaining);
            GameManagemnt gameManagmnt = FindAnyObjectByType<GameManagemnt>();

            if(gameManagmnt.playerWener == 1)
            {
                PhotonNetwork.Destroy(PhotonView.Find(playerID2));
            }
            else if (gameManagmnt.playerWener == 2)
            {
                PhotonNetwork.Destroy(PhotonView.Find(playerID1));
            }
        }
    }


    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(transform.position);
            stream.SendNext(rb.velocity);
            stream.SendNext(spriteR.flipX);
            stream.SendNext(rb.gravityScale);
            stream.SendNext(timeText.text);
            stream.SendNext(IsStarted);
        }
        else
        {
            neworkPosition = (Vector3)stream.ReceiveNext();
            neworkVelocity = (Vector2)stream.ReceiveNext();
            spriteR.flipX = (bool)stream.ReceiveNext();
            rb.gravityScale = (float)stream.ReceiveNext();
            timeText.text = (string)stream.ReceiveNext();
            IsStarted = (bool)stream.ReceiveNext();

            lag = Mathf.Abs((float)(PhotonNetwork.Time - info.SentServerTime));
            neworkPosition += (Vector3)neworkVelocity * lag;
        }
    }

    private void PlayerDropingToTheVoid(float DropValue)
    {
        if (transform.position.y < -DropValue)
        {
            if (viwe.IsMine)
            {
                transform.position = new Vector3(9.31f, 2.18f, 4.119727f);
            }
        }
    }
}