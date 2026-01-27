using Microsoft.AspNetCore.Mvc;
using TodoApi.DTOs;
using TodoApi.Helpers;
using TodoApi.Services;

namespace TodoApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TodoController : ControllerBase
    {
        private readonly ITodoService _todoService;
        private readonly ILogger<TodoController> _logger;

        public TodoController(ITodoService todoService, ILogger<TodoController> logger)
        {
            _todoService = todoService;
            _logger = logger;
        }

        /// <summary>
        /// Get all todos
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<ApiResponse<IEnumerable<TodoItemDto>>>> GetAllTodos()
        {
            try
            {
                var todos = await _todoService.GetAllAsync();
                return Ok(ApiResponse<IEnumerable<TodoItemDto>>.SuccessResponse(todos, "Todos retrieved successfully"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting all todos");
                return StatusCode(500, ApiResponse<IEnumerable<TodoItemDto>>.ErrorResponse("Internal server error"));
            }
        }

        /// <summary>
        /// Get todo by id
        /// </summary>
        [HttpGet("{id}")]
        public async Task<ActionResult<ApiResponse<TodoItemDto>>> GetTodoById(int id)
        {
            try
            {
                var todo = await _todoService.GetByIdAsync(id);
                if (todo == null)
                    return NotFound(ApiResponse<TodoItemDto>.ErrorResponse("Todo not found"));

                return Ok(ApiResponse<TodoItemDto>.SuccessResponse(todo, "Todo retrieved successfully"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting todo by id");
                return StatusCode(500, ApiResponse<TodoItemDto>.ErrorResponse("Internal server error"));
            }
        }

        /// <summary>
        /// Get completed todos
        /// </summary>
        [HttpGet("status/completed")]
        public async Task<ActionResult<ApiResponse<IEnumerable<TodoItemDto>>>> GetCompletedTodos()
        {
            try
            {
                var todos = await _todoService.GetCompletedAsync();
                return Ok(ApiResponse<IEnumerable<TodoItemDto>>.SuccessResponse(todos, "Completed todos retrieved successfully"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting completed todos");
                return StatusCode(500, ApiResponse<IEnumerable<TodoItemDto>>.ErrorResponse("Internal server error"));
            }
        }

        /// <summary>
        /// Get pending todos
        /// </summary>
        [HttpGet("status/pending")]
        public async Task<ActionResult<ApiResponse<IEnumerable<TodoItemDto>>>> GetPendingTodos()
        {
            try
            {
                var todos = await _todoService.GetPendingAsync();
                return Ok(ApiResponse<IEnumerable<TodoItemDto>>.SuccessResponse(todos, "Pending todos retrieved successfully"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting pending todos");
                return StatusCode(500, ApiResponse<IEnumerable<TodoItemDto>>.ErrorResponse("Internal server error"));
            }
        }

        /// <summary>
        /// Get todos by category
        /// </summary>
        [HttpGet("category/{category}")]
        public async Task<ActionResult<ApiResponse<IEnumerable<TodoItemDto>>>> GetTodosByCategory(string category)
        {
            try
            {
                var todos = await _todoService.GetByCategoryAsync(category);
                return Ok(ApiResponse<IEnumerable<TodoItemDto>>.SuccessResponse(todos, $"Todos in category '{category}' retrieved successfully"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting todos by category");
                return StatusCode(500, ApiResponse<IEnumerable<TodoItemDto>>.ErrorResponse("Internal server error"));
            }
        }

        /// <summary>
        /// Create a new todo
        /// </summary>
        [HttpPost]
        public async Task<ActionResult<ApiResponse<TodoItemDto>>> CreateTodo([FromBody] CreateTodoDto dto)
        {
            try
            {
                if (!ValidationHelper.IsValidTitle(dto.Title))
                    return BadRequest(ApiResponse<TodoItemDto>.ErrorResponse("Title is required and must be less than 255 characters"));

                if (!ValidationHelper.IsValidDescription(dto.Description))
                    return BadRequest(ApiResponse<TodoItemDto>.ErrorResponse("Description must be less than 1000 characters"));

                if (!ValidationHelper.IsValidPriority(dto.Priority))
                    return BadRequest(ApiResponse<TodoItemDto>.ErrorResponse("Priority must be 0 (Low), 1 (Medium), or 2 (High)"));

                var todo = await _todoService.CreateAsync(dto);
                return CreatedAtAction(nameof(GetTodoById), new { id = todo.Id }, 
                    ApiResponse<TodoItemDto>.SuccessResponse(todo, "Todo created successfully"));
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning(ex, "Validation error creating todo");
                return BadRequest(ApiResponse<TodoItemDto>.ErrorResponse(ex.Message));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating todo");
                return StatusCode(500, ApiResponse<TodoItemDto>.ErrorResponse("Internal server error"));
            }
        }

        /// <summary>
        /// Update a todo
        /// </summary>
        [HttpPut("{id}")]
        public async Task<ActionResult<ApiResponse<TodoItemDto>>> UpdateTodo(int id, [FromBody] UpdateTodoDto dto)
        {
            try
            {
                if (dto.Title != null && !ValidationHelper.IsValidTitle(dto.Title))
                    return BadRequest(ApiResponse<TodoItemDto>.ErrorResponse("Title must be less than 255 characters"));

                if (dto.Description != null && !ValidationHelper.IsValidDescription(dto.Description))
                    return BadRequest(ApiResponse<TodoItemDto>.ErrorResponse("Description must be less than 1000 characters"));

                if (dto.Priority.HasValue && !ValidationHelper.IsValidPriority(dto.Priority.Value))
                    return BadRequest(ApiResponse<TodoItemDto>.ErrorResponse("Priority must be 0 (Low), 1 (Medium), or 2 (High)"));

                var todo = await _todoService.UpdateAsync(id, dto);
                if (todo == null)
                    return NotFound(ApiResponse<TodoItemDto>.ErrorResponse("Todo not found"));

                return Ok(ApiResponse<TodoItemDto>.SuccessResponse(todo, "Todo updated successfully"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating todo");
                return StatusCode(500, ApiResponse<TodoItemDto>.ErrorResponse("Internal server error"));
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
                var result = await _todoService.DeleteAsync(id);
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

        /// <summary>
        /// Get completion statistics
        /// </summary>
        [HttpGet("stats/completion")]
        public async Task<ActionResult<ApiResponse<int>>> GetCompletionPercentage()
        {
            try
            {
                var percentage = await _todoService.GetCompletionPercentageAsync();
                return Ok(ApiResponse<int>.SuccessResponse(percentage, "Completion percentage retrieved successfully"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting completion percentage");
                return StatusCode(500, ApiResponse<int>.ErrorResponse("Internal server error"));
            }
        }
    }
}
