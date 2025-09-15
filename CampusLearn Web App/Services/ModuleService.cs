using CampusLearn_Web_App.Data;
using CampusLearn_Web_App.Models;
using Microsoft.EntityFrameworkCore;

namespace CampusLearn_Web_App.Services
{
    public class ModuleService
    {
        private readonly CampusLearnDbContext _context;
        private readonly ILogger<ModuleService> _logger;

        public ModuleService(CampusLearnDbContext context, ILogger<ModuleService> logger)
        {
            _context = context;
            _logger = logger;
        }

        /// <summary>
        /// Adds a new module to the system
        /// </summary>
        /// <param name="moduleName">The name of the module</param>
        /// <param name="moduleCode">The unique code for the module</param>
        /// <returns>The created module or null if creation failed</returns>
        public async Task<Module?> AddModule(string moduleName, string moduleCode)
        {
            try
            {
                _logger.LogInformation($"📝 Attempting to add new module: {moduleName} ({moduleCode})");

                // Check if module code already exists
                var existingModule = await _context.Modules
                    .FirstOrDefaultAsync(m => m.ModuleCode == moduleCode);

                if (existingModule != null)
                {
                    _logger.LogWarning($"⚠️ Module code {moduleCode} already exists");
                    return null;
                }

                // Create new module
                var newModule = new Module
                {
                    ModuleName = moduleName,
                    ModuleCode = moduleCode
                };

                _context.Modules.Add(newModule);
                await _context.SaveChangesAsync();

                _logger.LogInformation($"✅ Module successfully added: {moduleName} ({moduleCode}) with ID: {newModule.ModuleID}");
                return newModule;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"❌ Error adding module: {moduleName} ({moduleCode})");
                return null;
            }
        }

        /// <summary>
        /// Updates an existing module
        /// </summary>
        /// <param name="moduleId">The ID of the module to update</param>
        /// <param name="moduleName">The new module name</param>
        /// <param name="moduleCode">The new module code</param>
        /// <returns>The updated module or null if update failed</returns>
        public async Task<Module?> UpdateModule(int moduleId, string moduleName, string moduleCode)
        {
            try
            {
                _logger.LogInformation($"📝 Attempting to update module ID: {moduleId}");

                // Find the existing module
                var existingModule = await _context.Modules
                    .FirstOrDefaultAsync(m => m.ModuleID == moduleId);

                if (existingModule == null)
                {
                    _logger.LogWarning($"⚠️ Module with ID {moduleId} not found");
                    return null;
                }

                // Check if the new module code conflicts with another module
                var codeConflict = await _context.Modules
                    .FirstOrDefaultAsync(m => m.ModuleCode == moduleCode && m.ModuleID != moduleId);

                if (codeConflict != null)
                {
                    _logger.LogWarning($"⚠️ Module code {moduleCode} already exists for another module");
                    return null;
                }

                // Update the module
                existingModule.ModuleName = moduleName;
                existingModule.ModuleCode = moduleCode;

                await _context.SaveChangesAsync();

                _logger.LogInformation($"✅ Module successfully updated: {moduleName} ({moduleCode})");
                return existingModule;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"❌ Error updating module ID: {moduleId}");
                return null;
            }
        }

        /// <summary>
        /// Deletes a module from the system
        /// </summary>
        /// <param name="moduleId">The ID of the module to delete</param>
        /// <returns>True if deletion was successful, false otherwise</returns>
        public async Task<bool> DeleteModule(int moduleId)
        {
            try
            {
                _logger.LogInformation($"🗑️ Attempting to delete module ID: {moduleId}");

                // Find the module
                var moduleToDelete = await _context.Modules
                    .Include(m => m.StudentModules)
                    .Include(m => m.TutorModules)
                    .Include(m => m.Topics)
                    .FirstOrDefaultAsync(m => m.ModuleID == moduleId);

                if (moduleToDelete == null)
                {
                    _logger.LogWarning($"⚠️ Module with ID {moduleId} not found");
                    return false;
                }

                // Check if module has dependencies
                if (moduleToDelete.StudentModules.Any() || 
                    moduleToDelete.TutorModules.Any() || 
                    moduleToDelete.Topics.Any())
                {
                    _logger.LogWarning($"⚠️ Cannot delete module {moduleToDelete.ModuleName} - has associated students, tutors, or topics");
                    return false;
                }

                // Delete the module
                _context.Modules.Remove(moduleToDelete);
                await _context.SaveChangesAsync();

                _logger.LogInformation($"✅ Module successfully deleted: {moduleToDelete.ModuleName} ({moduleToDelete.ModuleCode})");
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"❌ Error deleting module ID: {moduleId}");
                return false;
            }
        }

        /// <summary>
        /// Gets all modules in the system
        /// </summary>
        /// <returns>List of all modules</returns>
        public async Task<List<Module>> GetAllModules()
        {
            try
            {
                _logger.LogInformation("📋 Retrieving all modules");
                return await _context.Modules.ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Error retrieving modules");
                return new List<Module>();
            }
        }

        /// <summary>
        /// Gets a specific module by ID
        /// </summary>
        /// <param name="moduleId">The module ID</param>
        /// <returns>The module or null if not found</returns>
        public async Task<Module?> GetModuleById(int moduleId)
        {
            try
            {
                _logger.LogInformation($"🔍 Retrieving module ID: {moduleId}");
                return await _context.Modules
                    .Include(m => m.StudentModules)
                    .Include(m => m.TutorModules)
                    .Include(m => m.Topics)
                    .FirstOrDefaultAsync(m => m.ModuleID == moduleId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"❌ Error retrieving module ID: {moduleId}");
                return null;
            }
        }

        /// <summary>
        /// Gets a specific module by code
        /// </summary>
        /// <param name="moduleCode">The module code</param>
        /// <returns>The module or null if not found</returns>
        public async Task<Module?> GetModuleByCode(string moduleCode)
        {
            try
            {
                _logger.LogInformation($"🔍 Retrieving module by code: {moduleCode}");
                return await _context.Modules
                    .FirstOrDefaultAsync(m => m.ModuleCode == moduleCode);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"❌ Error retrieving module by code: {moduleCode}");
                return null;
            }
        }
    }
}