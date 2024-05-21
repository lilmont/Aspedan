namespace Aspedan.Controllers;

[Route("api/TaskItem")]
[ApiController]
public class TaskItemController(
	AspedanDbContext dbContext,
	IMapper mapper,
	TaskItemValidation validator) : ControllerBase
{
	private readonly AspedanDbContext _dbContext = dbContext;
	private readonly IMapper _mapper = mapper;

	[HttpGet]
	[ProducesResponseType(StatusCodes.Status200OK)]
	public async Task<ActionResult<IEnumerable<TaskItemDto>>> Get(CancellationToken cancellationToken) =>
		Ok(await _dbContext.TaskItems.ToListAsync(cancellationToken));

	[HttpPost]
	[ProducesResponseType(StatusCodes.Status200OK)]
	[ProducesResponseType(StatusCodes.Status400BadRequest)]
	[ProducesResponseType(StatusCodes.Status500InternalServerError)]
	public async Task<ActionResult> Create([FromBody] TaskItemDto taskItemDto, CancellationToken cancellationToken)
	{
		try
		{
			var validationResult = await validator.ValidateAsync(taskItemDto, cancellationToken);
			if (!validationResult.IsValid)
				return BadRequest(validationResult.ToErrorDetails());

			await _dbContext.TaskItems.AddAsync(_mapper.Map<TaskItem>(taskItemDto), cancellationToken);
			await _dbContext.SaveChangesAsync(cancellationToken);
			return Ok($"{taskItemDto.Title} Created!");
		}
		catch
		{
			return StatusCode(StatusCodes.Status500InternalServerError);
		}
	}

	[HttpPut("{id:int}")]
	public async Task<ActionResult> Update(int id, [FromBody] TaskItemDto taskItemDto, CancellationToken cancellationToken)
	{
		try
		{
			var validationResult = await validator.ValidateAsync(taskItemDto, cancellationToken);
			if (!validationResult.IsValid)
				return BadRequest(validationResult.ToErrorDetails());

			var updatedTaskItem = _mapper.Map<TaskItem>(taskItemDto);
			updatedTaskItem.Id = id;
			_dbContext.Update(updatedTaskItem);
			await _dbContext.SaveChangesAsync(cancellationToken);
			return Ok($"{taskItemDto.Title} Updated!");
		}
		catch
		{
			return StatusCode(StatusCodes.Status500InternalServerError);
		}
	}

	[HttpDelete("{id:int}")]
	[ProducesResponseType(StatusCodes.Status200OK)]
	[ProducesResponseType(StatusCodes.Status400BadRequest)]
	[ProducesResponseType(StatusCodes.Status500InternalServerError)]
	public async Task<ActionResult> Delete(int id, CancellationToken cancellationToken)
	{
		try
		{
			var currentTaskItem = await _dbContext.TaskItems.FindAsync(id);
			if (currentTaskItem is null)
				return NotFound("Task does not exist!");

			_dbContext.TaskItems.Remove(currentTaskItem);
			await _dbContext.SaveChangesAsync(cancellationToken);
			return Ok($"{currentTaskItem.Title} Deleted!");
		}
		catch
		{
			return StatusCode(StatusCodes.Status500InternalServerError);
		}
	}
}
