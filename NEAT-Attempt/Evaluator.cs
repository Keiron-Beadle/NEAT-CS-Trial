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

        private const float MAX_GENETIC_DIFF = 3.0f;
        private float highestScore = 0.0f;
        private Genome fittestGenome = null;

        public Evaluator()
        {
            genomes = new List<Genome>();
            nextGeneration = new List<Genome>();
            speciesDict = new Dictionary<Genome, Species>();
            scoreDict = new Dictionary<Genome, float>();
            species = new List<Species>();
        }

        void Evaluate()
        {
            //Step 1: Assign genomes into species
            AssignGenomesToSpecies();

            //Step 2: Run Simulation for Genomes
            RunSimulation();

            for (int i = 0; i < species.Count; i++)
            {
                MergeSort.Sort(ref species[i].members);

            }
            //Put best genomes from each species into next gen
            //Breed the rest of the genomes

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
                //if (score > highestScore) //Used to keep track of overall best 
                // {
                //     highestScore = score; 
                //     fittestGenome = genomes[i];
                // }

            }
        }

        private void AssignGenomesToSpecies()
        {
            for (int i = 0; i < genomes.Count; i++)
            {
                bool speciesFound = false;
                for (int j = 0; j < species.Count; j++)
                {
                    if (Genome.CompareGenes(genomes[i], species[i].mascot) < MAX_GENETIC_DIFF)
                    {
                        speciesFound = true;
                        species[j].members.Add(genomes[i]);
                        speciesDict.Add(genomes[i], species[i]);
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

        public abstract float EvaluateGenome(Genome genome);

        int CompareGenomesFitness(Genome g1, Genome g2)
        {
            if (g1.Fitness > g2.Fitness) return 1;
            else if (g1.Fitness < g2.Fitness) return -1;
            else return 0;
        }
    }
}
