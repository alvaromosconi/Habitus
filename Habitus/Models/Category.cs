namespace Habitus.Models;

public class Category
{
    public int Id { get; set; }
    public string Name { get; set; }

    public ICollection<Habit> Habits { get; set; }   
}