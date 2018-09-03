using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class Coor
{
    public int x;
    public int y;
    public List<int> neighbours;
    public int id;
    public Coor(int x, int y, int id)
    {
        this.x = x;
        this.y = y;
        this.id = id;
        neighbours = new List<int>();
    }

    public void AddNeighbour(int n)
    {
        foreach (int i in neighbours) {
            if (i == n) {
                return;//no need to add it again
            }
        }
        neighbours.Add(n);
    }

    public void RemoveNeighbour(int n)
    {
        foreach (int i in neighbours) {
            if (i == n) {
                neighbours.Remove(n);
                break;
            }
        }
    }
}

