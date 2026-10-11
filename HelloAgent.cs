using Microsoft.Agents.AI;
using Microsoft.Extensions.AI;
using OpenAI;
using OpenAI.Chat;

namespace AgenticAIPractice
{
    public static class HelloAgent
    {
        public static async Task Run()
        {
            var apiKey = Environment.GetEnvironmentVariable("ApiKey")
                ?? throw new InvalidOperationException("OPENAI_API_KEY environment variable is not set.");

            // Define the lools
            var tools = new List<AITool>
            {
                AIFunctionFactory.Create(DateTimeTools.GetCurrentDateTimeString),
                AIFunctionFactory.Create(DateTimeTools.CalculateDateDifference),
                AIFunctionFactory.Create(CalculatorTools.Calculate),
                AIFunctionFactory.Create(CalculatorTools.CalculatePercentage)
            };

            var agent = new OpenAIClient(apiKey)
                            .GetChatClient("gpt-5.4-mini")
                            .CreateAIAgent(
                                       name: "DevAssistant",
                                       instructions: """
                                   You are an AI assistant with access to tools.

                                   BEHAVIOR:
                                   - Be concise but thorough
                                   - Use tools when appropriate instead of making up answers
                                   - Always respond in the user's language

                                   IMPORTANT:
                                   - For calculations, ALWAYS use the calculator tool
                                   - Never make up data: use tools to get real information
                                   """,
                                       tools: tools);

            Console.WriteLine("Agent is ready with tools. Type your message and press Enter (type 'exit' to quit).\n");

            var thread = agent.GetNewThread();

            while (true)
            {
                Console.Write("You: ");
                var userInput = Console.ReadLine();
                if (string.IsNullOrWhiteSpace(userInput) || userInput.Equals("exit", StringComparison.OrdinalIgnoreCase))
                    break;
                Console.WriteLine("Agent > ");
                await foreach (var response in agent.RunStreamingAsync(userInput, thread))
                {
                    Console.Write(response.ToString());
                }
                Console.WriteLine("\n");
            }
        }
    }
}
