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

        /// <summary>
        /// Get all todos
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<ApiResponse<IEnumerable<TaskItemDto>>>> GetAllTodos()
        {
            try
            {
                var tasks = await _taskService.GetAllAsync();

                return Ok(ApiResponse<IEnumerable<TaskItemDto>>.SuccessResponse(tasks, "User tasks retrieved successfully"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting user tasks");
                return StatusCode(500, ApiResponse<IEnumerable<TaskItemDto>>.ErrorResponse("Internal server error"));
            }
        }

        /// <summary>
        /// Get todo by id
        /// </summary>
        [HttpGet("{id}")]
        public async Task<ActionResult<ApiResponse<TaskItemDto>>> GetTodoById(int id)
        {
            try
            {
                var todo = await _taskService.GetByIdAsync(id);
                if (todo == null)
                    return NotFound(ApiResponse<TaskItemDto>.ErrorResponse("Todo not found"));

                return Ok(ApiResponse<TaskItemDto>.SuccessResponse(todo, "Todo retrieved successfully"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting todo by id");
                return StatusCode(500, ApiResponse<TaskItemDto>.ErrorResponse("Internal server error"));
            }
        }

        /// <summary>
        /// Create a new todo
        /// </summary>
        [HttpPost]
        public async Task<ActionResult<ApiResponse<TaskItemDto>>> CreateTask([FromBody] CreateTaskDto dto)
        {
            try
            {
                if (!ValidationHelper.IsValidTitle(dto.Title))
                    return BadRequest(ApiResponse<TaskItemDto>.ErrorResponse("Title is required and must be less than 255 characters"));

                if (!ValidationHelper.IsValidDescription(dto.Description))
                    return BadRequest(ApiResponse<TaskItemDto>.ErrorResponse("Description must be less than 1000 characters"));

                var todo = await _taskService.CreateAsync(dto);
                return CreatedAtAction(nameof(GetTodoById), new { id = todo.Id },
                    ApiResponse<TaskItemDto>.SuccessResponse(todo, "Todo created successfully"));
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning(ex, "Validation error creating todo");
                return BadRequest(ApiResponse<TaskItemDto>.ErrorResponse(ex.Message));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating todo");
                return StatusCode(500, ApiResponse<TaskItemDto>.ErrorResponse("Internal server error"));
            }
        }

        /// <summary>
        /// Update a todo
        /// </summary>
        [HttpPut("complete/{id}")]
        public async Task<ActionResult<ApiResponse<TaskItemDto>>> UpdateTodo(int id, [FromBody] CompleteTaskDto dto)
        {
            try
            {
                if (dto.Title != null && !ValidationHelper.IsValidTitle(dto.Title))
                    return BadRequest(ApiResponse<TaskItemDto>.ErrorResponse("Title must be less than 255 characters"));

                if (dto.Description != null && !ValidationHelper.IsValidDescription(dto.Description))
                    return BadRequest(ApiResponse<TaskItemDto>.ErrorResponse("Description must be less than 1000 characters"));

                var todo = await _taskService.CompleteTaskAsync(id, dto);
                if (todo == null)
                    return NotFound(ApiResponse<TaskItemDto>.ErrorResponse("Todo not found"));

                return Ok(ApiResponse<TaskItemDto>.SuccessResponse(todo, "Todo updated successfully"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating todo");
                return StatusCode(500, ApiResponse<TaskItemDto>.ErrorResponse("Internal server error"));
            }
        }

        /// <summary>
        /// Delete a todo
        /// </summary>
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
    }
}
