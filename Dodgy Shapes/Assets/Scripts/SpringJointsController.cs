using UnityEngine;

public class SpringJointsController : MonoBehaviour
{
    public Rigidbody2D leftJointRb;
    public Rigidbody2D rightJointRb;

    private Transform player;

    private SpringJoint2D leftJoint;
    private SpringJoint2D rightJoint;

    private float positionX;

    public void Awake()
    {
        this.player = GameObject.FindGameObjectWithTag("Player").GetComponent<Transform>();
        this.leftJoint = this.leftJointRb.GetComponent<SpringJoint2D>();
        this.rightJoint = this.rightJointRb.GetComponent<SpringJoint2D>();

        Rigidbody2D playerBody = player.GetComponent<Rigidbody2D>();
        leftJoint.connectedBody = playerBody;
        rightJoint.connectedBody = playerBody;

        float cameraWidth = (Camera.main.orthographicSize * 2) * Camera.main.aspect;
        this.positionX = cameraWidth / 4;

        SetPosition(this.leftJointRb, -positionX, player.position.y);
        SetPosition(this.rightJointRb, positionX, player.position.y);
    }

    public void Update()
    {
        SetPosition(this.leftJointRb, -positionX, player.position.y);
        SetPosition(this.rightJointRb, positionX, player.position.y);
    }

    public void EnableDisableJoints()
    {

        if (leftJoint.enabled)
        {
            rightJoint.enabled = true;
            leftJoint.enabled = false;
        }
        else
        {
            rightJoint.enabled = false;
            leftJoint.enabled = true;
        }
    }

    public void TurnoOffJoints()
    {
        rightJoint.enabled = false;
        leftJoint.enabled = false;
    }

    public void TurnOnJoints(bool isLeftTurnedOn)
    {
        if (isLeftTurnedOn)
        {
            this.leftJoint.enabled = true;
            this.rightJoint.enabled = false;
        }
        else
        {
            this.rightJoint.enabled = true;
            this.leftJoint.enabled = false;
        }
    }

    public void ChangeFrequency(float frequency)
    {
        this.leftJoint.frequency = frequency;
        this.rightJoint.frequency = frequency;
    }

    private void SetPosition(Rigidbody2D point, float x, float y)
    {
        Vector2 currentPosition = point.position;
        currentPosition.x = x;
        currentPosition.y = y;
        point.position = currentPosition;
    }
}
