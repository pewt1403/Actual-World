using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainCharMovement : MonoBehaviour
{
    Vector3 m_Movement = Vector3.zero;
    Animator m_Animator;
    CharacterController m_CharControl;
    
 
    private float speed = 2.5f;
    private float rotationSpeed = 240.0f;
    private float timer = 0.8f;
    private bool atkStatus = false;
    private bool isWalking;
    float gravity = 20.0f;

    public AudioSource _source;
    public AudioClip walkSound;
    public AudioClip runSound;
    private float _walk = 0.45f;
    private float _run = 0.6f;
    private float _timerS = 0f;


    // Start is called before the first frame update
    void Start()
    {
        AudioSource[] audioSources = GetComponents<AudioSource>();
        _source = audioSources[0];
        walkSound = audioSources[0].clip;
        runSound = audioSources[1].clip;

        m_Animator = GetComponent<Animator>();
        m_CharControl = GetComponent<CharacterController>();
    }

    // Update is called once per frame
    void Update()
    {
        // Get WASD
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");
        bool shiftL = Input.GetKey(KeyCode.LeftShift);
 
        // Cal Forward Vector
        Vector3 camForward = Vector3.Scale(Camera.main.transform.forward, new Vector3(1, 0, 1)).normalized;
        Vector3 move = vertical * camForward + horizontal * Camera.main.transform.right;

        if (move.magnitude > 1f) move.Normalize();

        //Cal rotation for player
        move = transform.InverseTransformDirection(move);

        // Get Euler Angles
        float turnAmount = Mathf.Atan2(move.x, move.z);
        transform.Rotate(0, turnAmount * rotationSpeed * Time.deltaTime, 0);

        if (m_CharControl.isGrounded)
        {

            if (atkStatus == true)
            {
                m_Movement = Vector3.zero;
                    
            }
            else
            {
                bool hasHorizontalInput = !Mathf.Approximately(horizontal, 0f);
                bool hasVerticalInput = !Mathf.Approximately(vertical, 0f);
                isWalking = hasHorizontalInput || hasVerticalInput;
                m_Animator.SetBool("IsWalking", isWalking);
                m_Animator.SetBool("IsRunning", shiftL);
                float realSpeed;
                if (shiftL && isWalking)
                {
                    realSpeed = speed * 2;
                }
                else
                {
                    realSpeed = speed;
                }

                m_Movement = transform.forward * move.magnitude;
                m_Movement *= realSpeed;
                
            }
           
        }
        if (shiftL && isWalking && _timerS > _walk)
        {
            _source.volume = Random.Range(0.2f, 0.4f) * AudioListener.volume;
            _source.pitch = Random.Range(0.9f, 1.1f);
            _source.PlayOneShot(runSound);
            _timerS = 0f;
        }
        else if (isWalking && _timerS > _run)
        {
            _source.volume = Random.Range(0.1f, 0.3f) * AudioListener.volume ;
            _source.pitch = Random.Range(0.9f, 1.1f);
            _source.PlayOneShot(walkSound);
            _timerS = 0f;
        }
        
        _timerS += Time.deltaTime;
        m_CharControl.Move(m_Movement * Time.deltaTime);
        m_Movement.y -= gravity * Time.deltaTime;
        

    }

    private void FootL()
    {

    }
    private void FootR()
    {

    }
    private void Hit()
    {

    }
    public void setAtkStatus(bool atking)
    {
        atkStatus = atking;
    }

}
