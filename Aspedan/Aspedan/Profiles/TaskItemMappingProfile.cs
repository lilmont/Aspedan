namespace Aspedan.Profiles;

public class TaskItemMappingProfile : Profile
{
	public TaskItemMappingProfile()
	{
		CreateMap<TaskItem, TaskItemDto>().ReverseMap();
	}
}
