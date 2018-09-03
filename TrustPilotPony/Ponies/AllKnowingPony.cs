using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TrustPilotPony
{
    //Notes: If no path then go in direction of domokun until it it as close as two to threee lengths units from it. then stay.
    //Take shortest path.

    class AllKnowingPony : DragonPony
    {
        //escape plan is if the domokun gets too close.
        protected List<int> escapePlan = new List<int>();
        //If in this state then next state will be.
        protected Dictionary<State, State> exploredStates = new Dictionary<State, State>();
        protected Dictionary<int, State> predictedStatesDomukun = new Dictionary<int, State>();
        protected Dictionary<int, State> predictedStatesPony = new Dictionary<int, State>();
        protected Dictionary<string, string> oneStepWorseCase = new Dictionary<string, string>();
        public AllKnowingPony(Maze m) : base(m)
        {
            oneStepWorseCase.Add("west", "over");
            name = "AllKnowingPony";
        }
    }

    public class State
    {
        int pony;
        int domokun;

        int PonyPosition
        {
            get { return pony; }
            set { pony = value; }
        }

        int DomukunPosition
        {
            get { return domokun; }
            set { pony = value; }
        }
    }
}
