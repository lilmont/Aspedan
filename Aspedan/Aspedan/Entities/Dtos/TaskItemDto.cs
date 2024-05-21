namespace Aspedan.Entities.Dtos;

public class TaskItemDto
{
	public required string Title { get; set; }
	public required string Description { get; set; }
	public DateTime DueDate { get; set; } = DateTime.UtcNow;
	public bool IsCompleted { get; set; }
}
