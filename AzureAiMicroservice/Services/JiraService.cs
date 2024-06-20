using Atlassian.Jira;
using AzureAiMicroservice.Configurations;
using Microsoft.Extensions.Options;

namespace AzureAiMicroservice.Services;

public class JiraService(IOptions<JiraOptions> options,ILogger<JiraService> logger) : ITicketService
{
    private readonly Jira jira = Jira.CreateRestClient(options.Value.BaseUrl, options.Value.Username, options.Value.ApiToken);
    
    public async Task GetTasks()
    {
        // Define the JQL query
        var jql = $"project = {options.Value.ProjectKey}";
        
        var issues = await jira.Issues.GetIssuesFromJqlAsync(jql);
        
        foreach (var issue in issues)
        {
            logger.LogInformation($"Key: {issue.Key}, Summary: {issue.Summary}, Status: {issue.Status}");
        }
    }
}