namespace BlazorApp.Models;

public class Space
{
    public int Id { get; set; }

    public string Name { get; set; }

    public Photo? Photo { get; set; }
    
    public string? PhotoUrl { get; set; }

    public bool Availability { get; set; }

    public int CondominiumId {  get; set; }
}