using System;
using System.Collections.Generic;
using System.Text;

namespace NEAT_Attempt
{
    class Genome
    {
        List<ConnectionGene> connections;
        List<NodeGene> nodes;
        float fitness = 0.0f;
        const double PROBABILITY_PERTURBING = 0.8;
        const float C1 = 1.0f;
        const float C2 = 1.0f;
        const float C3 = 1.0f;

        public Genome()
        {
            connections = new List<ConnectionGene>();
            nodes = new List<NodeGene>();
        }

        public List<ConnectionGene> Connections { get { return connections; } }
        public List<NodeGene> Nodes { get { return nodes; } }
        public float Fitness { get { return fitness; } set { fitness = value; } }

        public void AddNode(NodeGene pNode)
        {
            try
            {
                nodes.Insert(pNode.ID, pNode);
            }
            catch
            {
                nodes.Add(null);
                AddNode(pNode);
            }
        }

        public void AddConnection(ConnectionGene pConnection)
        { //Could insert by inno number here, try that out after tutorial, might auto-line up for crossover
            try
            {
                connections.Insert(pConnection.InnovationNumber, pConnection);
            }
            catch
            {
                connections.Add(null);
                AddConnection(pConnection);
            }
        }

        public void MutateWeights(Random r)
        {
            for (int i = 0; i < Connections.Count; i++)
            {
                if (r.NextDouble() < PROBABILITY_PERTURBING)
                {
                    Connections[i].Weight = Connections[i].Weight * (float)(r.NextDouble() * 4.0f - 2.0f);
                }
                else
                {
                    Connections[i].Weight = (float)r.NextDouble() * 4.0f-2.0f;
                }
            }
        }

        public void ConnectionMutation(Random r, InnovationGen innovator)
        {
            NodeGene n1 = nodes[r.Next(0, nodes.Count)];
            NodeGene n2 = nodes[r.Next(0, nodes.Count)];
            bool reversedConnection = false;
            if (n1.Type == NodeGene.TYPE.INPUT && n2.Type == NodeGene.TYPE.INPUT || n1.ID == n2.ID)
            {
                //Should not have Inputs connecting to Inputs or same nodes connecting to same nodes
                return;
            }
            if (n1.Type == NodeGene.TYPE.HIDDEN && n2.Type == NodeGene.TYPE.INPUT) reversedConnection = true;
            else if (n1.Type == NodeGene.TYPE.OUTPUT && n2.Type == NodeGene.TYPE.HIDDEN) reversedConnection = true;
            else if (n1.Type == NodeGene.TYPE.OUTPUT && n2.Type == NodeGene.TYPE.INPUT) reversedConnection = true;

            bool connectionExists = false;
            foreach (ConnectionGene gene in connections)
            {
                if (gene.InNode == n1.ID && gene.OutNode == n2.ID)
                {
                    connectionExists = true;
                    break;
                }
                else if (gene.InNode == n2.ID && gene.OutNode == n1.ID)
                {
                    connectionExists = true;
                    break;
                }
            }
            NodeGene temp;
            if (reversedConnection)
            {
                temp = n1;
                n1 = n2;
                n2 = temp;
            }

            if (connectionExists)
                ConnectionMutation(r, innovator);
            else
            {
                float weight = (float)r.NextDouble() * 2.0f - 1.0f;
                connections.Add(new ConnectionGene(n1.ID, n2.ID, weight, true, innovator.Innovation));
            }
        }

        public void NodeMutation(Random r, InnovationGen innovator)
        {
            ConnectionGene con = connections[r.Next(0, connections.Count)];
            NodeGene n1 = nodes[con.InNode];
            NodeGene n2 = nodes[con.OutNode];
            con.IsActivated = false;
            NodeGene temp = new NodeGene(NodeGene.TYPE.HIDDEN, nodes.Count);
            ConnectionGene inToNew = new ConnectionGene(n1.ID, temp.ID, 1.0f, true, innovator.Innovation);
            ConnectionGene newToOut = new ConnectionGene(temp.ID, n2.ID, con.Weight, true, innovator.Innovation);
            nodes.Add(temp);
            connections.Add(inToNew);
            connections.Add(newToOut);
        }

        public static Genome Crossover(Genome parent1, Genome parent2)
        {
            Genome child = new Genome();
            Genome mostFit = parent1.Fitness > parent2.Fitness ? parent1 : parent2;
            Genome leastFit = parent1.Fitness > parent2.Fitness ? parent2 : parent1;
            bool sameFitness = parent1.Fitness == parent2.Fitness; //Need to know whether same fitness to discard/allow disjoint/excess genes
            int parent1LastInnovation = parent1.Connections[parent1.Connections.Count - 1].InnovationNumber;
            int parent2LastInnovation = parent2.Connections[parent2.Connections.Count - 1].InnovationNumber;
            int largestInnovation;
            if (parent1.Fitness > parent2.Fitness) largestInnovation = parent1LastInnovation;
            else if (parent2.Fitness > parent1.Fitness) largestInnovation = parent2LastInnovation;
            else { largestInnovation = parent1LastInnovation > parent2LastInnovation ? parent1LastInnovation : parent2LastInnovation; }
            AddNodesToChild(child, mostFit, leastFit, sameFitness, largestInnovation);
            AddConnectionGenesToChild(child, mostFit, leastFit, sameFitness, largestInnovation);
            return child;
        }

