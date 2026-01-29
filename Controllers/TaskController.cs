using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TodoApi.DTOs;
using TodoApi.Helpers;
using TodoApi.Services;

namespace TodoApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class taskController : ControllerBase
    {
        private readonly ITaskService _taskService;
        private readonly ILogger<taskController> _logger;

        public taskController(ITaskService taskService, ILogger<taskController> logger)
        {
            _taskService = taskService;
            _logger = logger;
        }

        [HttpGet]
        public async Task<ActionResult<ApiResponse<IEnumerable<TaskItemDto>>>> GetAllTasks()
        {
            try
            {
                var tasks = await _taskService.GetAllAsync();
                return Ok(ApiResponse<IEnumerable<TaskItemDto>>.SuccessResponse(tasks, "User tasks retrieved successfully"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting user tasks");
                return StatusCode(500, ApiResponse.ErrorResponse("Internal server error"));
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ApiResponse<TaskItemDto>>> GetTodoById(int id)
        {
            try
            {
                var todo = await _taskService.GetByIdAsync(id);
                if (todo == null)
                    return NotFound(ApiResponse.ErrorResponse("Todo not found"));

                return Ok(ApiResponse<TaskItemDto>.SuccessResponse(todo, "Todo retrieved successfully"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting todo by id");
                return StatusCode(500, ApiResponse.ErrorResponse("Internal server error"));
            }
        }

        [HttpPost("complete/{id}")]
        public async Task<ActionResult<ApiResponse<TaskItemDto>>> CompleteTask(int id)
        {
            try
            {
                await _taskService.CompleteTaskAsync(id);
                var task = await _taskService.GetByIdAsync(id);
                if (task == null)
                    return NotFound(ApiResponse.ErrorResponse("Todo not found"));

                return CreatedAtAction(nameof(GetTodoById), new { id = task.Id },
                    ApiResponse<TaskItemDto>.SuccessResponse(task, "Todo created successfully"));
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning(ex, "Validation error creating todo");
                return BadRequest(ApiResponse.ErrorResponse(ex.Message));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating todo");
                return StatusCode(500, ApiResponse.ErrorResponse("Internal server error"));
            }
        }

        [HttpPost]
        public async Task<ActionResult<ApiResponse<TaskItemDto>>> CreateTask([FromBody] CreateTaskDto dto)
        {
            try
            {
                if (!ValidationHelper.IsValidTitle(dto.Title))
                    return BadRequest(ApiResponse.ErrorResponse("Title is required and must be less than 255 characters"));

                if (!ValidationHelper.IsValidDescription(dto.Description))
                    return BadRequest(ApiResponse.ErrorResponse("Description must be less than 1000 characters"));

                var todo = await _taskService.CreateAsync(dto);
                return CreatedAtAction(nameof(GetTodoById), new { id = todo.Id },
                    ApiResponse<TaskItemDto>.SuccessResponse(todo, "Todo created successfully"));
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning(ex, "Validation error creating todo");
                return BadRequest(ApiResponse.ErrorResponse(ex.Message));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating todo");
                return StatusCode(500, ApiResponse.ErrorResponse("Internal server error"));
            }
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<ApiResponse<TaskItemDto>>> UpdateTodo(int id, [FromBody] CreateTaskDto dto)
        {
            try
            {
                if (dto.Title != null && !ValidationHelper.IsValidTitle(dto.Title))
                    return BadRequest(ApiResponse.ErrorResponse("Title must be less than 255 characters"));

                if (dto.Description != null && !ValidationHelper.IsValidDescription(dto.Description))
                    return BadRequest(ApiResponse.ErrorResponse("Description must be less than 1000 characters"));
                var todo = await _taskService.UpdateAsync(id, dto);
                if (todo == null)
                    return NotFound(ApiResponse.ErrorResponse("Todo not found"));

                return Ok(ApiResponse<TaskItemDto>.SuccessResponse(todo, "Todo updated successfully"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating todo");
                return StatusCode(500, ApiResponse.ErrorResponse("Internal server error"));
            }
        }
        [HttpDelete("{id}")]
        public async Task<ActionResult<ApiResponse>> DeleteTodo(int id)
        {
            try
            {
                var result = await _taskService.DeleteAsync(id);
                if (!result)
                    return NotFound(ApiResponse.ErrorResponse("Todo not found"));

                return Ok(ApiResponse.SuccessResponse("Todo deleted successfully"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting todo");
                return StatusCode(500, ApiResponse.ErrorResponse("Internal server error"));
            }
        }
        [HttpGet("search")]
        public async Task<ActionResult<ApiResponse<IEnumerable<TaskItemDto>>>> SearchTasks([FromBody] SearchTaskDto dto)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(dto.SearchTerm))
                {
                    return BadRequest(ApiResponse.ErrorResponse("Search term cannot be empty"));
                }
                var tasks = await _taskService.SearchAsync(dto.SearchTerm);
                return Ok(ApiResponse<IEnumerable<TaskItemDto>>.SuccessResponse(tasks, "Search completed successfully"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error searching tasks");
                return StatusCode(500, ApiResponse.ErrorResponse("Internal server error"));
            }
        }
        [HttpPost("get-paged")]
        public async Task<ActionResult<ApiResponse<PagedTaskItemDto>>> GetPagedTasks([FromBody] PagedRequestDto dto)
        {
            try
            {
                if (dto.PageNumber <= 0 || dto.Limit <= 0)
                {
                    return BadRequest(ApiResponse.ErrorResponse("PageNumber and Limit must be greater than zero"));
                }
                var pagedTasks = await _taskService.GetPagedAsync(dto.PageNumber, dto.Limit);
                return Ok(ApiResponse<PagedTaskItemDto>.SuccessResponse(pagedTasks, "Paged tasks retrieved successfully"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting paged tasks");
                return StatusCode(500, ApiResponse.ErrorResponse("Internal server error"));
            }
        }
    }
}

        