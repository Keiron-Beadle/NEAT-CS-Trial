using System;
using System.Collections.Generic;
using System.Text;

namespace NEAT_Attempt
{
    class BasicEvaluator : Evaluator
    {

        public BasicEvaluator(int pPopulationSize, Genome pStartingGenome, InnovationGen pInnovator) 
            : base(pPopulationSize, pStartingGenome, pInnovator) 
        { }

        protected override float EvaluateGenome(Genome genome)
        {
            return genome.Connections.Count;
        }
    }
}
