using System;
using System.Collections.Generic;
using System.Text;

namespace NEAT_Attempt
{
    class NodeGene
    {
        public enum TYPE
        {
            INPUT,
            HIDDEN,
            OUTPUT
        }

        TYPE type;
        int id;

        public NodeGene(TYPE pType, int pID)
        {
            type = pType;
            id = pID;
        }

        public NodeGene Copy() { return new NodeGene(type, id); }

        public TYPE Type { get { return type; } }
        public int ID { get { return id; } }
    }
}
