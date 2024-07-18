using Atlassian.Jira;
using AzureAiMicroservice.Configurations;
using AzureAiMicroservice.DTOs;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System.Text;

namespace AzureAiMicroservice.Services;

public class JiraService(IOptions<JiraOptions> options, ILogger<JiraService> logger) : ITicketService
{
    private readonly Jira _jira = Jira.CreateRestClient(options.Value.BaseUrl, options.Value.Username, options.Value.ApiToken);
    private const int MaxIssues = 100;

    public async Task<MemoryStream> GetTasks(int? maxResult = null)
    {
        var issueTypes = await _jira.IssueTypes.GetIssueTypesAsync();
        var tasks = issueTypes.Where(i => !i.IsSubTask && i.Name != "Epic" && !i.Name.Contains("[System]")).Select(x => x.Id).ToList();
        // Define the JQL query
        var jql = $"project = {options.Value.ProjectKey} AND (issuetype = Task or issuetype IN ({string.Join(",",tasks)}))";
        
        var startAt = 0;
        var total = 0;
        var trainData = new List<JiraIssueTrainDto>();
        do
        {
            IPagedQueryResult<Issue> currentIssues = await _jira.Issues.GetIssuesFromJqlAsync(jql, MaxIssues, startAt);
            total = currentIssues.TotalItems;
            startAt += currentIssues.ItemsPerPage;

            try
            {
                foreach (var issue in currentIssues)
                {
                    var subTasks = await issue.GetSubTasksAsync(maxIssues: 20);
                    if(!subTasks.Any())
                    {
                        continue;
                    }
                    
                    var trainDto = new JiraIssueTrainDto
                    {
                        Input = new JiraIssueDto(issue),
                        Output = subTasks.Select(s => new JiraIssueDto(s)).ToList()
                    };
                    trainData.Add(trainDto);
                }
            }
            catch (Exception e)
            {
                logger.LogError(e, $"Error processing Jira batch from {startAt}");
            }

            logger.LogInformation($"Retrieved {startAt} of {total} Jira tasks");

            if (trainData.Count > 50)
            {
                break;
            }
        } while (startAt < total);

        var json = JsonConvert.SerializeObject(trainData);
        return new MemoryStream(Encoding.UTF8.GetBytes(json));
    }
}