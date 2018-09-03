using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace TrustPilotPony
{
    //This pony recalculates it's plan after every move in order to avoid the Domokun.
    //DOesn't search in a state space just treats the domokun as a wall. At level 7 it starts tor un away from domokun.
    class AdjustingPony : SimplePony
    {
        
        public AdjustingPony(Maze m) : base(m)
        {
            name = "AdjustingPony";
            HasBeenEvaluatedHan = HasBeenEvaluatedDomokunWall;
        }

        public override void ExecutePlan()
        {
            string state = "";
            string move = "";
            int i = 1;
            AppendToLog(ref i, state, "START");
            do {
                List<int> path = AStar(maze.ponyPos, maze.endPoint);
                move = Directions(path).First();
                state = WebHandler.Instance.MakeMove(maze.mazeID, move);
                AppendToLog(ref i, state, move);
                JObject jobj = JObject.Parse(state);
                state = (string)jobj["state"];
                maze.UpdateInfo();

                Console.Write(WebHandler.Instance.GetMazeVisual(maze.mazeID));
                System.Threading.Thread.Sleep(200);
                Console.Clear();
                Console.WriteLine(state);
            } while ((state != "won") && (state != "over"));

            Console.Clear();
            Console.Write(WebHandler.Instance.GetMazeVisual(maze.mazeID));
            Console.WriteLine(state);
            WriteLogFile();
        }

        protected virtual bool HasBeenEvaluatedDomokunWall(int neighbour, List<Node> closedSet)
        {
            if(neighbour == maze.domokunPos) {
                return true;
            }
            return base.HasBeenEvaluated(neighbour, closedSet);
        }
    }
}