        public static float CompareGenes(Genome parent1, Genome parent2)
        {
            int matchingGenes = 0;
            int disjointGenes = 0;
            int excessGenes = 0;
            float weightOfMatchingGenes = 0;
            List<ConnectionGene> p1 = parent1.Connections;
            List<ConnectionGene> p2 = parent2.Connections;
            int highestInnovation = p1.Count > p2.Count ? p1.Count - 1 : p2.Count - 1;
            for (int i = 0; i < highestInnovation; i++)
            {
                try { bool t = p1[i] == null; }
                catch { for (int j = i; j < p2.Count; j++) { excessGenes++; } break; }
                try { bool t = p2[i] == null; }
                catch { for (int j = i; j < p1.Count; j++) { excessGenes++; } break; }
                if (p1[i] == null && p2[i] == null) continue;
                if (p1[i] != null && p2[i] == null)
                {
                    if (NullList(p2, i))
                    {
                        excessGenes++;
                        continue;
                    }
                    disjointGenes++;
                    continue;
                }
                if (p1[i] == null && p2[i] != null)
                {
                    if (NullList(p1, i))
                    {
                        excessGenes++;
                        continue;
                    }
                    disjointGenes++;
                    continue;
                }
                if (p1[i].InnovationNumber == p2[i].InnovationNumber)
                {
                    weightOfMatchingGenes += (p1[i].Weight + p2[i].Weight) / 2;
                    matchingGenes++;
                    continue;
                }
            }
            //Console.WriteLine("Matching Genes: " + matchingGenes);
            //Console.WriteLine("Avg Weight Diff: " + weightOfMatchingGenes / matchingGenes);
            //Console.WriteLine("Excess Genes: " + excessGenes);
            //Console.WriteLine("Disjoint Genes: " + disjointGenes);
            return C1*excessGenes + C2*disjointGenes + C3*(weightOfMatchingGenes / matchingGenes);
        }

        private static bool NullList(List<ConnectionGene> p1, int i)
        {
            for (int j = i; j < p1.Count; j++)
            {
                if (p1[j] != null)
                    return false;
            }
            return true;
        }

        private static void AddNodesToChild(Genome child, Genome mostFit, Genome leastFit, bool sameFitness, int largestInnovation)
        {
            if (sameFitness)
            {
                foreach (NodeGene n in mostFit.Nodes)
                {
                    if (n == null) continue;
                    child.AddNode(n.Copy());
                }
                foreach (NodeGene n in leastFit.Nodes)
                {
                    if (n == null) continue;
                    if (!child.Nodes.Contains(n))
                        child.AddNode(n.Copy());
                }
                return;
            }

            foreach (NodeGene n in mostFit.Nodes)
            {
                child.AddNode(n.Copy());
            }
        }

        private static void AddConnectionGenesToChild(Genome child, Genome mostFit, Genome leastFit, bool sameFitness, int largestInnovation)
        {
            for (int i = 0; i <= largestInnovation; i++)
            {
                if (mostFit.Connections[i] != null && !sameFitness)
                {
                    ConnectionGene c = mostFit.Connections[i]; //If only fit parent has this gene, inherit it, else,
                    if (leastFit.Connections[i] != null) //We need to check if parent2 has this gene, if so
                    {
                        Random rnd = new Random();
                        int rand = rnd.Next(0, 2); //inherit matching gene randomly
                        if (rand == 1)
                        {
                            c = leastFit.Connections[i];
                        }
                    }
                    child.AddConnection(new ConnectionGene(c.InNode, c.OutNode, c.Weight, c.IsActivated, c.InnovationNumber));
                    continue;
                }
                try
                {
                    if (sameFitness && mostFit.Connections[i] != null && leastFit.Connections[i] != null)
                    {
                        ConnectionGene c1 = mostFit.Connections[i];
                        ConnectionGene c2 = leastFit.Connections[i];

                        int testGene = c1.IsSame(c2);
                        if (testGene == 2)
                        {
                            ConnectionGene c;
                            Random rnd = new Random();
                            int rand = rnd.Next(0, 2);
                            if (rand == 0) c = c1;
                            else c = c2;
                            child.AddConnection(new ConnectionGene(c.InNode, c.OutNode, c.Weight, c.IsActivated, c.InnovationNumber));
                        }
                        else if (testGene == 0)
                        {
                            child.AddConnection(new ConnectionGene(c1.InNode, c1.OutNode, c1.Weight, c1.IsActivated, c1.InnovationNumber));
                        }
                        else if (testGene == 1)
                        {
                            bool active = false;
                            Random rnd = new Random();
                            int rand = rnd.Next(0, 2);
                            if (rand == 0)
                                active = true;
                            child.AddConnection(new ConnectionGene(c1.InNode, c1.OutNode, c1.Weight, active, c1.InnovationNumber));
                        }
                        continue;
                    }
                }
                catch (ArgumentOutOfRangeException e)
                {
                    if (i >= mostFit.Connections.Count) { child.AddConnection(leastFit.Connections[i]); }
                    else { child.AddConnection(mostFit.Connections[i]); }
                    continue;
                }

                if (mostFit.Connections[i] == null && leastFit.Connections[i] != null && sameFitness)
                {
                    ConnectionGene c = leastFit.Connections[i];
                    child.AddConnection(new ConnectionGene(c.InNode, c.OutNode, c.Weight, c.IsActivated, c.InnovationNumber));
                    continue;
                }
                if (leastFit.Connections[i] == null && mostFit.Connections[i] != null && sameFitness)
                {
                    ConnectionGene c = mostFit.Connections[i];
                    child.AddConnection(new ConnectionGene(c.InNode, c.OutNode, c.Weight, c.IsActivated, c.InnovationNumber));
                }
            }
        }
    }
}
