using System;
using System.Collections.Generic;
using System.Text;

namespace NEAT_Attempt
{
    class ConnectionGene
    {
        int inNode;
        int outNode;
        float weight;
        bool isActivated;
        int innovationNum;

        public ConnectionGene(int pInNode, int pOutNode, float pWeight, bool pIsActivated, int pInnoNum)
        {
            inNode = pInNode;
            outNode = pOutNode;
            weight = pWeight;
            isActivated = pIsActivated;
            innovationNum = pInnoNum;
        }

        public int IsSame(ConnectionGene pTrialGene)
        {
            bool sameIn = inNode == pTrialGene.InNode;
            bool sameOut = outNode == pTrialGene.OutNode;
            bool sameInno = innovationNum == pTrialGene.InnovationNumber;
            bool sameActivation = isActivated == pTrialGene.IsActivated;
            if (sameIn && sameOut && sameInno)
            {
                if (sameActivation)
                    return 0; //Means same activation on both parents
                else
                    return 1; // Means roll 50/50 on whether this should be activated in child
            }
            return 2; //Means not similar connection
        }

        public int InNode { get { return inNode; } set { inNode = value; } }
        public int OutNode { get { return outNode; } set { inNode = value; } }
        public float Weight { get { return weight; } set { weight = value; } }
        public bool IsActivated { get { return isActivated; } set { isActivated = value; } }
        public int InnovationNumber { get { return innovationNum; } set { innovationNum = value; } }
    }
}
