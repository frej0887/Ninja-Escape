﻿using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PlayerMovement : MonoBehaviour{
    //Move
    public float moveSpeed;
    public float standardMoveSpeed = 5;
    public float rotationSpeed = 100;
    public int runSpeed = 8;

    //Crouch
    public int crouchSpeed = 2;
    public float normalCrouchHeight = 0.6757823f;
    public float crouchHeight = 0.4f;

    //Sticky and Slippery Floor
    public float stickyFloorSpeed;
    public bool onStickyFloor = false;
    public float slipperyFloorSpeed;
    public bool onSlippery = false;

    //Jump
    public float jumpHeight = 5;
    public float jumpTime = .9f;
    public float lastJump = -.9f;

    //Stun
    public float addStunTime = 3f;
    public bool stun = false;
    public float time2;
    public float stunTime = 1;
    public TimerScript timer;
    public GameObject plusTimer;

    public PlayerSound PS;
    public Rigidbody myRigidBody;
    public Vector3 PlayerPos;
    public Vector3 PlayerSize;

    //Drunk controls
    public bool drunk = false;
    public float drunkTimer;
    public float timeDrunk;

    //Animation
    public Animator ninjaController;
    public float aniCrossFade;

    // Use this for initialization
    void Start ()    {
        myRigidBody = GetComponent<Rigidbody>();
        PS = GetComponent<PlayerSound>();
    }
	
	// Update is called once per frame
	void Update () {

        drunkTimer -= Time.deltaTime;
        if (Time.time - time2 >= stunTime)    {
            stun = false;
            plusTimer.SetActive(false);
        }
    }

    void FixedUpdate() {
        if (onSlippery == false)    {
            Move();
        }
        Crouch();
        if (onStickyFloor == false)    {
            Jump();
        }
        Restart();
    }

    public void Move()    {
        if (stun == false && drunkTimer < 0)     {
            gameObject.layer = 8;
            transform.Translate(-moveSpeed * Time.deltaTime * Input.GetAxis("Vertical"), 0, moveSpeed * Time.deltaTime * Input.GetAxis("Horizontal"), Space.World);

            if (Input.GetAxis("Vertical") != 0 || Input.GetAxis("Horizontal") != 0) {
                PS.WalkSound();
                ninjaController.CrossFade("Walk", aniCrossFade);
            }

            if (Input.GetAxis("Horizontal") > 0)    {
                transform.eulerAngles = new Vector3(0,0,0);
            }
            else if (Input.GetAxis("Horizontal") < 0)    {
                transform.eulerAngles = new Vector3(0, 180, 0);
            }
            if (Input.GetAxis("Vertical") < 0)    {
                transform.eulerAngles = new Vector3(0, 90, 0);
            } 
            else if (Input.GetAxis("Vertical") > 0)    {
                transform.eulerAngles = new Vector3(0,-90, 0);
            }
            print("Normal");
        }
        else if (stun == false && drunkTimer >= 0)    {
            gameObject.layer = 8;
            transform.Translate(moveSpeed * Time.deltaTime * Input.GetAxis("Vertical"), 0, -moveSpeed * Time.deltaTime * Input.GetAxis("Horizontal"), Space.World);

            if (Input.GetAxis("Vertical") != 0 || Input.GetAxis("Horizontal") != 0)     {
                PS.WalkSound();
                ninjaController.CrossFade("Walk", aniCrossFade);
            }

            if (Input.GetAxis("Horizontal") < 0)    {
                transform.eulerAngles = new Vector3(0, 0, 0);
            }
            else if (Input.GetAxis("Horizontal") > 0)    {
                transform.eulerAngles = new Vector3(0, 180, 0);
            }
            if (Input.GetAxis("Vertical") > 0)     {
                transform.eulerAngles = new Vector3(0, 90, 0);
            }
            else if (Input.GetAxis("Vertical") < 0)     {
                transform.eulerAngles = new Vector3(0, -90, 0);
            }
        }
    }
    
    public void Jump()    {
        if (stun == false)    {
            if (Time.time - lastJump > jumpTime)
            {
                if (Input.GetKeyDown(KeyCode.Space))
                {
                    myRigidBody.velocity = new Vector3(myRigidBody.velocity.x, jumpHeight, myRigidBody.velocity.z);
                    lastJump = Time.time;
                    PS.JumpSound();
                    PS.IsJumping = true;
                }
            }
            else {
                PS.IsJumping = false;
            }
        }
    }
    
    public void Crouch()    {
        if (Input.GetKeyDown(KeyCode.LeftControl) || Input.GetKeyDown(KeyCode.X))    {
            moveSpeed = crouchSpeed;

        }
        if (Input.GetKeyUp(KeyCode.LeftControl) || Input.GetKeyUp(KeyCode.X))    {
            moveSpeed = standardMoveSpeed;
        }
    }

   /* public void Run() {
        if (Input.GetKeyDown(KeyCode.LeftShift) || Input.GetKeyDown(KeyCode.Z))    {
            moveSpeed = runSpeed;
        }
        if (Input.GetKeyUp(KeyCode.LeftShift) || Input.GetKeyUp(KeyCode.Z))     {
            moveSpeed = standardMoveSpeed;
        }
    }*/

    public void SlowDown()    {
        moveSpeed = stickyFloorSpeed;
        PS.InSticky = true;
    }

    public void normalPace()    {
        moveSpeed = standardMoveSpeed;
        PS.InSticky = false;
    }

    public void Restart() {
        if(Input.GetKeyDown(KeyCode.R)) {
            SceneManager.LoadScene("Level2");
        }

    }
    public Vector3 getPlayerPos()    {
        PlayerPos = new Vector3(transform.position.x, transform.position.z, transform.position.y);
        return PlayerPos;
    }

    public Vector3 getPlayerSize()    {
        PlayerSize = new Vector3(transform.lossyScale.x, transform.lossyScale.z, transform.lossyScale.y);
        return PlayerSize;
    }

    void OnTriggerEnter(Collider other) {
        if (other.tag == "StickyFloor")    {
            onStickyFloor = true;
            SlowDown();
        }

        if (other.gameObject.CompareTag("Barrel"))    {
            timer.AddTime(addStunTime);
            plusTimer.SetActive(true);
            stun = true;
            time2 = Time.time;
            ninjaController.CrossFade("Stun", aniCrossFade);
        }
        if(other.gameObject.CompareTag("SliderFloor")) {
            onSlippery = true;       
                
            myRigidBody.AddForce(0, 0, slipperyFloorSpeed * Time.deltaTime, ForceMode.Impulse);
            print("SlipperyFloor Activated");
     
        } 
        if (other.gameObject.CompareTag("Rome")) {
            drunkTimer = timeDrunk;
        }
    }

    void OnTriggerExit(Collider other) {
        if (other.tag == "StickyFloor") {
            onStickyFloor = false;
            normalPace();
        }
        if(other.tag == "SliderFloor") {
            onSlippery = false;
        }
    }

}

