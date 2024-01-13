using System.ComponentModel.DataAnnotations;

namespace Habitus.Domain.Models;

public class Category
{
    [Key]
    public int Id { get; set; }
    public string Name { get; set; }
    public IEnumerable<Habit> habits { get; set; } = new List<Habit>();
};