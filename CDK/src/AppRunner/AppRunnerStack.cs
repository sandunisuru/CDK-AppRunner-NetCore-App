using System;
using System.Collections.Generic;
using Amazon.CDK;
using Amazon.CDK.AWS.AppRunner;
using Amazon.CDK.AWS.EC2;
using Amazon.CDK.AWS.Ecr.Assets;
using Amazon.CDK.AWS.IAM;
using AppRunner.Helpers;
using AppRunner.Models;
using Constructs;

namespace AppRunner
{
    public class AppRunnerStack : Stack
    {
        internal AppRunnerStack(Construct scope, string id, StackProps props, CustomStackProps customStackProps): base(scope, id, props)
        {
            var config = Helper.GetConfigs(customStackProps.Stage,
                (Dictionary<string, object>)scope.Node.TryGetContext("stages"));

            var appRunnerServiceRole = new Role(this, $"AppRunnerServiceRole{customStackProps.Stage}", new RoleProps()
            {
                AssumedBy = new ServicePrincipal("build.apprunner.amazonaws.com")
            });

            appRunnerServiceRole.AddManagedPolicy(
                ManagedPolicy.FromAwsManagedPolicyName("service-role/AWSAppRunnerServicePolicyForECRAccess"));

            var appRunnerInstanceRole = new Role(this, "AppRunnerInstanceRole", new RoleProps()
            {
                AssumedBy = new ServicePrincipal("tasks.apprunner.amazonaws.com")
            });

            var appRunnerContainerImage = new DockerImageAsset(this, "ECRImage", new DockerImageAssetProps()
            {
                Directory = "../../AppRunnerTestAPI"
            });

            List<EnvironmentVariables> environmentVariablesList = new List<EnvironmentVariables>();
            environmentVariablesList.Add(new EnvironmentVariables()
                { Name = "AWS_ACCESS_KEY", Value = (string)config.GetValueOrDefault("awsAccessKey") });
            environmentVariablesList.Add(new EnvironmentVariables()
                { Name = "AWS_SECRET_KEY", Value = (string)config.GetValueOrDefault("awsAccessSecret") });
            

            var appRunnerService = new CfnService(this, $"ApprunnerService{customStackProps.Stage}", new CfnServiceProps()
            {
                SourceConfiguration = new CfnService.SourceConfigurationProperty()
                {
                    AutoDeploymentsEnabled = true,
                    ImageRepository = new CfnService.ImageRepositoryProperty()
                    {
                        ImageRepositoryType = "ECR",
                        ImageIdentifier = appRunnerContainerImage.ImageUri,
                        ImageConfiguration = new CfnService.ImageConfigurationProperty()
                        {
                            Port = "80",
                            RuntimeEnvironmentVariables = environmentVariablesList
                        }
                    },
                    AuthenticationConfiguration = new CfnService.AuthenticationConfigurationProperty()
                        { AccessRoleArn = appRunnerServiceRole.RoleArn }
                },
                HealthCheckConfiguration = new CfnService.HealthCheckConfigurationProperty()
                {
                    Protocol = "HTTP",
                    Interval = 20,
                    HealthyThreshold = 1,
                    Path = "/api/health",
                    Timeout = 5,
                    UnhealthyThreshold = 3
                },
                ServiceName = StackName,
                InstanceConfiguration = new CfnService.InstanceConfigurationProperty()
                {
                    Cpu = "1024",
                    Memory = "2048",
                    InstanceRoleArn = appRunnerInstanceRole.RoleArn
                }
            });

            new CfnOutput(this, "AppRunnerServiceUrl", new CfnOutputProps()
            {
                Value = $"https://{appRunnerService.AttrServiceUrl}"
            });
        }
    }
}