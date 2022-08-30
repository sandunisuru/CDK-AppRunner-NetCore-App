using System.Collections.Generic;
using Amazon.CDK;
using Amazon.CDK.AWS.StepFunctions;
using Constructs;

namespace AppRunner.Models
{
    public class CustomStackProps
    {
        public string? App { get; set; }
        public string Stage { get; set; }
    }
}