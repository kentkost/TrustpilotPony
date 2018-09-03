using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TrustPilotPony.Code
{
    class CheatingPony : AdjustingPony
    {
        public CheatingPony(Maze m) : base(m)
        {
            name = "CheatingPony";
        }
    }
}
