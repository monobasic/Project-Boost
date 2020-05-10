using UnityEngine;
using UnityEngine.SceneManagement;

public class Rocket : MonoBehaviour
{
    [SerializeField] float mainThrust = 300f;
    [SerializeField] float rcsThrust = 1200f;
    Rigidbody rigidBody;
    AudioSource audioSource;

    enum State { Alive, Dying, Transcending };
    State state = State.Alive;

    private void LoadNextScene() {
        SceneManager.LoadScene(1);
    }
    private void LoadFirstScene() {
        SceneManager.LoadScene(0);
    }

    // Start is called before the first frame update
    void Start()
    {
        rigidBody = GetComponent<Rigidbody>();
        audioSource = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        if (state == State.Alive) {
            Thrust();
            Rotate();
        }
    }

    void OnCollisionEnter(Collision collision) {
        if (state != State.Alive) {
            return;
        }

        switch(collision.gameObject.tag) {
            case "Friendly":
                break;
            case "Finish":
                state = State.Transcending;
                Invoke("LoadNextScene", 1f);
                break;
            default:
                print("dead");
                state = State.Dying;
                Invoke("LoadFirstScene", 1f);
                break;
        }
    }

    private void Thrust()
    {
        float thisFrameThrust = mainThrust * Time.deltaTime;

        if (Input.GetKey(KeyCode.Space))
        {
            rigidBody.AddRelativeForce(Vector3.up * thisFrameThrust);
            if (!audioSource.isPlaying)
            {
                audioSource.Play();
            }
        }
        else
        {
            audioSource.Stop();
        }
    }

    private void Rotate()
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
