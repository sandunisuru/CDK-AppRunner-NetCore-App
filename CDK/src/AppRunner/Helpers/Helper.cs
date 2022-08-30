using System.Collections.Generic;

namespace AppRunner.Helpers;

public class Helper
{
    public static Dictionary<string, object> GetConfigs(string stage, Dictionary<string, object> configObject)
    {
        object stageVariables;
        configObject.TryGetValue(stage, out stageVariables);
        return (Dictionary<string, object>)stageVariables;
    }
}