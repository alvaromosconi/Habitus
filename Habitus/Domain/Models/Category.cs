namespace Habitus.Domain.Models;

public record Category
{
    public int Id { get; set; }
    public string Name { get; set; }
    public IEnumerable<Habit> habits { get; set; } = new List<Habit>();
};