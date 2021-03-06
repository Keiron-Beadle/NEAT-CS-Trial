using System;

namespace NEAT_Attempt
{
    class Program
    {
        static void Main(string[] args)
        {

        }

        private static void TestForLowConnections()
        {
            InnovationGen innovator = new InnovationGen();
            Genome start = new Genome();
            start.AddNode(new NodeGene(NodeGene.TYPE.INPUT, 0));
            start.AddNode(new NodeGene(NodeGene.TYPE.INPUT, 1));
            start.AddNode(new NodeGene(NodeGene.TYPE.OUTPUT, 2));
            start.AddConnection(0, 2, 0.5f, true, innovator);
            start.AddConnection(1, 2, 0.5f, true, innovator);
            BasicEvaluator eval = new BasicEvaluator(150, start, innovator);
            for (int i = 0; i < 100; i++)
            {
                eval.Evaluate();
                Console.Write("Generation: " + i + ' ');
                Console.Write("Connections in Highest: " + eval.FittestGenome + ' ');
                Console.WriteLine("Number of Species: " + eval.NumberOfSpecies);
                // if (i % 10 == 0)
                // {
                //     Console.WriteLine();
                //print(eval.FittestGenome);
                //      Console.WriteLine();
                // }
            }
        }

        private static void TestForHighConnections()
        {
            InnovationGen innovator = new InnovationGen();
            Genome start = new Genome();
            start.AddNode(new NodeGene(NodeGene.TYPE.INPUT, 0));
            start.AddNode(new NodeGene(NodeGene.TYPE.INPUT, 1));
            start.AddNode(new NodeGene(NodeGene.TYPE.OUTPUT, 2));
            start.AddConnection(0,2,0.5f,true, innovator);
            start.AddConnection(1,2,0.5f,true,innovator);
            BasicEvaluator eval = new BasicEvaluator(150, start, innovator);
            for (int i = 0; i < 100; i++)
            {
                eval.Evaluate();
                Console.Write("Generation: " + i + ' ');
                Console.Write("Highest fitness: " + eval.HighestFitness + ' ');
                Console.WriteLine("Number of Species: " + eval.NumberOfSpecies);
                // if (i % 10 == 0)
                // {
                //     Console.WriteLine();
                //print(eval.FittestGenome);
                //      Console.WriteLine();
                // }
            }
        }

        //private static void TestCompareGenes()
        //{
        //    Genome p1 = new Genome();
        //    for (int i = 0; i < 3; i++)
        //    {
        //        NodeGene gene = new NodeGene(NodeGene.TYPE.INPUT, i);
        //        p1.AddNode(gene);
        //    }
        //    p1.Fitness = 10;
        //    p1.AddNode(new NodeGene(NodeGene.TYPE.OUTPUT, 3));
        //    p1.AddNode(new NodeGene(NodeGene.TYPE.HIDDEN, 4));

        //    p1.AddConnection(new ConnectionGene(1, 4, 1.0f, true, 0));
        //    p1.AddConnection(new ConnectionGene(2, 4, 1.0f, false, 1));
        //    p1.AddConnection(new ConnectionGene(3, 4, 1.0f, true, 2));
        //    p1.AddConnection(new ConnectionGene(2, 5, 1.0f, true, 3));
        //    p1.AddConnection(new ConnectionGene(5, 4, 1.0f, true, 4));
        //    p1.AddConnection(new ConnectionGene(1, 5, 1.0f, true, 7));

        //    Genome p2 = new Genome();
        //    for (int i = 0; i < 3; i++)
        //    {
        //        NodeGene gene = new NodeGene(NodeGene.TYPE.INPUT, i + 1);
        //        p2.AddNode(gene);
        //    }
        //    p2.AddNode(new NodeGene(NodeGene.TYPE.OUTPUT, 3));
        //    p2.AddNode(new NodeGene(NodeGene.TYPE.HIDDEN, 4));
        //    p2.AddNode(new NodeGene(NodeGene.TYPE.HIDDEN, 5));

        //    p2.AddConnection(new ConnectionGene(1, 4, 1.0f, true, 0));
        //    p2.AddConnection(new ConnectionGene(2, 4, 1.0f, false, 1));
        //    p2.AddConnection(new ConnectionGene(3, 4, 1.0f, true, 2));
        //    p2.AddConnection(new ConnectionGene(2, 5, 1.0f, true, 3));
        //    p2.AddConnection(new ConnectionGene(5, 4, 1.0f, true, 4));
        //    p2.AddConnection(new ConnectionGene(5, 6, 1.0f, true, 5));
        //    p2.AddConnection(new ConnectionGene(6, 4, 1.0f, true, 6));
        //    p2.AddConnection(new ConnectionGene(3, 5, 1.0f, true, 8));
        //    p2.AddConnection(new ConnectionGene(1, 6, 1.0f, true, 9));

