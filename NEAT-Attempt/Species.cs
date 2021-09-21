using System;
using System.Collections.Generic;
using System.Text;

namespace NEAT_Attempt
{
    class Species
    {
        public Genome mascot;
        public List<Genome> members;
        public float totalAdjustedFitness = 0.0f;
        
        public Species(Genome pMascot)
        {
            mascot = pMascot;
            members = new List<Genome>();
            members.Add(mascot);
        }

        public void AddAdjustedFitness(float pFitness)
        {
            totalAdjustedFitness += pFitness;
        }

        public void Reset(Random r)
        {
            int newMascot = r.Next(0, members.Count);
            mascot = members[newMascot];
            members.Clear();
            totalAdjustedFitness = 0.0f;
        }
    }
}
