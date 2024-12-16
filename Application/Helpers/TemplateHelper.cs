namespace Application.Helpers;

public static class TemplateHelper
{
    public static string GetTemplateContent(Dictionary<string, string> placeholders)
    {
        var templatePath = Path.Combine(AppContext.BaseDirectory, "Templates", "EmailTemplate.html");
        var templateContent = File.ReadAllText(templatePath);

        foreach (var placeholder in placeholders)
        {
            templateContent = templateContent.Replace($"{{{{{placeholder.Key}}}}}", placeholder.Value);
        }

        return templateContent;
    }
}