﻿using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Rocket : MonoBehaviour
{
  [SerializeField] float mainThrust = 300f;
  [SerializeField] float rcsThrust = 1200f;
  [SerializeField] AudioClip mainEngine;
  [SerializeField] AudioClip success;
  [SerializeField] AudioClip death;
  [SerializeField] ParticleSystem mainEngineParticles;
  [SerializeField] ParticleSystem successParticles;
  [SerializeField] ParticleSystem deathParticles;

  Rigidbody rigidBody;
  AudioSource audioSource;

  enum State { Alive, Dying, Transcending };
  State state = State.Alive;

  private bool collisionsDisabled = false;

  // Start is called before the first frame update
  void Start()
  {
    rigidBody = GetComponent<Rigidbody>();
    audioSource = GetComponent<AudioSource>();
  }

  // Update is called once per frame
  void Update()
  {
    if (state == State.Alive)
    {
      RespondToThrustInput();
      RespondToRotationInput();
    }

    if (Debug.isDebugBuild)
    {
      RespondToDebugKeys();
    }
  }

  private void RespondToDebugKeys()
  {
    if (Input.GetKeyDown(KeyCode.L))
    {
      LoadNextScene();
    }
    else if (Input.GetKeyDown(KeyCode.C))
    {
        collisionsDisabled = ! collisionsDisabled;
    }
  }

  private void LoadNextScene()
  {
    SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
  }

  private void LoadFirstScene()
  {
    SceneManager.LoadScene(0);
  }

  void OnCollisionEnter(Collision collision)
  {
    if (state != State.Alive || collisionsDisabled) { return; } // Ignore collisions when dead

    switch (collision.gameObject.tag)
    {
      case "Friendly":
        break;
      case "Finish":
        state = State.Transcending;
        audioSource.Stop();
        audioSource.PlayOneShot(success);
        successParticles.Play();
        Invoke("LoadNextScene", 1f);
        break;
      default:
        state = State.Dying;
        audioSource.Stop();
        audioSource.PlayOneShot(death);
        mainEngineParticles.Stop();
        deathParticles.Play();
        Invoke("LoadFirstScene", 1f);
        break;
    }
  }

  private void RespondToThrustInput()
  {
    if (Input.GetKey(KeyCode.Space))
    {
      ApplyThrust(Time.deltaTime);
    }
    else
    {
      audioSource.Stop();
      mainEngineParticles.Stop();
    }
  }

  private void ApplyThrust(float deltaTime)
  {
    float thisFrameThrust = mainThrust * Time.deltaTime;
    rigidBody.AddRelativeForce(Vector3.up * thisFrameThrust);

    if (!mainEngineParticles.isPlaying)
    {
      mainEngineParticles.Play();
    }

    if (!audioSource.isPlaying)
    {
      audioSource.PlayOneShot(mainEngine);
    }
  }

  private void RespondToRotationInput()
  {
    rigidBody.freezeRotation = true; // Take manual control of rotation
    float thisFrameRotation = rcsThrust * Time.deltaTime;

    if (Input.GetKey(KeyCode.D))
    {
      transform.Rotate(-Vector3.forward * thisFrameRotation);
    }
    else if (Input.GetKey(KeyCode.A))
    {
      transform.Rotate(Vector3.forward * thisFrameRotation);
    }

    rigidBody.freezeRotation = false; // Resume physic control of rotation
  }
}