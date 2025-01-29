using UnityEngine;

public class CameraScript : MonoBehaviour
{
  private Vector3 target;
  private float distance = 100.0f;
  private float focus;
  private Vector3 offset;
  private float zoomSpeed = 2.0f;
  private float speed = 5.0f;
  private int valueM;

  public MazeManager mazeGenerate;
  public ButtonManager buttonManager;
  public new Camera camera;
 
  private void Update()
  {
    if (buttonManager.flagActiveFreeCamera)
    {
      if (Input.GetMouseButton(0))
      {
        float moveX = Input.GetAxis("Mouse X") * speed * Time.deltaTime;
        float moveY = Input.GetAxis("Mouse Y") * speed * Time.deltaTime;

        transform.Translate(moveX, moveY, 0);
      }

      float scroll = Input.GetAxis("Mouse ScrollWheel");
      Camera.main.orthographicSize -= scroll * zoomSpeed;
      Camera.main.orthographicSize = Mathf.Max(Camera.main.orthographicSize, 0.1f);
    }
    else
    {
      target = new Vector3(mazeGenerate.sizeMaze / 2, 0, mazeGenerate.sizeMaze / 2);
      transform.position = target + offset - transform.forward * distance;
      camera.orthographicSize = target.magnitude;
    }
  }
}
