using Microsoft.AspNetCore.Mvc;
using CampusLearn_Web_App.Services;
using CampusLearn_Web_App.Models;
using System.ComponentModel.DataAnnotations;
using OfficeOpenXml;

namespace CampusLearn_Web_App.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ModuleController : ControllerBase
    {
        private readonly ModuleService _moduleService;
        private readonly ILogger<ModuleController> _logger;

        public ModuleController(ModuleService moduleService, ILogger<ModuleController> logger)
        {
            _moduleService = moduleService;
            _logger = logger;
        }

        /// <summary>
        /// Get all modules
        /// </summary>
        /// <returns>List of all modules</returns>
        [HttpGet]
        public async Task<ActionResult<List<Module>>> GetAllModules()
        {
            try
            {
                _logger.LogInformation("📋 API request: Get all modules");
                var modules = await _moduleService.GetAllModules();
                _logger.LogInformation($"✅ Retrieved {modules.Count} modules");
                return Ok(modules);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Error retrieving all modules");
                return StatusCode(500, new { message = "Internal server error" });
            }
        }

        /// <summary>
        /// Get a specific module by ID
        /// </summary>
        /// <param name="id">Module ID</param>
        /// <returns>The module or 404 if not found</returns>
        [HttpGet("{id}")]
        public async Task<ActionResult<Module>> GetModule(int id)
        {
            try
            {
                _logger.LogInformation($"🔍 API request: Get module ID {id}");
                var module = await _moduleService.GetModuleById(id);
                
                if (module == null)
                {
                    _logger.LogWarning($"⚠️ Module ID {id} not found");
                    return NotFound(new { message = $"Module with ID {id} not found" });
                }

                _logger.LogInformation($"✅ Retrieved module: {module.ModuleName}");
                return Ok(module);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"❌ Error retrieving module ID {id}");
                return StatusCode(500, new { message = "Internal server error" });
            }
        }

        /// <summary>
        /// Get a module by code
        /// </summary>
        /// <param name="code">Module code</param>
        /// <returns>The module or 404 if not found</returns>
        [HttpGet("code/{code}")]
        public async Task<ActionResult<Module>> GetModuleByCode(string code)
        {
            try
            {
                _logger.LogInformation($"🔍 API request: Get module by code {code}");
                var module = await _moduleService.GetModuleByCode(code);
                
                if (module == null)
                {
                    _logger.LogWarning($"⚠️ Module code {code} not found");
                    return NotFound(new { message = $"Module with code {code} not found" });
                }

                _logger.LogInformation($"✅ Retrieved module: {module.ModuleName}");
                return Ok(module);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"❌ Error retrieving module by code {code}");
                return StatusCode(500, new { message = "Internal server error" });
            }
        }

        /// <summary>
        /// Add a new module
        /// </summary>
        /// <param name="request">Module creation request</param>
        /// <returns>The created module or error</returns>
        [HttpPost]
        public async Task<ActionResult<Module>> AddModule([FromBody] AddModuleRequest request)
        {
            try
            {
                _logger.LogInformation($"📝 API request: Add module {request.ModuleName} ({request.ModuleCode})");

                if (!ModelState.IsValid)
                {
                    _logger.LogWarning("⚠️ Invalid model state for add module request");
                    return BadRequest(ModelState);
                }

                var newModule = await _moduleService.AddModule(request.ModuleName, request.ModuleCode);
                
                if (newModule == null)
                {
                    _logger.LogWarning($"⚠️ Failed to add module - code {request.ModuleCode} may already exist");
                    return BadRequest(new { message = $"Module code {request.ModuleCode} already exists" });
                }

                _logger.LogInformation($"✅ Module successfully added: {newModule.ModuleName}");
                return CreatedAtAction(nameof(GetModule), new { id = newModule.ModuleID }, newModule);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"❌ Error adding module {request.ModuleName}");
                return StatusCode(500, new { message = "Internal server error" });
            }
        }

        /// <summary>
        /// Update an existing module
        /// </summary>
        /// <param name="id">Module ID to update</param>
        /// <param name="request">Module update request</param>
        /// <returns>The updated module or error</returns>
        [HttpPut("{id}")]
        public async Task<ActionResult<Module>> UpdateModule(int id, [FromBody] UpdateModuleRequest request)
        {
            try
            {
                _logger.LogInformation($"📝 API request: Update module ID {id}");

                if (!ModelState.IsValid)
                {
                    _logger.LogWarning("⚠️ Invalid model state for update module request");
                    return BadRequest(ModelState);
                }

                var updatedModule = await _moduleService.UpdateModule(id, request.ModuleName, request.ModuleCode);
                
                if (updatedModule == null)
                {
                    _logger.LogWarning($"⚠️ Failed to update module ID {id} - may not exist or code conflict");
                    return NotFound(new { message = $"Module with ID {id} not found or code {request.ModuleCode} already exists" });
                }

                _logger.LogInformation($"✅ Module successfully updated: {updatedModule.ModuleName}");
                return Ok(updatedModule);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"❌ Error updating module ID {id}");
                return StatusCode(500, new { message = "Internal server error" });
            }
        }

        /// <summary>
        /// Delete a module
        /// </summary>
        /// <param name="id">Module ID to delete</param>
        /// <returns>Success or error message</returns>
        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteModule(int id)
        {
            try
            {
                _logger.LogInformation($"🗑️ API request: Delete module ID {id}");

                var success = await _moduleService.DeleteModule(id);
                
                if (!success)
                {
                    _logger.LogWarning($"⚠️ Failed to delete module ID {id} - may not exist or has dependencies");
                    return BadRequest(new { message = $"Cannot delete module with ID {id} - module not found or has associated students/tutors/topics" });
                }

                _logger.LogInformation($"✅ Module ID {id} successfully deleted");
                return Ok(new { message = $"Module with ID {id} successfully deleted" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"❌ Error deleting module ID {id}");
                return StatusCode(500, new { message = "Internal server error" });
            }
        }

        /// <summary>
        /// Bulk import modules from Excel file
        /// </summary>
        /// <param name="file">Excel file containing module data</param>
        /// <returns>Import results with success/failure counts</returns>
        [HttpPost("bulk-import")]
        public async Task<ActionResult<BulkImportResult>> BulkImportModules(IFormFile file)
        {
            try
            {
                _logger.LogInformation($"📤 API request: Bulk import modules from file {file?.FileName}");

                if (file == null || file.Length == 0)
                {
                    _logger.LogWarning("⚠️ No file provided for bulk import");
                    return BadRequest(new { message = "No file provided" });
                }

                if (!Path.GetExtension(file.FileName).Equals(".xlsx", StringComparison.OrdinalIgnoreCase))
                {
                    _logger.LogWarning($"⚠️ Invalid file type: {Path.GetExtension(file.FileName)}");
                    return BadRequest(new { message = "Only .xlsx files are supported" });
                }

                var result = new BulkImportResult();
                var modules = new List<(string ModuleName, string ModuleCode)>();

                // Read Excel file
                using (var stream = new MemoryStream())
                {
                    await file.CopyToAsync(stream);
                    stream.Position = 0;

                    using (var package = new ExcelPackage(stream))
                    {
                        var worksheet = package.Workbook.Worksheets.FirstOrDefault();
                        if (worksheet == null)
                        {
                            _logger.LogWarning("⚠️ Excel file contains no worksheets");
                            return BadRequest(new { message = "Excel file contains no worksheets" });
                        }

                        var rowCount = worksheet.Dimension?.Rows ?? 0;
                        if (rowCount <= 1)
                        {
                            _logger.LogWarning("⚠️ Excel file contains no data rows");
                            return BadRequest(new { message = "Excel file contains no data (expecting header + data rows)" });
                        }

                        _logger.LogInformation($"📊 Processing {rowCount - 1} rows from Excel file");

                        // Read data (assuming first row is header: ModuleName, ModuleCode)
                        for (int row = 2; row <= rowCount; row++)
                        {
                            var moduleName = worksheet.Cells[row, 1].Value?.ToString()?.Trim();
                            var moduleCode = worksheet.Cells[row, 2].Value?.ToString()?.Trim();

                            if (string.IsNullOrEmpty(moduleName) || string.IsNullOrEmpty(moduleCode))
                            {
                                result.SkippedRows.Add($"Row {row}: Missing module name or code");
                                continue;
                            }

                            if (moduleName.Length > 100)
                            {
                                result.SkippedRows.Add($"Row {row}: Module name too long (max 100 characters)");
                                continue;
                            }

                            if (moduleCode.Length > 20)
                            {
                                result.SkippedRows.Add($"Row {row}: Module code too long (max 20 characters)");
                                continue;
                            }

                            modules.Add((moduleName, moduleCode));
                        }
                    }
                }

                _logger.LogInformation($"📋 Parsed {modules.Count} valid modules from Excel file");

                // Import modules
                foreach (var (moduleName, moduleCode) in modules)
                {
                    try
                    {
                        var newModule = await _moduleService.AddModule(moduleName, moduleCode);
                        if (newModule != null)
                        {
                            result.SuccessfulImports.Add($"{moduleName} ({moduleCode})");
                            result.SuccessCount++;
                        }
                        else
                        {
                            result.FailedImports.Add($"{moduleName} ({moduleCode}): Code already exists");
                            result.FailureCount++;
                        }
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, $"❌ Error importing module {moduleName} ({moduleCode})");
                        result.FailedImports.Add($"{moduleName} ({moduleCode}): {ex.Message}");
                        result.FailureCount++;
                    }
                }

                _logger.LogInformation($"✅ Bulk import completed: {result.SuccessCount} success, {result.FailureCount} failed, {result.SkippedRows.Count} skipped");

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Error during bulk import");
                return StatusCode(500, new { message = "Internal server error during bulk import" });
            }
        }
    }

    /// <summary>
    /// Request model for adding a new module
    /// </summary>
    public class AddModuleRequest
    {
        [Required]
        [MaxLength(100)]
        public string ModuleName { get; set; } = string.Empty;

        [Required]
        [MaxLength(20)]
        public string ModuleCode { get; set; } = string.Empty;
    }

    /// <summary>
    /// Request model for updating a module
    /// </summary>
    public class UpdateModuleRequest
    {
        [Required]
        [MaxLength(100)]
        public string ModuleName { get; set; } = string.Empty;

        [Required]
        [MaxLength(20)]
        public string ModuleCode { get; set; } = string.Empty;
    }

    /// <summary>
    /// Result model for bulk import operations
    /// </summary>
    public class BulkImportResult
    {
        public int SuccessCount { get; set; } = 0;
        public int FailureCount { get; set; } = 0;
        public List<string> SuccessfulImports { get; set; } = new();
        public List<string> FailedImports { get; set; } = new();
        public List<string> SkippedRows { get; set; } = new();
        
        public string Summary => $"Import completed: {SuccessCount} successful, {FailureCount} failed, {SkippedRows.Count} skipped";
    }
}