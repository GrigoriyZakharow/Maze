using System.Collections.Generic;
using UnityEngine;

public class MazeManager : MonoBehaviour {
  private int index = 0;
  private float timer;
  private const float time = 0.05f;
  private int countCellAction;
  private bool flagRestorationWalls = false;

  public StateAction stateAction = StateAction.Nothing;
  public TypeSearch typeSearch = TypeSearch.Nothing;
  public Graph graph = new Graph();
  public AnimationClip clipDestroy;
  public Cell[,] maze;
  public int sizeMaze = 4;
  public GameObject buttonGenerate;
  public GameObject buttonRegenerate;
  public List<Cell> listCells = new List<Cell>();
  public List<Vertex> visitedCells = new List<Vertex>();
  public List<Vertex> shortestPath = new List<Vertex>();

  private void Start() {
    timer = time;
  }

  public void ShowPath() {
    if (shortestPath.Count != 0) {
      ShowStep();
      timer = 0.1f;
    }
  }

  public void ShowStep() {
    if (visitedCells.Count != 0) {
      visitedCells[0].cell.panelStep.SetActive(true);
      visitedCells[0].cell.stepSize.gameObject.SetActive(true);
      visitedCells.RemoveAt(0);
    }
    else {
      shortestPath[0].cell.renderer.material.color = shortestPath[0].cell.green;
      shortestPath.RemoveAt(0);

      if (shortestPath.Count == 0) typeSearch = TypeSearch.Nothing;
    }
  }

  public void ShowAndHideWallet() {
    switch (stateAction) {
      case StateAction.Creation:
        if (index < listCells.Count) {
          countCellAction += maze.GetLength(0);

          while (index < countCellAction) listCells[index++].gameObject.SetActive(true);
        }
        else ChangeIndexAndStateAction();
        break;
      case StateAction.Generation:
        countCellAction += maze.GetLength(0);

        while (index < countCellAction) {
          if (listCells[index].leftWallIsActive is false) listCells[index].leftWall.SetActive(false);
          if (listCells[index].rightWallIsActive is false) listCells[index].rightWall.SetActive(false);

          index++;
        }

        if (index == listCells.Count) {
          ChangeIndexAndStateAction();
          ChangeButtonShowResetGeneration();
        }
        break;
      case StateAction.Regeneration:
        if (flagRestorationWalls is true) {
          countCellAction += maze.GetLength(0);

          while (index < countCellAction) {
            listCells[index].leftWall.SetActive(true);
            listCells[index++].rightWall.SetActive(true);
          }

          for (int i = 0; i < maze.GetLength(0); i++) {
            for (int j = 0; j < maze.GetLength(1); j++) {
              if (i == sizeMaze - 1) maze[i, j].rightWall.SetActive(false);
              if (j == sizeMaze - 1) maze[i, j].leftWall.SetActive(false);
            }
          }

          if (index == listCells.Count) {
            index = countCellAction = 0;
            flagRestorationWalls = false;
          }
        }
        else {
          countCellAction += maze.GetLength(0);

          while (index < countCellAction) {
            if (listCells[index].leftWallIsActive is false) listCells[index].leftWall.SetActive(false);
            if (listCells[index].rightWallIsActive is false) listCells[index].rightWall.SetActive(false);

            index++;
          }

          if (index == listCells.Count) ChangeIndexAndStateAction();
        }
        break;
      case StateAction.Removal:
        if (index < listCells.Count) {
          countCellAction += maze.GetLength(0);

          while (index < countCellAction) {
            listCells[index].animationComponent.clip = clipDestroy;
            listCells[index++].animationComponent.Play();
          }
        }

        if (index == listCells.Count) {
          ChangeIndexAndStateAction();
          ChangeButtonShowResetGeneration();
          listCells = new List<Cell>();
        }
        break;
    }
  }

  public void ChangeIndexAndStateAction() {
    index = 0;
    timer = time;
    countCellAction = 0;
    switch (stateAction) {
      case StateAction.Regeneration:
        stateAction = StateAction.Generated;
        break;
      case StateAction.Creation:
        stateAction = StateAction.Created;
        break;
      case StateAction.Generation:
        stateAction = StateAction.Generated;
        break;
      case StateAction.Removal:
        stateAction = StateAction.Nothing;
        break;
    }
  }

