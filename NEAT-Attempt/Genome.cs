﻿using System;
using System.Collections.Generic;
using System.Text;

namespace NEAT_Attempt
{
    class Genome
    {
        List<ConnectionGene> connections;
        List<NodeGene> nodes;
        int fitness = 0;
        int Innovation_Number = 0;

        public Genome()
        {
            connections = new List<ConnectionGene>();
            nodes = new List<NodeGene>();
        }

        public List<ConnectionGene> Connections { get { return connections; } }
        public List<NodeGene> Nodes { get { return nodes; } }

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

        public int Fitness { get { return fitness; } set { fitness = value; } }

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
            //connections.Add(pConnection);
        }

        public void ConnectionMutation(Random r)
        {
            NodeGene n1 = nodes[r.Next(0, nodes.Count)];
            NodeGene n2 = nodes[r.Next(0, nodes.Count)];
            bool reversedConnection = false;
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
                ConnectionMutation(r);
            else
            {
                float weight = (float)r.NextDouble() * 2.0f - 1.0f;
                Innovation_Number++;
                connections.Add(new ConnectionGene(n1.ID, n2.ID, weight, true, Innovation_Number));
            }
        }

        public void NodeMutation(Random r)
        {
            ConnectionGene con = connections[r.Next(0, connections.Count)];
            NodeGene n1 = nodes[con.InNode];
            NodeGene n2 = nodes[con.OutNode];
            con.IsActivated = false;
            NodeGene temp = new NodeGene(NodeGene.TYPE.HIDDEN, connections.Count);
            Innovation_Number++;
            ConnectionGene inToNew = new ConnectionGene(n1.ID, temp.ID, 1.0f, true, Innovation_Number);
            ConnectionGene newToOut = new ConnectionGene(temp.ID, n2.ID, con.Weight, true, Innovation_Number);
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
            int largestInnovation = parent1LastInnovation > parent2LastInnovation ? parent1LastInnovation : parent2LastInnovation;
            AddNodesToChild(child, mostFit, leastFit, sameFitness, largestInnovation);
            AddConnectionGenesToChild(child, mostFit, leastFit, sameFitness, largestInnovation);
            return child;
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
                    ConnectionGene c = mostFit.Connections[i];
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