using System;
using System.Collections.Generic;
using System.Text;

namespace NEAT_Attempt
{
    class InnovationGen
    {
        private int innovationNum = 0;

        public int Innovation { get { return innovationNum++; } }
    }
}
