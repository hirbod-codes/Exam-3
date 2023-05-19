namespace Exam_3.Models;

using System.Collections.Generic;

public class Car
{
    public int Id { get; set; }
    public int? MechanicId { get; set; }
    public string Model { get; set; } = null!;
    public List<int> VisitIds { get; set; } = new List<int>();
}
