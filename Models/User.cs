namespace Exam_3.Models;

using System.Collections.Generic;

public class User
{
    public int Id { get; set; }
    public bool IsMechanic { get; set; } = false;
    public string Username { get; set; } = null!;
    public string Password { get; set; } = null!;
    public List<Car> Cars { get; set; } = new List<Car>();
}