        //    print(p1);
        //    print(p2);

        //    Console.WriteLine("Difference: " + Genome.CompareGenes(p1, p2));
        //}

        //private static void TestConnectionMutation()
        //{
        //    InnovationGen innovator = new InnovationGen();
        //    Random r = new Random();
        //    Genome genome = new Genome();
        //    NodeGene input1 = new NodeGene(NodeGene.TYPE.INPUT, 0);
        //    NodeGene input2 = new NodeGene(NodeGene.TYPE.INPUT, 1);
        //    NodeGene output1 = new NodeGene(NodeGene.TYPE.OUTPUT, 2);

        //    genome.AddNode(input1);
        //    genome.AddNode(input2);
        //    genome.AddNode(output1);

        //    genome.AddConnection(new ConnectionGene(0, 2, 0.5f, true, innovator.Innovation));
        //    genome.AddConnection(new ConnectionGene(1, 2, 1.0f, true, innovator.Innovation));

        //    print(genome);
        //    genome.NodeMutation(r, innovator);
        //    genome.ConnectionMutation(r, innovator,10);
        //    print(genome);
        //}

        //static void TestCrossover()
        //{
        //    Genome p1 = new Genome();
        //    Random rnd = new Random();
        //    for (int i = 0; i < 3; i++)
        //    {
        //        NodeGene gene = new NodeGene(NodeGene.TYPE.INPUT, i);
        //        p1.AddNode(gene);
        //    }
        //    p1.Fitness = 10;
        //    p1.AddNode(new NodeGene(NodeGene.TYPE.OUTPUT, 3));
        //    p1.AddNode(new NodeGene(NodeGene.TYPE.HIDDEN, 4));

        //    p1.AddConnection(new ConnectionGene(1, 4, 1.0f, true, 0));
        //    p1.AddConnection(new ConnectionGene(2, 4, 1.0f, false, 1));
        //    p1.AddConnection(new ConnectionGene(3, 4, 1.0f, true, 2));
        //    p1.AddConnection(new ConnectionGene(2, 5, 1.0f, true, 3));
        //    p1.AddConnection(new ConnectionGene(5, 4, 1.0f, true, 4));
        //    p1.AddConnection(new ConnectionGene(1, 5, 1.0f, true, 7));

        //    Genome p2 = new Genome();
        //    for (int i = 0; i < 3; i++)
        //    {
        //        NodeGene gene = new NodeGene(NodeGene.TYPE.INPUT, i + 1);
        //        p2.AddNode(gene);
        //    }
        //    p2.AddNode(new NodeGene(NodeGene.TYPE.OUTPUT, 3));
        //    p2.AddNode(new NodeGene(NodeGene.TYPE.HIDDEN, 4));
        //    p2.AddNode(new NodeGene(NodeGene.TYPE.HIDDEN, 5));

        //    p2.AddConnection(new ConnectionGene(1, 4, 1.0f, true, 0));
        //    p2.AddConnection(new ConnectionGene(2, 4, 1.0f, false, 1));
        //    p2.AddConnection(new ConnectionGene(3, 4, 1.0f, true, 2));
        //    p2.AddConnection(new ConnectionGene(2, 5, 1.0f, true, 3));
        //    p2.AddConnection(new ConnectionGene(5, 4, 1.0f, false, 4));
        //    p2.AddConnection(new ConnectionGene(5, 6, 1.0f, true, 5));
        //    p2.AddConnection(new ConnectionGene(6, 4, 1.0f, true, 6));
        //    p2.AddConnection(new ConnectionGene(3, 5, 1.0f, true, 8));
        //    p2.AddConnection(new ConnectionGene(1, 6, 1.0f, true, 9));

        //    print(p1);
        //    print(p2);

        //    Genome child = Genome.Crossover(p1, p2, rnd);
        //    print(child);
        //}

        static void print(Genome p)
        {
            string outputInno = "";
            string output = "";
            string outputDIS = "";
            string space = " ";
            int counter = 0;
            foreach (ConnectionGene g in p.Connections)
            {
                if (g.InnovationNumber > counter)
                {
                    while (counter < g.InnovationNumber - 1)
                    {
                        outputInno += ($"{space,6}");
                        output += ($"{space,6}");
                        outputDIS += ($"{space,6}");
                        counter++;
                    }
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
                counter++;
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
