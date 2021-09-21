using System;

namespace NEAT_Attempt
{
    class Program
    {
        static void Main(string[] args)
        {
            Genome p1 = new Genome();
            for (int i = 0; i < 3; i++)
            {
                NodeGene gene = new NodeGene(NodeGene.TYPE.INPUT, i);
                p1.AddNode(gene);
            }
            p1.AddNode(new NodeGene(NodeGene.TYPE.OUTPUT, 3));
            p1.AddNode(new NodeGene(NodeGene.TYPE.HIDDEN, 4));

            p1.AddConnection(new ConnectionGene(1,4,1.0f,true,0));
            p1.AddConnection(new ConnectionGene(2,4,1.0f,false,1));
            p1.AddConnection(new ConnectionGene(3,4,1.0f,true,2));
            p1.AddConnection(new ConnectionGene(2,5,1.0f,true,3));
            p1.AddConnection(new ConnectionGene(5,4,1.0f,true,4));
            p1.AddConnection(new ConnectionGene(1,5,1.0f, true,7));

            Genome p2 = new Genome();
            for (int i = 0; i < 3; i++)
            {
                NodeGene gene = new NodeGene(NodeGene.TYPE.INPUT, i + 1);
                p2.AddNode(gene);
            }
            p2.AddNode(new NodeGene(NodeGene.TYPE.OUTPUT, 3));
            p2.AddNode(new NodeGene(NodeGene.TYPE.HIDDEN, 4));
            p2.AddNode(new NodeGene(NodeGene.TYPE.HIDDEN, 5));

            p2.AddConnection(new ConnectionGene(1,4,1.0f, true,0));
            p2.AddConnection(new ConnectionGene(2,4,1.0f,false,1));
            p2.AddConnection(new ConnectionGene(3,4,1.0f, true,2));
            p2.AddConnection(new ConnectionGene(2,5,1.0f,true,3));
            p2.AddConnection(new ConnectionGene(5,4,1.0f, false, 4));
            p2.AddConnection(new ConnectionGene(5,6,1.0f,true,5));
            p2.AddConnection(new ConnectionGene(6,4,1.0f,true,6));
            p2.AddConnection(new ConnectionGene(3,5,1.0f,true,8));
            p2.AddConnection(new ConnectionGene(1,6,1.0f,true,9));

            print(p1);
            print(p2);

            Genome child = Genome.Crossover(p1, p2);
            print(child);
        }

        static void print(Genome p)
        {
            string outputInno = "";
            string output = "";
            string outputDIS = "";
            string space = " ";
            foreach (ConnectionGene g in p.Connections)
            {
                if (g == null)
                {
                    outputInno += ($"{space,6}");
                    output += ($"{space,6}");
                    outputDIS += ($"{space,6}");
                    continue;
                }
                outputInno += ($"{g.InnovationNumber,6}");
                string connectionStr = g.InNode + "->" + g.OutNode;
                output += ($"{connectionStr,6}");
                if (!g.IsActivated)
                {
                    outputDIS += ($"{"DIS",6}");
                }
                else
                    outputDIS += ($"{" ",6}");
            }
            outputInno += '\r';
            output += "\r\n";
            Console.WriteLine(outputInno);
            Console.WriteLine(output);
            Console.WriteLine(outputDIS);
            Console.WriteLine();
        }
    }
}
