using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NEAT_Attempt
{
    class Genome
    {
        static List<ConnectionGene> allConnections = new List<ConnectionGene>();
        List<ConnectionGene> connections;
        List<NodeGene> nodes;
        float fitness = 0.0f;
        const double PROBABILITY_PERTURBING = 0.8;
        const float C1 = 1.0f;
        const float C2 = 1.0f;
        const float C3 = 0.4f;

        public Genome()
        {
            connections = new List<ConnectionGene>();
            nodes = new List<NodeGene>();
        }

        public Genome(Genome copy)
        {
            connections = new List<ConnectionGene>();
            nodes = new List<NodeGene>();
            for (int i = 0; i < copy.connections.Count; i++)
            {
                ConnectionGene connection = new ConnectionGene(copy.connections[i].InNode,
                                                               copy.connections[i].OutNode,
                                                               copy.connections[i].Weight,
                                                               copy.connections[i].IsActivated,
                                                               copy.connections[i].InnovationNumber);
                connections.Add(connection);
            }

            for (int i = 0; i < copy.nodes.Count; i++)
            {
                NodeGene node = new NodeGene(copy.nodes[i].Type, copy.nodes[i].ID);
                nodes.Add(node);
            }
        }

        public List<ConnectionGene> Connections { get { return connections; } }
        public List<NodeGene> Nodes { get { return nodes; } }
        public float Fitness { get { return fitness; } set { fitness = value; } }

        public void AddNode(NodeGene pNode)
        {
            nodes.Add(pNode);
        }

        public void AddConnection(int p1ID, int p2ID, float pWeight, bool pEnabled, InnovationGen pInnovator)
        {
            var con = ConnectionInPool(p1ID, p2ID);
            if (con.Item1)
            {
                connections.Add(new ConnectionGene(p1ID, p2ID, pWeight, pEnabled, con.Item2));
            }
            else
            {
                var newCon = new ConnectionGene(p1ID, p2ID, pWeight, pEnabled, pInnovator.Innovation);
                allConnections.Add(newCon);
                connections.Add(newCon);
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

        public void ConnectionMutation(Random r, InnovationGen innovator, int attempts)
        {
            for (int i = 0; i < attempts; i++)
            {
                NodeGene n1 = nodes[r.Next(0, nodes.Count)];
                NodeGene n2 = nodes[r.Next(0, nodes.Count)];
                bool reversedConnection = false;
                if (n1.Type == NodeGene.TYPE.INPUT && n2.Type == NodeGene.TYPE.INPUT || n1.ID == n2.ID)
                {
                    //Should not have Inputs connecting to Inputs or same nodes connecting to same nodes
                    continue;
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
                    continue;
                else
                {
                    float weight = (float)r.NextDouble() * 2.0f - 1.0f;
                    AddConnection(n1.ID, n2.ID, weight, true, innovator);                
                }
            }
        
        }

        public void NodeMutation(Random r, InnovationGen innovator)
        {
            ConnectionGene con = connections[r.Next(0, connections.Count)];
            NodeGene n1 = nodes[con.InNode];
            NodeGene n2 = nodes[con.OutNode];
            con.IsActivated = false;
            NodeGene temp = new NodeGene(NodeGene.TYPE.HIDDEN, nodes.Count);
            AddConnection(n1.ID, temp.ID, 1.0f, true, innovator);
            AddConnection(temp.ID, n2.ID, con.Weight, true, innovator);
            AddNode(temp);
        }

        private static Tuple<bool,int> ConnectionInPool(int iD1, int iD2)
        {
            for (int i = 0; i < allConnections.Count; i++)
            {
                if (allConnections[i].InNode == iD1 && allConnections[i].OutNode == iD2)
                    return new Tuple<bool,int>(true,allConnections[i].InnovationNumber);
            }
            return new Tuple<bool,int>(false,-1);
        }

        public static Genome Crossover(Genome parent1, Genome parent2, Random rnd, InnovationGen pInnovator)
        {
            Genome child = new Genome();
            Genome mostFit = parent1.Fitness > parent2.Fitness ? parent1 : parent2;
            Genome leastFit = parent1.Fitness > parent2.Fitness ? parent2 : parent1;
            bool sameFitness = parent1.Fitness == parent2.Fitness; //Need to know whether same fitness to discard/allow disjoint/excess genes
            
            AddNodesToChild(child, mostFit, leastFit, sameFitness);
            AddConnectionGenesToChild(child, mostFit, leastFit, sameFitness, rnd, pInnovator);
            return child;
        }

        public static float CompareGenes(Genome parent1, Genome parent2)
        {
            int N = (parent1.Connections.Count + parent1.Nodes.Count) > (parent2.Connections.Count + parent2.Nodes.Count) ?
                parent1.connections.Count + parent1.Nodes.Count : parent2.Connections.Count + parent2.Nodes.Count;
            int matchingGenes = 0;
            int disjointGenes = 0;
            int excessGenes = 0;
            float weightOfMatchingGenes = 0;
            List<NodeGene> p1Nodes = parent1.Nodes.OrderByDescending(entry => entry.ID).ToList();
            List<NodeGene> p2Nodes = parent1.Nodes.OrderByDescending(entry => entry.ID).ToList();
            int highestInno1 = p1Nodes[0].ID;
            int highestInno2 = p2Nodes[0].ID;
            int highestInno = highestInno1 > highestInno2 ? highestInno1 : highestInno2;
            for (int i = 0; i < parent1.Nodes.Count; i++) //Check Node gene first
            {
                NodeGene g1 = parent1.Nodes.Find(node => node.ID == i);
                NodeGene g2 = parent2.Nodes.Find(node => node.ID == i);
                if (g1 != null && g2 != null)
                    matchingGenes++;
                else
                    disjointGenes++;
            }
            for (int i = 0; i <= highestInno; i++)
            {
                NodeGene g1 = parent1.Nodes.Find(node => node.ID == i);
                NodeGene g2 = parent2.Nodes.Find(node => node.ID == i);
                if (g1 == null && highestInno1 < i && g2 != null)
                    excessGenes++;
                else if (g2 == null & highestInno2 < i && g1 != null)
                    excessGenes++;
            }

            List<ConnectionGene> p1Connections = parent1.Connections.OrderByDescending(entry => entry.InnovationNumber).ToList();
            List<ConnectionGene> p2Connections = parent2.Connections.OrderByDescending(entry => entry.InnovationNumber).ToList();
            highestInno1 = p1Connections[0].InnovationNumber;
            highestInno2 = p2Connections[0].InnovationNumber;
            highestInno = highestInno1 > highestInno2 ? highestInno1 : highestInno2;
            for (int i = 0; i <= highestInno; i++) 
            {
                ConnectionGene c1 = parent1.Connections.Find(con => con.InnovationNumber == i);
                ConnectionGene c2 = parent2.Connections.Find(con => con.InnovationNumber == i);
                if (c1 != null && c2 != null)
                {
                    matchingGenes++;
                    weightOfMatchingGenes = Math.Abs(c1.Weight - c2.Weight);
                }
                else
                    disjointGenes++;
            }
            for (int i = 0; i <= highestInno; i++)
            {
                ConnectionGene c1 = parent1.Connections.Find(con => con.InnovationNumber == i);
                ConnectionGene c2 = parent2.Connections.Find(con => con.InnovationNumber == i);
                if (c1 == null && highestInno1 < i && c2 != null)
                    excessGenes++;
                else if (c2 == null & highestInno2 < i && c1 != null)
                    excessGenes++;
            }
            disjointGenes -= excessGenes;
            //Console.WriteLine("Matching Genes: " + matchingGenes);
            //Console.WriteLine("Avg Weight Diff: " + weightOfMatchingGenes / matchingGenes);
            //Console.WriteLine("Excess Genes: " + excessGenes);
            //Console.WriteLine("Disjoint Genes: " + disjointGenes);
            float excessCalc = (C1 * excessGenes) / N;
            float disjointCalc = (C2 * disjointGenes) / N;
            float weightOFMatchingAvg = weightOfMatchingGenes / matchingGenes;
            float weightCalc = C3 * weightOFMatchingAvg;
            float output = excessCalc + disjointCalc + weightCalc;
            return excessCalc + disjointCalc + weightCalc;
        }

        private static void AddNodesToChild(Genome child, Genome mostFit, Genome leastFit, bool sameFitness)
        {
            if (sameFitness)
            {
                foreach (NodeGene n in mostFit.Nodes)
                {
                    child.AddNode(n.Copy());
                }

                foreach (NodeGene n in leastFit.Nodes)
                {
                    bool present = false;
                    for (int i = 0; i < child.nodes.Count; i++)
                    {
                        if (child.nodes[i].ID == n.ID) 
                        { 
                            present = true; 
                            break;
                        }
                    }
                    if (!present)
                        child.AddNode(n.Copy());
                }
                return;
            }

            foreach (NodeGene n in mostFit.Nodes)
            {
                child.AddNode(n.Copy());
            }
        }

        private static void AddConnectionGenesToChild(Genome child, Genome mostFit, Genome leastFit, bool sameFitness, Random rnd, InnovationGen pInnovator)
        {
            for (int i = 0; i < mostFit.connections.Count; i++)
            {
                if (!sameFitness)
                {
                    ConnectionGene c = mostFit.Connections[i]; //If only fit parent has this gene, inherit it, else,
                    Tuple<bool, ConnectionGene> leastFitConnection = GenomeHasConnection(leastFit, c);
                    if (leastFitConnection.Item1) //We need to check if parent2 has this gene, if so
                    {
                        int rand = rnd.Next(0, 2); //inherit matching gene randomly
                        if (rand == 1)
                            c = leastFitConnection.Item2;
                    }
                    child.AddConnection(c.InNode, c.OutNode, c.Weight, c.IsActivated, pInnovator);
                    continue;
                }
                else if (sameFitness)
                {
                    ConnectionGene c1 = mostFit.Connections[i];
                    Tuple<bool, ConnectionGene> leastFitConnection = GenomeHasConnection(leastFit, c1);
                    ConnectionGene c2 = null;
                    if (leastFitConnection.Item1)
                    {
                        c2 = leastFitConnection.Item2;
                    }
                    if (c2 != null)
                    {
                        bool disabled = !c1.IsActivated || !c2.IsActivated;
                        if (rnd.Next(0, 2) == 0 && disabled) { c1.IsActivated = false; c2.IsActivated = false; }
                        if (rnd.Next(0, 2) == 0) { child.AddConnection(c1.InNode, c1.OutNode, c1.Weight, c1.IsActivated, pInnovator); }
                        else { child.AddConnection(c2.InNode, c2.OutNode, c2.Weight, c2.IsActivated, pInnovator); }
                    }
                    else
                    {
                        child.AddConnection(c1.InNode, c1.OutNode, c1.Weight, c1.IsActivated, pInnovator);
                    }

                }
            }
        }

        private static Tuple<bool, ConnectionGene> GenomeHasConnection(Genome pLeastFit, ConnectionGene c)
        {
            for (int i = 0; i < pLeastFit.Connections.Count; i++)
                if (pLeastFit.Connections[i].InnovationNumber == c.InnovationNumber) 
                    return new Tuple<bool,ConnectionGene>(true,pLeastFit.connections[i]);
            return new Tuple<bool,ConnectionGene>(false,null);
        }
    }
}
