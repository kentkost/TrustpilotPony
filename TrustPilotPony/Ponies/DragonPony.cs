using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace TrustPilotPony
{
    //Inspired by South Park episode "Guitar Queer-o". This pony wants the domokun to try and catch it. BUt it never catches it.
    //The pony will first find the cycles in the maze.  Then go as close to the domokun as possible afterwards it will go to a cycle. hopefully the domokun will follow.
    //If the goal path is blocked by the domokun it will start going to one of the cycles before going to the goal.
    class DragonPony : AdjustingPony
    {
        protected Dictionary<int, List<int>> cycles = new Dictionary<int, List<int>>();
        protected Dictionary<int, int> pointToCycle = new Dictionary<int, int>();

        public DragonPony(Maze m) : base(m)
        {
            name = "DragonPony";
        }

        public override void ExecutePlan()
        {
            FindAllCycles();

            //Move towards the goal.
            //if the pony and the domokun are length of two from eachother. Then go to a point with a cycle + take the cycle + the path from current position to goal.
            //It just assumes that the domokun will follow irregardless of how far it gets away from the endpoint. It wont check to see if it actually will.
            string state = "";
            int i =0;
            do {
                int distToD = DistanceToDomokun(maze.ponyPos, maze.domokunPos);
                if(distToD <= 2) {
                    //point with cycle

                }
                List<int> path = AStar(maze.ponyPos, maze.endPoint);
                string move = Directions(path).First();
                state = WebHandler.Instance.MakeMove(maze.mazeID, move);
                AppendToLog(ref i, state, move);
                JObject jobj = JObject.Parse(state);
                state = (string)jobj["state"];
                maze.UpdateInfo();


                Console.Write(WebHandler.Instance.GetMazeVisual(maze.mazeID));
                System.Threading.Thread.Sleep(200);
                Console.Clear();
                Console.WriteLine(state);
            } while ((state != "won") != (state != "over"));
            
        }

        //does not take into account it might go into the domokun
        protected List<int> PathToClosestCycle(int p)
        {
            HasBeenEvaluatedHan = HasBeenEvaluatedDomokunWall;
            int dist = int.MaxValue;
            int tempDist;
            List<int> path = new List<int>();
            for (int i=0; i< numOfPoints; i++) {
                if(pointToCycle.ContainsKey(i) && i != maze.domokunPos) {
                    tempDist = AStar(maze.ponyPos, i).Count;
                    if(dist > tempDist) {
                        path = AStar(maze.ponyPos, i);
                    }
                }
            }
            return path;
        }

        protected int DistanceToDomokun(int p1, int p2)
        {
            return AStar(p1, p2).Count;
        }

        protected List<int> ReArrangeCycle(int cycleStart)
        {
            return new List<int>();
        }

        protected void FindAllCycles()
        {
            int cycleId = 0;
            List<int> openset = new List<int>();
            for(int i=0; i<maze.width*maze.height; i++) {
                 openset.Add(i);
            }

            while(openset.Count > 0) {
                int cur = openset.First();
                List<int> cycleTemp = Cycle(cur);

                //cycle found
                if (cycleTemp.Count > 0) {
                    //Clean the openset and add the Cycle
                    cycles.Add(cycleId, cycleTemp);
                    CleanOpenSet(ref openset, cycleTemp, cycleId);
                    cycleId++;
                }
            }
        }

        protected void CleanOpenSet(ref List<int> oSet, List<int> cycle, int cycleId)
        {
            foreach(int i in cycle) {
                if (oSet.Contains(i)) {
                    oSet.Remove(i);
                    pointToCycle.Add(i, cycleId); //This point it part of this cycle.
                }
            }
        }

        protected List<int> Cycle(int point)
        {
            List<int> neighbours = maze.coordinates[point].neighbours;
            List<int> cycle;
            
            foreach(int n  in neighbours) {
                maze.coordinates[n].RemoveNeighbour(point);
                cycle = AStar(n, point);
                maze.coordinates[n].AddNeighbour(point);

                if (cycle.Count > 0) {
                    return cycle; //We only need one cycle per point.
                }
            }
            return new List<int>();
        }

        protected int InACycle(int point)
        {
            foreach(KeyValuePair<int, List<int>> cycle in cycles) {
                if (cycle.Value.Contains(point)) {
                    return cycle.Key;
                }
            }
            return -1;
        }

    }
}
