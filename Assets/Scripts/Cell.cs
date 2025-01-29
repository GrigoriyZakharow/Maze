using UnityEngine;
using UnityEngine.UI;

public class Cell : MonoBehaviour
{
  public int x;
  public int z;

  public new Renderer renderer;
  public Color green;
  public Color red;

  public GameObject cube;
  public GameObject leftWall;
  public GameObject rightWall;

  public bool leftWallIsActive = true;
  public bool rightWallIsActive = true;
  
  public GameObject panelStep;
  public Text stepSize;

  public bool visited = false;
  public new int name = -1;
  public float distance = 0;

  public Animation animationComponent;
}