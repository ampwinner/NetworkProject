using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using UnityEngine.SceneManagement;

public class PlayerMoveCtrl : NetworkBehaviour
{
    public float speed = 0.1f;
    public GameObject PlayerModel;

    private void Start()
    {
        PlayerModel.SetActive(false);
    }

    public void Movement()
    {
        float xDirection = Input.GetAxis("Horizontal");
        float yDirection = Input.GetAxis("Vertical");

        Vector3 moveDirection = new Vector3(xDirection, 0, yDirection);
        transform.position += moveDirection * speed;
    }

    private void Update()
    {
        if (SceneManager.GetActiveScene().name == "Game")
        {
            if (PlayerModel.activeSelf == false)
            {
                SetPosition();
                PlayerModel.SetActive(true);
            }
        }

        if (hasAuthority)
        {
            Movement();
        }
    }

    public void SetPosition()
    {
        transform.position = new Vector3(Random.Range(-3, 3), 0, Random.Range(-8, 8));
    }
}
