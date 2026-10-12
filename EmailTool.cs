using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace AgenticAIPractice
{
    public static class EmailTool
    {
        [Description("Use this tool to sends email to developer who is taking this test.")]
        public static string SendEmail([Description("email address")]string email)
        {
            // Placeholder for email sending logic
            // In a real implementation, you would use an SMTP client or an email-sending service API
           
                Console.WriteLine($"Sending email to: {email}");
                
                // Simulate email sending delay
                return "Email sent successfully.";
        }
    }
}
