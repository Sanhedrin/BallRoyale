using UnityEngine;
using System.Collections;

public partial class Ball : MonoBehaviour
{

    private bool m_Grounded = true;

	[SerializeField]
	private float m_MoveSpeed = 20;

    [SerializeField]
    private float m_JumpSpeed = 20;

	private Rigidbody m_RigidBody;

	// Use this for initialization
	void Start ()
    {
		m_RigidBody = GetComponent<Rigidbody> ();
	}
	
	// Update is called once per frame
    void Update()
    {
        if (Input.GetKey(KeyCode.LeftArrow))
        {
            m_RigidBody.AddForce(Vector3.left * m_MoveSpeed);
        }
        if (Input.GetKey(KeyCode.RightArrow))
        {
            m_RigidBody.AddForce(Vector3.right * m_MoveSpeed);
        }
        if (Input.GetKey(KeyCode.UpArrow))
        {
            m_RigidBody.AddForce(Vector3.forward * m_MoveSpeed);
        }
        if (Input.GetKey(KeyCode.DownArrow))
        {
            m_RigidBody.AddForce(Vector3.back * m_MoveSpeed);
        }
        if (Input.GetKeyDown(KeyCode.Space) && m_Grounded)
        {
            m_RigidBody.AddForce(Vector3.up * m_JumpSpeed);
        }
    }

    public void OnCollisionEnter(Collision i_TheCollision)
    {
        if (i_TheCollision.gameObject.name == "Cube")
        {
            m_Grounded = true;
        }
    }

    public void OnCollisionExit(Collision i_TheCollision)
    {
        if (i_TheCollision.gameObject.name == "Cube")
        {
            m_Grounded = false;
        }
    }

    
}
