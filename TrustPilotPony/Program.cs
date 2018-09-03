using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.IO;

namespace TrustPilotPony
{
    class Program
    {
        static int Main(string[] args)
        {
            
            string str = WebHandler.Instance.GetMazeId("fluttershy", 25, 25, 3);
            //Console.WriteLine(str);
            //Console.Write(WebHandler.Instance.GetMazeVisual(str));
            string mazeJson = WebHandler.Instance.GetMaze(str);
            if(mazeJson == "error") { return -1; }
            Maze m = new Maze(mazeJson, str);
            AdjustingPony pony = new AdjustingPony(m);
            pony.ExecutePlan();
            //SimplePony pony = new SimplePony(m);
            //pony.ExecutePlan();
            Console.ReadKey();
            return 1;
        }
    }
}
