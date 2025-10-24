namespace CampusLearn_Web_App.Models
{
	public class Student : User
	{
		public void SubscribeToModule(int moduleId)
		{
			// Create a new StudentModule object to represent the subscription.
			var newSubscription = new StudentModule
			{
				UserID = this.UserID,
				ModuleID = moduleId
			};

			// Add the new subscription to the Student's collection of modules.
			this.StudentModules.Add(newSubscription);
		}

		public void UnSubscribeToModule(int moduleId)
		{
			// Find the subscription to be removed.
			var subscriptionToRemove = this.StudentModules.FirstOrDefault(sm => sm.ModuleID == moduleId);

			// If the subscription exists, remove it from the collection.
			if (subscriptionToRemove != null)
			{
				this.StudentModules.Remove(subscriptionToRemove);
			}
		}
	}
}