namespace AzureAiMicroservice.DTOs;

public class JiraIssueTrainDto
{
    public JiraIssueDto Input { get; set; }
    public List<JiraIssueDto> Output { get; set; }
}