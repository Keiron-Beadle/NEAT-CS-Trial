using System;
using System.Collections.Generic;
using System.Text;

namespace NEAT_Attempt
{
    abstract class Evaluator
    {
        private int populationSize;
        private List<Genome> genomes;
        private List<Genome> nextGeneration;
        private Dictionary<Genome, Species> speciesDict;
        private Dictionary<Genome, float> scoreDict;
        private List<Species> species;
        private InnovationGen connectionInnovator;

        private const float MAX_GENETIC_DIFF = 2.3f;
        private const float MUTATION_RATE = 0.8f;
        private const float ADD_CONNECTION_RATE = 0.05f;
        private const float ADD_NODE_RATE = 0.03f;
        private float highestScore = 0.0f;
        public Genome FittestGenome { get; private set; }

        public float HighestFitness { get { return highestScore; } }
        public float NumberOfSpecies { get { return species.Count; } }

        public Evaluator(int pPopSize, Genome pStartingGenome, InnovationGen pInnovator)
        {
            genomes = new List<Genome>();
            nextGeneration = new List<Genome>();
            speciesDict = new Dictionary<Genome, Species>();
            scoreDict = new Dictionary<Genome, float>();
            species = new List<Species>();
            connectionInnovator = pInnovator;
            populationSize = pPopSize;
            for (int i = 0; i < populationSize; i++)
                genomes.Add(new Genome(pStartingGenome));
        }

        public void Evaluate()
        {
            //Step 1: Assign genomes into species, then check for dormant species
            AssignGenomesToSpecies();
            CheckForDormantSpecies(); //Remove species with 0 members
            //Step 2: Run Simulation for Genomes
            RunSimulation();

            //Step 3: Take best performing from each species and add them to next generation
            NextGenFromSpecies();

            //Step 4: Make up the population size by breeding other genomes
            BreedGenes();

            genomes = nextGeneration;
            CleanupForNextGen();
        }

        private void CleanupForNextGen()
        {
            Random rnd = new Random(); //Reset species for next generation
            for (int i = 0; i < species.Count; i++) { species[i].Reset(rnd); }
            scoreDict.Clear();
            speciesDict.Clear();
            nextGeneration = new List<Genome>();
        }

        private void CheckForDormantSpecies()
        {
            for (int i = 0; i < species.Count; i++)
            {
                if (species[i].members.Count == 0)
                {
                    species.RemoveAt(i);
                    i--;
                }
            }
        }

        private void BreedGenes()
        {
            Random r = new Random();
            while (nextGeneration.Count < populationSize)
            {
                Species species = GetSpeciesByWeight(r);
                Genome parent = GetRandomGenomeByWeight(species, r);
                Genome parent2 = GetRandomGenomeByWeight(species, r);
                Genome child = Genome.Crossover(parent, parent2,r,connectionInnovator);
                if (r.NextDouble() < MUTATION_RATE)
                {
                    child.MutateWeights(r);
                   // Console.WriteLine("Mutated Weights");
                }
                if (r.NextDouble() < ADD_CONNECTION_RATE)
                {
                    child.ConnectionMutation(r, connectionInnovator, 10);
                   // Console.WriteLine("Added COnnection");
                }
                if (r.NextDouble() < ADD_NODE_RATE)
                {
                    child.NodeMutation(r, connectionInnovator);
                   // Console.WriteLine("Added node");
                }
                nextGeneration.Add(child);
            }
        }

        private Genome GetRandomGenomeByWeight(Species s, Random r)
        {
            float sumFitness = 0.0f;
            for (int i = 0; i < s.members.Count; i++)
                sumFitness += s.members[i].Fitness;
            double random = r.NextDouble() * sumFitness;
            float count = 0.0f;
            for (int i = 0; i < s.members.Count; i++)
            {
                count += s.members[i].Fitness;
                if (count > random)
                    return s.members[i];
            }
            throw new Exception("Error getting random genome by weight");
        }

        private Species GetSpeciesByWeight(Random r)
        {
            float sumFitness = 0.0f;
            for (int i = 0; i < species.Count; i++)
                sumFitness += species[i].totalAdjustedFitness;
            float random = (float)r.NextDouble() * sumFitness;
            float count = 0.0f;
            for (int i = 0; i < species.Count; i++)
            {
                count += species[i].totalAdjustedFitness;
                if (count >= random)
                    return species[i];
            }
            throw new Exception("Error getting random species by weight");
        }

        private void NextGenFromSpecies()
        {
            for (int i = 0; i < species.Count; i++)
            {
                Genome[] sorted = MergeSort.Sort(species[i].members.ToArray());
                //MergeSort.Sort(ref species[i].members);
                nextGeneration.Add(sorted[sorted.Length-1]);
            }
        }

        private void RunSimulation()
        {
            for (int i = 0; i < genomes.Count; i++)
            {
                Species s = speciesDict[genomes[i]];
                float score = EvaluateGenome(genomes[i]);
                float adjustedScore = score / speciesDict[genomes[i]].members.Count;
                s.AddAdjustedFitness(adjustedScore);
                genomes[i].Fitness = adjustedScore;
                if (score > highestScore) //Used to keep track of overall best 
                {
                    highestScore = score; 
                    FittestGenome = genomes[i];
                }

            }
        }

        private void AssignGenomesToSpecies()
        {
            for (int i = 0; i < genomes.Count; i++)
            {
                bool speciesFound = false;
                for (int j = 0; j < species.Count; j++)
                {
                    if (Genome.CompareGenes(genomes[i], species[j].mascot) < MAX_GENETIC_DIFF)
                    {
                        speciesFound = true;
                        species[j].members.Add(genomes[i]);
                        speciesDict.Add(genomes[i], species[j]);
                        break;
                    }
                }
                if (!speciesFound)
                {
                    Species s = new Species(genomes[i]);
                    species.Add(s);
                    speciesDict.Add(genomes[i], s);
                }
            }
        }

        protected abstract float EvaluateGenome(Genome genome);
    }
}