  private void Update() {
    timer -= Time.deltaTime;
    if (typeSearch == TypeSearch.Nothing) {
      if (timer <= 0f) {
        ShowAndHideWallet();
        timer = time;
      }
    }
    else {
      if (timer < 0f) {
        ShowPath();
        timer = time * 2;
      }
    }
  }

  public void GenerateMaze(Cell[,] maze) {
    Cell current = maze[0, 0];
    current.visited = true;
    graph = new Graph();

    Stack<Cell> stack = new Stack<Cell>();
    do {
      List<Cell> unvisibre = new List<Cell>();

      int x = current.x;
      int z = current.z;

      if (x > 0 && !maze[x - 1, z].visited) unvisibre.Add(maze[x - 1, z]);
      if (z > 0 && !maze[x, z - 1].visited) unvisibre.Add(maze[x, z - 1]);
      if (x < sizeMaze - 2 && !maze[x + 1, z].visited) unvisibre.Add(maze[x + 1, z]);
      if (z < sizeMaze - 2 && !maze[x, z + 1].visited) unvisibre.Add(maze[x, z + 1]);

      if (unvisibre.Count > 0) {
        Cell chosen = unvisibre[Random.Range(0, unvisibre.Count)];
        RemoveWall(current, chosen);

        chosen.visited = true;
        stack.Push(chosen);
        current = chosen;
      }

      else if (stack.Count > 0) {
        current = stack.Pop();

        if (stack.Count > 0) {
          Cell next = stack.Peek();
          RemoveWall(current, next);
        }
      }

    } while (stack.Count > 0);

    maze[maze.GetLength(0) - 1, maze.GetLength(1) - 2].leftWallIsActive = false;
    maze[0, 0].leftWallIsActive = false;

    graph.ConvertToGraph(maze);

    if (buttonRegenerate.activeInHierarchy is false) stateAction = StateAction.Generation;
    else stateAction = StateAction.Regeneration;
  }

  private void RemoveWall(Cell cell1, Cell cell2) {
    if (cell1.x == cell2.x) {
      if (cell1.z > cell2.z) cell1.rightWallIsActive = false;
      else cell2.rightWallIsActive = false;
    }
    else {
      if (cell1.x > cell2.x) cell1.leftWallIsActive = false;
      else cell2.leftWallIsActive = false;
    }
  }

  public void CreatCells(GameObject cellPref) {
    int nameInt = 0;
    Cell cellContainer;

    maze = new Cell[sizeMaze, sizeMaze];

    for (int i = 0; i < maze.GetLength(0); i++) {
      for (int j = 0; j < maze.GetLength(1); j++) {
        cellContainer = Instantiate(cellPref, new Vector3(i, 0, j), Quaternion.identity).GetComponent<Cell>();
        cellContainer.gameObject.SetActive(false);

        if (i == sizeMaze - 1) {
          cellContainer.rightWall.SetActive(false);
          cellContainer.cube.SetActive(false);
        }

        if (j == sizeMaze - 1) {
          cellContainer.leftWall.SetActive(false);
          cellContainer.cube.SetActive(false);
        }

        maze[i, j] = cellContainer;
        maze[i, j].x = i;
        maze[i, j].z = j;

        if (i < maze.GetLength(0) - 1 && j < maze.GetLength(1) - 1) maze[i, j].name = nameInt++;

        listCells.Add(cellContainer);
      }
    }
    stateAction = StateAction.Creation;
  }

  public void RegenerateMaze() {
    flagRestorationWalls = true;

    for (int i = 0; i < maze.GetLength(0); i++) {
      for (int j = 0; j < maze.GetLength(1); j++) {
        if (i != sizeMaze - 1 && j != sizeMaze - 1) {
          maze[i, j].rightWallIsActive = maze[i, j].leftWallIsActive = true;
          maze[i, j].visited = false;
        }
      }
    }
    ChangeButtonShowResetGeneration();
    GenerateMaze(maze);
  }

  public void ChangeButtonShowResetGeneration() {
    switch (stateAction) {
      case StateAction.Generated:
        buttonGenerate.SetActive(false);
        buttonRegenerate.SetActive(true);
        break;
      case StateAction.Nothing:
        buttonGenerate.SetActive(true);
        buttonRegenerate.SetActive(false);
        break;
    }
  }
}