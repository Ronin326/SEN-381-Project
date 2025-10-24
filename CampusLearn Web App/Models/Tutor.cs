namespace CampusLearn_Web_App.Models
{
	public class Tutor : User
	{
		// Tutor-specific methods
		public void AddToModule(int moduleId)
		{
			// Create a new TutorModule object to represent the module assignment.
			var newAssignment = new TutorModule
			{
				UserID = this.UserID,
				ModuleID = moduleId
			};

			// Add the new assignment to the Tutor's collection of modules.
			this.TutorModules.Add(newAssignment);
		}

		public void RemoveFromModule(int moduleId)
		{
			// Find the assignment to be removed.
			var assignmentToRemove = this.TutorModules.FirstOrDefault(tm => tm.ModuleID == moduleId);

			// If the assignment exists, remove it from the collection.
			if (assignmentToRemove != null)
			{
				this.TutorModules.Remove(assignmentToRemove);
			}
		}
	}
}