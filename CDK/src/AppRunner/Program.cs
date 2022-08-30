using Amazon.CDK;
using System;
using System.Collections.Generic;
using System.Linq;
using AppRunner.Models;
using Environment = Amazon.CDK.Environment;

namespace AppRunner
{
    sealed class Program
    {
        public static void Main(string[] args)
        {
            var app = new App();
            new AppRunnerStack(app, "AppRunnerStack", new StackProps()
            {
                Env = new Environment() { Account = "AWS_ACCOUNT_ID", Region = "eu-west-1" }
            },
                new CustomStackProps()
            {
                App = "app_runner",
                Stage = "dev"
            });
            app.Synth();
        }
    }
}