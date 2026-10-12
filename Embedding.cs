using OpenAI.Embeddings;
using System;
using System.Collections.Generic;
using System.Text;

namespace AgenticAIPractice
{
    public class RAGLookup
    {
        public int Id { get; set; }
        public string Description { get; set; }
        public string QuestionstobeAsked { get; set; }
        public float[] DescriptionEmbedding { get; set; }
        public List<String> Questions { get; set; }
        public RAGLookup()
        {
            Questions = new List<String>();
        }
    }
    public static class Embedding
    {
        public static async Task<string> Generate(string input)
        {
            var apiKey = Environment.GetEnvironmentVariable("ApiKey")
                ?? throw new InvalidOperationException("OPENAI_API_KEY environment variable is not set.");
            var embeddingClient = new EmbeddingClient("text-embedding-3-small", apiKey);

            List<RAGLookup> lookupStore = getRAGData();

            foreach (var item in lookupStore)
            {
                var embed = await embeddingClient.GenerateEmbeddingAsync(item.Description);
                item.DescriptionEmbedding = embed.Value.ToFloats().ToArray();

            }
            while (1 == 1)
            {
                string candidateExp = input;
                var candidateEmbed = await embeddingClient.GenerateEmbeddingAsync(candidateExp);
                var candidateVector = candidateEmbed.Value.ToFloats().ToArray();

                var bestMatch = lookupStore
                    .OrderByDescending(x => CalculateCosineSimilarity(x.DescriptionEmbedding, candidateVector))
                    .First();

                Console.WriteLine(bestMatch.Description);
                return bestMatch.QuestionstobeAsked;
            }
        }
        public static List<RAGLookup> getRAGData()
        {
            return new List<RAGLookup>
            {
            new RAGLookup
            {
                Description = "0-3 yrs year experience .NET developer",
                QuestionstobeAsked = "Basics of C#, OOP, SQL Server.",
            },
            new RAGLookup
            {
                Description = "Senior .NET developer with 5+ years",
                QuestionstobeAsked = "Advanced ASP.NET Core, microservices, cloud, design patterns."
            },
            new RAGLookup
            {
                Description = "Architect 10+ years",
                QuestionstobeAsked = "Distributed systems, DDD, Azure architecture, performance tuning."
            }
            };
        }
        public static double CalculateCosineSimilarity(float[] vector1, float[] vector2)
        {
            if (vector1.Length != vector2.Length)
            {
                throw new ArgumentException("Vectors must be of the same length.");
            }

            double dotProduct = 0.0;
            double magnitude1 = 0.0;
            double magnitude2 = 0.0;

            for (int i = 0; i < vector1.Length; i++)
            {
                dotProduct += vector1[i] * vector2[i];
                magnitude1 += vector1[i] * vector1[i];
                magnitude2 += vector2[i] * vector2[i];
            }

            magnitude1 = Math.Sqrt(magnitude1);
            magnitude2 = Math.Sqrt(magnitude2);

            if (magnitude1 == 0.0 || magnitude2 == 0.0)
            {
                // Handle cases where one or both vectors are zero vectors
                return 0.0;
            }

            return dotProduct / (magnitude1 * magnitude2);
        }
    }
}
