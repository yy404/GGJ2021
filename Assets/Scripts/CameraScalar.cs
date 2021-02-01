using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraScalar : MonoBehaviour
{
    private Board board;
    public float cameraOffsetX;
    public float cameraOffsetY;
    public float cameraOffsetZ = -10f;
    public float padding = 0.5f;

    // Start is called before the first frame update
    void Start()
    {
        board = FindObjectOfType<Board>();
        if (board != null)
        {
            RepositionCamera(board.width - 1, board.height - 1);
        }
    }

    //// Update is called once per frame
    //void Update()
    //{

    //}

    void RepositionCamera(float x, float y)
    {
        Vector3 tempPosition = new Vector3((x / 2) + cameraOffsetX, (y / 2) + cameraOffsetY, cameraOffsetZ);
        transform.position = tempPosition;
        if (board.width != board.height)
        {
            Debug.Log("No resize for camera");
        }
        else
        {
            Camera.main.orthographicSize = (board.height / 2) + padding;
        }
    }
}
