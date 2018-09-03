using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace TrustPilotPony
{
    public class Maze
    {
        public int ponyPos; //need two pony pos for error checking
        public int endPoint;
        public int domokunPos;
        public string mazeID;
        public int difficulty;
        public int height, width;

        public Dictionary<int, Coor> coordinates = new Dictionary<int, Coor>();

        public Maze(string _json, string _mazeID)
        {
            mazeID = _mazeID;

            JObject jsonRoot = JObject.Parse(_json);

            ponyPos = (int)jsonRoot["pony"][0];
            endPoint = (int)jsonRoot["end-point"][0];
            domokunPos = (int)jsonRoot["domokun"][0];
            difficulty = (int)jsonRoot["difficulty"];

            List<int> size = JsonConvert.DeserializeObject<List<int>>(jsonRoot["size"].ToString());
            width = size[0];
            height = size[1];
            List<List<string>> data = JsonConvert.DeserializeObject<List<List<string>>>(jsonRoot["data"].ToString());

            //initialize maze
            int corID = 0;
            for (int y = 0; y < height; y++) {
                for (int x = 0; x < width; x++) {
                    coordinates.Add(corID, new Coor(x, y, corID));
                    corID++;
                }
            }

            int cell = 0;
            foreach (List<string> walls in data) {
                //add the paths for north and west
                if (cell % width != 0 && cell - 1 >= 0) {
                    //Coor westNeighbour = maze[cell-1];
                    coordinates[cell].AddNeighbour(cell - 1);
                }
                if (cell - width >= 0) {
                    coordinates[cell].AddNeighbour(cell - width);
                }
                //remove the paths that has walls
                foreach (string s in walls) {
                    if (s.ToLower() == "north" && (cell - width > 0)) {
                        coordinates[cell].RemoveNeighbour(cell - width);
                    }
                    if (s.ToLower() == "west" && (cell % width != 0 && cell - 1 >= 0)) {
                        coordinates[cell].RemoveNeighbour(cell - 1);
                    }
                }
                //Symmetric property. This way the north, south and east walls also get added
                foreach (int c in coordinates[cell].neighbours) {
                    coordinates[c].AddNeighbour(cell);
                }
                cell++;
            }
        }

        public void UpdateInfo()
        {
            string json = WebHandler.Instance.GetMaze(mazeID);
            JObject jsonRoot = JObject.Parse(json);
            ponyPos = (int)jsonRoot["pony"][0];
            endPoint = (int)jsonRoot["end-point"][0];
            domokunPos = (int)jsonRoot["domokun"][0];
        }

        public string PrintAll()
        {
            string x = "\nponyPos: " + ponyPos +
                "\nendPoint: " + endPoint +
                "\ndomokunPos: " + domokunPos +
                "\ndifficulty: " + difficulty +"\n";
            //Console.Write(x);
            return x;
        }
    }
}