using UnityEngine;
using UnityEngine.UI;

public class ButtonManager : MonoBehaviour {
  private const int maxSizeCountCells = 100;
  private const int minSizeCountCells = 4;

  public Text textsizeMaze;
  public MazeManager mazeGenerate;
  public GameObject cellPref;
  public GameObject buttonShowSearch;
  public GameObject buttonResetSearch;
  public Text kindSearchPanel;
  public bool flagActiveFreeCamera;

  public void ActiveFreeCamera() {
    flagActiveFreeCamera = !flagActiveFreeCamera;
  }

  public void ChangeKindSearch() {
    if (kindSearchPanel.text == "BFS") kindSearchPanel.text = "DFS";
    else kindSearchPanel.text = "BFS";
  }

  public void AddCell() {
    if (mazeGenerate.sizeMaze != maxSizeCountCells && mazeGenerate.stateAction == StateAction.Nothing) {
      mazeGenerate.sizeMaze += 2;
      textsizeMaze.text = mazeGenerate.sizeMaze.ToString() + "x" + mazeGenerate.sizeMaze.ToString();
    }
  }

  public void SubCell() {
    if (mazeGenerate.sizeMaze != minSizeCountCells && mazeGenerate.stateAction == StateAction.Nothing) {
      mazeGenerate.sizeMaze -= 2;
      textsizeMaze.text = mazeGenerate.sizeMaze.ToString() + "x" + mazeGenerate.sizeMaze.ToString();
    }
  }

  public void ExitGame() {
    Application.Quit();
  }

  public void ShowCells() {
    if (mazeGenerate.stateAction == StateAction.Nothing) {
      mazeGenerate.CreatCells(cellPref);
    }
  }

  public void ShowMaze() {
    if (mazeGenerate.stateAction == StateAction.Created && mazeGenerate.buttonGenerate.activeInHierarchy is true) {
      mazeGenerate.GenerateMaze(mazeGenerate.maze);
    }
  }

  public void DestroyCells() {
    if (mazeGenerate.stateAction == StateAction.Created || mazeGenerate.stateAction == StateAction.Generated) {
      mazeGenerate.listCells.Reverse();
      mazeGenerate.stateAction = StateAction.Removal;
      ResetSearch();
    }
  }

  public void RegenerateMaze() {
    if (mazeGenerate.stateAction == StateAction.Generated && mazeGenerate.buttonRegenerate.activeInHierarchy is true) {
      mazeGenerate.RegenerateMaze();
      ResetSearch();
    }
  }

  public void ActivateSearch() {
    if (mazeGenerate.stateAction == StateAction.Generated) {
      if (kindSearchPanel.text == "BFS") {
        (mazeGenerate.visitedCells, mazeGenerate.shortestPath) = mazeGenerate.graph.BFS();
        mazeGenerate.typeSearch = TypeSearch.BFS;
      }
      else {
        (mazeGenerate.visitedCells, mazeGenerate.shortestPath) = mazeGenerate.graph.DFS();
        mazeGenerate.typeSearch = TypeSearch.DFS;
      }
      ChangeShowResetSearch(mazeGenerate.typeSearch);
    }
  }

  public void ResetSearch() {
    mazeGenerate.typeSearch = TypeSearch.Nothing;
    foreach (var item in mazeGenerate.maze) {
      item.panelStep.SetActive(false);
      item.renderer.material.color = item.GetComponent<Cell>().red;
      item.stepSize.text = "";
      item.stepSize.gameObject.SetActive(false);
    }
    ChangeShowResetSearch(mazeGenerate.typeSearch);
  }

  public void ChangeShowResetSearch(TypeSearch typeSearch) {
    switch (typeSearch) {
      case TypeSearch.Nothing:
        buttonShowSearch.SetActive(true);
        buttonResetSearch.SetActive(false);
        break;
      default:
        buttonShowSearch.SetActive(false);
        buttonResetSearch.SetActive(true);
        break;
    }
  }
}