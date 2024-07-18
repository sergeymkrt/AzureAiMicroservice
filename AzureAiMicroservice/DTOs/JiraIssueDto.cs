using Atlassian.Jira;

namespace AzureAiMicroservice.DTOs;

public class JiraIssueDto(Issue issue)
{
    public string Summary { get; set; } = issue.Summary;
    public string Description { get; set; } = issue.Description;
}