using Microsoft.Agents.AI;
using Microsoft.Extensions.AI;
using OpenAI;
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


            // Define the lools
            var tools = new List<AITool>
            {
                AIFunctionFactory.Create(EmailTool.SendEmail),
            };

            IChatClient chatClient = new ChatClient(modelId, apiKey).AsIChatClient();

            var agent = new OpenAIClient(apiKey)
                            .GetChatClient("gpt-5.4-mini")
                            .CreateAIAgent(
                name: "MockInterviewAgent",
                instructions: """
                    You are a mock interview agent that helps users prepare for job interviews with access to tools..
                    BEHAVIOR:
                    - As you are mock interviewing the user, give only relevant questions based on the user's background and the job role.
                    - For other questions denied, politely inform the user that you will not ask those questions as they are not relevant to the user's background and the job role.    
                    - Ask only 5 relevant questions based on the user's background and the job role.
                    - Be professional, encouraging, and constructive in your feedback.
                    
                    IMPORTANT:
                    - For semding email, ALWAYS use the email tool if have email address in user's input. Do not send email directly from the agent. Use the email tool to send email.
                    """,
                   tools: tools
            );

            Console.WriteLine(".Net Mock Interview Agent is ready with tools. Give me your background and the job role(type 'exit' to quit).\n");

            var thread = agent.GetNewThread();
            
            while (true)
            {
                Console.Write("You: ");
                var userInput = Console.ReadLine();
                var bestMatchDescription = await Embedding.Generate(userInput);
                if (string.IsNullOrWhiteSpace(userInput) || userInput.Equals("exit", StringComparison.OrdinalIgnoreCase))
                    break;
                Console.WriteLine("Agent \n");
                await foreach (var response in agent.RunStreamingAsync($"{userInput} ask questions around {bestMatchDescription}", thread))
                {
                    Console.Write(response.ToString());
                }
                Console.WriteLine("\n");
            }
        }
    }
}
