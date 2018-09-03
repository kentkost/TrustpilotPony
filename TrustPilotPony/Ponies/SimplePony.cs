using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace TrustPilotPony
{
    //This is a simple pony that will just go for the end-point. Irregardless of where the dokumon is.
    public class SimplePony
    {
        protected string name = "";
        protected Maze maze;
        protected string log = "";
        protected Dictionary<int, Node> map = new Dictionary<int, Node>();
        protected Dictionary<int, Coor> coordinates = new Dictionary<int, Coor>();
        protected int numOfPoints;
        protected delegate bool HasBeenEvaluatedDel(int neighbour, List<Node> closedSet);
        protected HasBeenEvaluatedDel HasBeenEvaluatedHan;

        public SimplePony(Maze m)
        {
            name = "SimplePony";
            maze = m;
            coordinates = m.coordinates;
            foreach (KeyValuePair<int, Coor> pair in maze.coordinates) {
                map.Add(pair.Key, new Node(pair.Key));
            }
            numOfPoints = maze.height * maze.width;
            HasBeenEvaluatedHan = HasBeenEvaluated;
        }

        virtual public void ExecutePlan()
        {
            List<int> path = AStar(maze.ponyPos, maze.endPoint);
            List<string> dirs = Directions(path);
            int i = 1;
            AppendToLog(ref i, "", "START");
            foreach (string s in dirs) {
                string state = WebHandler.Instance.MakeMove(maze.mazeID, s);
                AppendToLog(ref i, state, s);
                JObject jobj = JObject.Parse(state);
                string temp = (string)jobj["state"];
                if(temp == "over") {
                    Console.WriteLine("Game Over. Pony is dead");
                    break;
                }
                Console.WriteLine(s);
                Console.Write(WebHandler.Instance.GetMazeVisual(maze.mazeID));
                System.Threading.Thread.Sleep(200);
                i++;
            }
            WriteLogFile();
        }

        virtual public List<string> Directions(List<int> path)
        {
            List<string> directions = new List<string>();
            int start = maze.ponyPos;
            int end = maze.endPoint;

            if (start == end || path.Count==0) {
                return new List<string>() { "stay" };
            }

            for (int i = 0; i < path.Count - 1; i++) {
                Coor from = coordinates[path[i]];
                Coor to = coordinates[path[i + 1]];
                if (from.x != to.x) {
                    if (from.x + 1 == to.x) {
                        directions.Add("east");
                    }
                    else if (from.x - 1 == to.x) {
                        directions.Add("west");
                    }
                }
                else {
                    if (from.y + 1 == to.y) {
                        directions.Add("south");
                    }
                    else if (from.y - 1 == to.y) {
                        directions.Add("north");
                    }
                }
            }
            return directions;
        }

        virtual protected List<int> AStar(int start, int end)
        {
            List<Node> closedSet = new List<Node>();
            List<Node> openSet = new List<Node>();
            map[start].gScore = 0;
            map[start].fScore = DistanceBetween(coordinates[start], coordinates[end]);
            openSet.Add(map[start]);

            Node Goal = map[end];

            while (openSet.Count > 0) {
                openSet = openSet.OrderBy(n => n.fScore).ToList();
                Node cur = openSet.First();
                if(cur.id == Goal.id) {
                    return Backtrace();
                }

                openSet.Remove(cur);
                closedSet.Add(cur);

                foreach(int neighbour in coordinates[cur.id].neighbours) {
                    if(HasBeenEvaluated(neighbour, closedSet)) {
                        continue;
                    }

                    double tentative_gScore = cur.gScore + 1;

                    if (!HasBeenEvaluatedHan(neighbour, openSet)) {
                        openSet.Add(map[neighbour]);
                    }
                    else if(tentative_gScore >= map[neighbour].gScore) {
                        continue;
                    }

                    map[neighbour].cameFrom = cur.id;
                    map[neighbour].gScore = tentative_gScore;
                    map[neighbour].fScore = map[neighbour].gScore + DistanceBetween(coordinates[neighbour], coordinates[maze.endPoint]);
                }
            }
            return new List<int>();
        }

        virtual protected bool HasBeenEvaluated(int neighbour, List<Node> closedSet)
        {
            foreach(Node n in closedSet) {
                if(n.id == neighbour) {
                    return true;
                }
            }
            return false;
        }

        virtual protected double DistanceBetween(Coor start, Coor end)
        {
            int num1 = (start.x - end.x) * (start.x - end.x);
            int num2 = (start.y - end.y) * (start.y - end.y);
            return Math.Sqrt(num1 + num2);
        }

        protected List<int> Backtrace()
        {
            List<int> path = new List<int>();

            int cur=maze.endPoint;
            path.Add(maze.endPoint);
            while (cur != maze.ponyPos) {
                path.Add(map[cur].cameFrom);
                cur = map[cur].cameFrom;
            }

            path.Reverse();
            return path;
        }

        public void WriteLogFile()
        {
            string path = System.IO.Directory.GetCurrentDirectory();
            path += "\\logs\\" + maze.difficulty+"\\";
            if (!Directory.Exists(path)) {
                Directory.CreateDirectory(path);
            }
            File.WriteAllText(path+maze.mazeID+".txt", log);
        }

        public void AppendToLog(ref int stage, string state, string direction, string misc="")
        {
            log += "\nStage: " + stage;
            log += maze.PrintAll();
            log += state;
            log += "\nDirection: " + direction + "\n";
            log += WebHandler.Instance.GetMazeVisual(maze.mazeID);
            log += "\n______________________________________________________________________________________________________________";
            stage++;
        }

        public void AppendToLog(string misc = "")
        {
            log += misc;
        }
    }

    public class Node
    {
        public double gScore;
        public double fScore;
        public int cameFrom;
        public int id;
        public Node(int id) {
            this.id = id;
            gScore = Int32.MaxValue;
            fScore = Int32.MaxValue;
        }
    }
}
