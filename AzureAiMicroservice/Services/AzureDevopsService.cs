using AzureAiMicroservice.Configurations;
using Microsoft.Extensions.Options;
using Microsoft.TeamFoundation.WorkItemTracking.WebApi;
using Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.WebApi;

namespace AzureAiMicroservice.Services;

public class AzureDevopsService(IOptions<AzureDevopsOptions> options) : ITicketService
{
    private readonly VssConnection connection = new(new Uri(options.Value.BaseUrl),
        new VssBasicCredential(string.Empty, options.Value.PersonalAccessToken));

    /// <summary>
    /// Example.
    /// </summary>
    public async Task GetTasks()
    {
        var workItemTrackingClient = connection.GetClient<WorkItemTrackingHttpClient>();
        var wiql = new Wiql()
        {
            Query = "SELECT [System.Id], [System.Title], [System.State] FROM WorkItems WHERE [System.WorkItemType] = 'User Story'"
        };

        var result = await workItemTrackingClient.QueryByWiqlAsync(wiql, options.Value.ProjectName);

        if (result.WorkItems.Any())
        {
            var ids = result.WorkItems.Select(wi => wi.Id).Take(200).ToArray();
            
            var workItems = await workItemTrackingClient.GetWorkItemsAsync(ids);

            // Print the work item details
            foreach (var workItem in workItems)
            {
                Console.WriteLine($"ID: {workItem.Id}, Title: {workItem.Fields["System.Title"]}, State: {workItem.Fields["System.State"]}");
            }
        }
        else
        {
            Console.WriteLine("No user stories found.");
        }
    }
}