﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]

public class TapController : MonoBehaviour
{

    public delegate void PlayerDelegate();
    public static event PlayerDelegate OnPlayerDied;
    public static event PlayerDelegate OnPlayerScored;

   public float tapForce=2000;
   public float tiltSmooth=5;
   public Vector3 startPos;

    public AudioSource tapAudio;
    public AudioSource scoreAudio;
    public AudioSource gameoverAudio;

   Rigidbody2D rigidbody1;
   Quaternion downRotation;
   Quaternion forwardRotation;

   GameManager game;

   void Start(){
       rigidbody1=GetComponent<Rigidbody2D>();
       downRotation=Quaternion.Euler(0,0,-90);
       forwardRotation = Quaternion.Euler(0,0,35);
       game =GameManager.Instance;
       rigidbody1.simulated= false;
   }

    void OnEnable(){
        GameManager.OnGameStarted +=OnGameStarted;
        GameManager.OnGameOverConfirmed +=OnGameOverConfirmed;
    }

    void OnDisable(){
        GameManager.OnGameStarted -= OnGameStarted;
        GameManager.OnGameOverConfirmed -= OnGameOverConfirmed;
    }

    void OnGameStarted(){
        rigidbody1.velocity =Vector3.zero;
        rigidbody1.simulated=true;
    }

    void OnGameOverConfirmed(){
        transform.localPosition = startPos;
        transform.rotation = Quaternion.identity;
    }

   void Update(){
        if(game.GameOver) return;
        if(Input.GetMouseButtonDown(0)){
            tapAudio.Play();
            rigidbody1.velocity=Vector3.zero;
            rigidbody1.AddForce(Vector2.up * tapForce,ForceMode2D.Force);
             transform.rotation =forwardRotation;

        }
        transform.rotation =Quaternion.Lerp(transform.rotation, downRotation,tiltSmooth * Time.deltaTime);
   }

   void OnTriggerEnter2D(Collider2D col){
        if(col.gameObject.tag == "ScoreZone"){
            //score
            OnPlayerScored(); //event sent to gamemanager;
                //score sound
                scoreAudio.Play();
        }
        if(col.gameObject.tag=="DeadZone"){
                rigidbody1.simulated=false;
                //dead
            OnPlayerDied(); //event sent to gamemanager;
                //sound
                gameoverAudio.Play();
        }
   }
}