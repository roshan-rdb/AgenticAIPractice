using Microsoft.Agents.AI;
using Microsoft.Extensions.AI;
using OpenAI.Chat;
using System;
using System.Collections.Generic;
using System.Text;

namespace AgenticAIPractice
{
    public static class MockInterviewAgent
    {
        public async static Task Run()
        {
            var apiKey = Environment.GetEnvironmentVariable("ApiKey")
                ?? throw new InvalidOperationException("OPENAI_API_KEY environment variable is not set.");
            var modelId = "gpt-5.4-mini";

            IChatClient chatClient = new ChatClient(modelId, apiKey).AsIChatClient();

            AIAgent agent = chatClient.CreateAIAgent(
                name: "MockInterviewAgent",
                instructions: """
                    You are a mock interview agent that helps users prepare for job interviews.
                    - As you are mock interviewing the user, give only relevant questions based on the user's background and the job role.
                    - For other questions, politely ask the user to provide more information about their background and the job role.
                    - Ask only 3 relevant questions based on the user's background and the job role.
                    - Be professional, encouraging, and constructive in your feedback.
                    """
            //tools: new List<AITool>() // Add any tools if needed
            );

            Console.WriteLine("Mock Interview Agent is ready with tools. Give me your background and the job role(type 'exit' to quit).\n");

            var thread = agent.GetNewThread();

            while (true)
            {
                Console.Write("You: ");
                var userInput = Console.ReadLine();
                if (string.IsNullOrWhiteSpace(userInput) || userInput.Equals("exit", StringComparison.OrdinalIgnoreCase))
                    break;
                Console.WriteLine("Agent \n");
                await foreach (var response in agent.RunStreamingAsync(userInput, thread))
                {
                    Console.Write(response.ToString());
                }
                Console.WriteLine("\n");
            }
        }
    }
}
