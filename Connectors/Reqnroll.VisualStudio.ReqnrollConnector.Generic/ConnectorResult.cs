namespace ReqnrollConnector.Discovery;

public record ConnectorResult(
    ImmutableArray<StepDefinition> StepDefinitions,
    ImmutableArray<Reqnroll.VisualStudio.ReqnrollConnector.Models.Hook> Hooks,
    ImmutableSortedDictionary<string, string> SourceFiles,
    ImmutableSortedDictionary<string, string> TypeNames,
    ImmutableSortedDictionary<string, string> AnalyticsProperties,
    string? ErrorMessage
);
