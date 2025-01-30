using System.Collections.Generic;

public class Vertex {
  public int name;
  public Cell cell;

  public Vertex(int name, Cell cell) {
    this.name = name;
    this.cell = cell;
  }

  public override bool Equals(object obj) {
    if (obj == null || GetType() != obj.GetType()) {
      return false;
    }

    Vertex v = (Vertex)obj;
    return name == v.name;
  }

  public override int GetHashCode() {
    return name.GetHashCode();
  }
}

public class Edge {
  public Vertex from;
  public Vertex to;

  public Edge(Vertex from, Vertex to) {
    this.from = from;
    this.to = to;
  }
}

public class Graph {
  public Vertex startPosition;
  public Vertex finishPosition;
  public List<Vertex> vertexes = new List<Vertex>();
  public List<Edge> edges = new List<Edge>();

  public void AddVertex(Vertex vertext) {
    vertexes.Add(vertext);
  }

  public void AddEdge(Vertex from, Vertex to) {
    Edge edge = new Edge(from, to);
    edges.Add(edge);
    edge = new Edge(to, from);
    edges.Add(edge);
  }

  public List<Vertex> GetVertexLists(Vertex vertex) {
    List<Vertex> result = new List<Vertex>();

    foreach (var edge in this.edges) {
      if (edge.from.name == vertex.name) result.Add(edge.to);
    }

    return result;
  }

  public void ConvertToGraph(Cell[,] maze) {
    startPosition = new Vertex(maze[0, 0].name, maze[0, 0]);
    finishPosition = new Vertex(maze[maze.GetLength(0) - 2, maze.GetLength(1) - 2].name, maze[maze.GetLength(0) - 2, maze.GetLength(1) - 2]);
    List<int> visited = new List<int>();

    for (int x = 0; x < maze.GetLength(0) - 1; x++) {
      for (int z = 0; z < maze.GetLength(1) - 1; z++) {
        Vertex currentVertex = new Vertex(maze[x, z].name, maze[x, z]);

        AddEdgeIfValid(maze, visited, currentVertex, x + 1, z, maze[x + 1, z].leftWallIsActive);
        AddEdgeIfValid(maze, visited, currentVertex, x, z + 1, maze[x, z + 1].rightWallIsActive);
      }
    }
  }

  private void AddEdgeIfValid(Cell[,] maze, List<int> visited, Vertex fromVertex, int x, int z, bool activeWall) {
    if (activeWall is false) {
      Vertex toVertex = new Vertex(maze[x, z].name, maze[x, z]);
      this.AddEdge(fromVertex, toVertex);

      if (!visited.Contains(maze[x, z].name)) {
        this.AddVertex(toVertex);
        this.AddVertex(fromVertex);
        visited.Add(maze[x, z].name);
        visited.Add(fromVertex.cell.name);
      }
    }
  }

  public (List<Vertex>, List<Vertex>) BFS() {
    Queue<Vertex> queue = new Queue<Vertex>();
    List<Vertex> shortestPath = new List<Vertex>();
    List<Vertex> visitedCells = new List<Vertex>();
    Dictionary<Vertex, Vertex> predecessors = new Dictionary<Vertex, Vertex>();
    Vertex current = finishPosition;

    visitedCells.Add(startPosition);
    queue.Enqueue(startPosition);
    predecessors[startPosition] = null;
    startPosition.cell.distance = 1;
    startPosition.cell.stepSize.text = startPosition.cell.distance.ToString();


    while (queue.Count > 0) {
      Vertex vertex = queue.Dequeue();

      if (vertex.Equals(finishPosition)) {
        break;
      }

      foreach (var neighbor in this.GetVertexLists(vertex)) {
        if (!visitedCells.Contains(neighbor)) {
          visitedCells.Add(neighbor);
          queue.Enqueue(neighbor);
          predecessors[neighbor] = vertex;
          neighbor.cell.distance = vertex.cell.distance + 1;
          neighbor.cell.stepSize.text = neighbor.cell.distance.ToString();
        }
      }
    }

    GetShortestPath(current, shortestPath, predecessors);

    return (visitedCells, shortestPath);
  }

  private void GetShortestPath(Vertex current, List<Vertex> shortestPath, Dictionary<Vertex, Vertex> predecessors) {
    while (current != null) {
      shortestPath.Add(current);
      current = predecessors[current];
    }
    shortestPath.Reverse();
  }

  public (List<Vertex>, List<Vertex>) DFS() {
    List<Vertex> l = new List<Vertex>();
    List<Vertex> visitedCells = new List<Vertex>();
    List<Vertex> shortestPath = new List<Vertex>();
    startPosition.cell.distance = 1;
    startPosition.cell.stepSize.text = startPosition.cell.distance.ToString();
    DFSUtil(startPosition, finishPosition, visitedCells, shortestPath, l);
    return (l, shortestPath);
  }

  private bool DFSUtil(Vertex current, Vertex endVertex, List<Vertex> visitedCells, List<Vertex> shortestPath, List<Vertex> l) {
    visitedCells.Add(current);
    shortestPath.Add(current);
    l.Add(current);

    if (current.Equals(endVertex)) {
      return true;
    }

    foreach (var neighbor in GetVertexLists(current)) {
      if (!visitedCells.Contains(neighbor)) {
        neighbor.cell.distance = current.cell.distance + 1;
        neighbor.cell.stepSize.text = neighbor.cell.distance.ToString();
        if (DFSUtil(neighbor, endVertex, visitedCells, shortestPath, l)) {
          return true;
        }
      }
    }

    shortestPath.Remove(current);
    return false;
  }
}