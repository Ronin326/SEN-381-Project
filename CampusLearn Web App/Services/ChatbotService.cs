using System;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace CampusLearn_Web_App.Services
{
    public class ChatbotService
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _config;
        private readonly Dictionary<string, object> _appKnowledge;

        public ChatbotService(HttpClient httpClient, IConfiguration config)
        {
            _httpClient = httpClient;
            _config = config;

            // Load your dataset (app_knowledge.json) into memory
            var knowledgePath = Path.Combine(Directory.GetCurrentDirectory(), "Data", "appKnowledge.json");

            if (File.Exists(knowledgePath))
            {
                var jsonData = File.ReadAllText(knowledgePath);
                _appKnowledge = JsonSerializer.Deserialize<Dictionary<string, object>>(jsonData)
                    ?? new Dictionary<string, object>();
            }
            else
            {
                Console.WriteLine("⚠️ Warning: appKnowledge.json not found in /Data folder.");
                _appKnowledge = new Dictionary<string, object>();
            }
        }

        public async Task<string> GetChatResponseAsync(string userMessage)
        {
            var apiKey = _config["Chatbot:ApiKey"];
            var model = _config["Chatbot:Model"];
            var endpoint = _config["Chatbot:Endpoint"];

            _httpClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", apiKey);

            // Combine dataset + user question into context prompt
            var datasetSummary = JsonSerializer.Serialize(_appKnowledge, new JsonSerializerOptions
            {
                WriteIndented = true
            });

            var prompt = $@"
You are a helpful assistant for the CampusLearn platform.
Your role is to help users understand how to use the app — how navigation, roles, dashboards, and features work.
Here is the UI outline of the webapp to give you some background context:
SEN 381 UI Outline:
Student:
1. Login and Registration Page
• Visuals: Clean, modern single-page layout with the CampusLearn™ logo prominently displayed at the top. The background should be simple, possibly with a subtle image of the campus or a relevant graphic.
• Forms:
o Login: Fields for Email and Password. The email field should have validation to ensure the @belgiumcampus.ac.za domain. A ""Forgot Password?"" link should also be available.
o Register: Fields for First Name, Last Name, Student ID, Email (with the same domain validation), and Password (with a ""Confirm Password"" field). The UI should provide real-time feedback on password strength.
• Buttons: Clear and distinct ""Log In"" and ""Register"" buttons.
2. Dashboard
• Top Navigation Bar:
o Logo: On the far left. A clickable link to return to the dashboard.
o Main Navigation: Links for Dashboard, My Courses, and Chats. These links should be styled to indicate the active page.
o Notifications: A bell icon with a notification count (e.g., (3)). Clicking it reveals a compact, scrollable dropdown panel.
▪ Dropdown Content: Displays a list of recent, unread notifications in chronological order. Each notification is a short line of text (e.g., ""New post in Software Engineering forum""). A ""Mark all as read"" button should be available at the bottom.
o User Profile: A user icon or profile picture.
▪ Dropdown Content: A dropdown menu with links to ""Profile Settings"" and ""Logout"".
• Main Content Area:
o Notifications Panel (Top Half): This is a dedicated, visually prominent section. It can be a card or a container with a title like ""Recent Activity"" or ""Your Notifications.""
▪ Content: This section contains the full, non-compacted notifications, perhaps with more detail and a link to the relevant page (e.g., ""Tutor John Doe replied to your question in Business Management. View Post"").
o My Courses Grid (Bottom Half): A responsive grid of cards, each representing a course the student is subscribed to. This section should have a title like ""My Courses"" or ""Your Subscriptions.""
▪ Course Card: Each card will display:
▪ Course Name (e.g., ""Software Engineering"")
▪ Course Code (e.g., ""SEN 381"")
▪ Small, actionable icons indicating new activity (e.g., a small chat icon with (2) for new forum posts, a document icon for new shared files).
▪ Clicking a card navigates to the detailed course page.
3. My Courses Page
• Top Navigation Bar: The same consistent navigation bar as the dashboard.
• Main Content Area:
o Subscribe Button: A clear, call-to-action button at the top of the page (e.g., ""Subscribe to a New Course""). Clicking it opens a modal or navigates to a course directory page.
o Course Directory: A page listing all available subjects with a search bar and filters (e.g., by faculty, by module code). Each subject on this page would have a ""Subscribe"" button.
o Course Grid: A grid of cards similar to the dashboard, but for all subscribed courses.
• Individual Course Page (Post-Click):
o Header: Displays the course name and code.
o Tutor List: A list of all tutors for this course, with their names and a small ""Message"" button to initiate a private chat.
o Side Navigation Panel: A collapsible side panel on the left with links to:
▪ Public Forum: Displays a list of forum posts for this specific course. Each post shows the title, author (can be anonymous), and a timestamp. Clicking a post opens the full discussion.
▪ Shared Documents: A list or gallery of all uploaded learning materials for this course, categorized by type (e.g., ""Videos,"" ""PDFs,"" ""Notes""). Each item is a clickable link to view or download the resource.
4. Chats Panel
• Navigation: Accessible via the ""Chats"" link in the top navigation bar.
• Visuals: A full-page, Teams-like layout.
o Left Pane (Chat List): A list of all chats, including private messages with tutors and group channels for public forums. Each list item will show the chat name (e.g., ""Tutor: John Doe,"" ""Public: Software Engineering"") and a small indicator for unread messages.
o Main Pane (Chat Window): The main chat window where messages are exchanged. It will include a message input box, an emoji/file attachment button, and a scrollable message history.
• Functionality:
o Private Chats: Students can initiate a private chat with a tutor from the individual course page.
o Public Channels: The public forums are now integrated as group channels within this panel. Students can view and participate in subject-specific discussions. The option to ""Post Anonymously"" should be available within these channels.
o Peer-to-Peer Chats: The chat hub should have a feature (e.g., ""New Chat"" button) that allows students to search for and start a private chat with another student.
5. Chatbot
• UI Placement: A small, floating chat icon will be persistently visible on the bottom right of every page, except perhaps the login page.
• Functionality: Clicking the icon expands a small chat window. This bot will handle FAQs, offer navigation tips, and, most importantly, escalate a query to a human tutor when it cannot provide a confident answer. The chatbot's last message to the student in this case will be, ""I've passed your query on to a tutor for this subject.""
Peer Tutor:
1. Login and Registration Page
• Confirmation: The login and registration process will be the same as for students. Tutors will also register using their @belgiumcampus.ac.za email. The system will likely have an internal process or an admin-side function to elevate a registered student's account to a tutor role.
2. Dashboard
• Top Navigation Bar:
o The navigation bar remains identical to the student's: Logo, Dashboard, My Courses, Chats, Notifications, and User Profile. This ensures a seamless transition between roles if a user is both a student and a tutor.
• Main Content Area:
o Notifications Panel (Top Half): This panel will be tailored to a tutor's needs, displaying alerts related to their tutoring responsibilities.
▪ Content: Notifications will include:
▪ ""New question posted in the Software Engineering public forum.""
▪ ""Student Jane Smith has sent you a new private message.""
▪ ""A question has been escalated to you from the AI Chatbot for Business Management.""
▪ ""A student has subscribed to your IT Security course.""
o My Courses Grid (Bottom Half): This grid will be split into two sections as you proposed.
▪ Tutoring Courses: A list or grid of courses they are registered to tutor. Each card would highlight key information like the number of open student queries or new forum posts.
▪ Subscribed Courses: A list of courses they are subscribed to as a student for their own academic needs. This allows the tutor to use the platform as a student as well.
3. My Courses Page
• Top Navigation Bar: Consistent with the dashboard.
• Main Content Area:
o ""Add a Tutoring Course"" Button: This button will replace the ""Subscribe to a New Subject"" button from the student UI. Clicking it will open a modal or page where the tutor can select from a list of courses they are approved to assist with. Once a course is added, the system updates the tutor list for that course.
o Course Grid: This page will be divided into the two distinct sections you outlined:
▪ Tutoring Courses: A grid of courses where the tutor can manage content and student interactions.
▪ Subscribed Courses: A grid of courses they are enrolled in as a student.
• Individual Tutoring Course Page (Post-Click):
o Header: Displays the course name and code.
o Tutor Management Section: This page will contain tutor-specific functionality.
▪ Tutor List: Displays a list of all tutors for that course.
▪ ""Upload Document"" Button: A clear call-to-action to upload a new video, PDF, or other learning material.
o Side Navigation Panel: This panel will be the same as the student's, but the tutor's actions will be different:
▪ Public Forum: Tutors can respond to posts and provide detailed feedback.
▪ Shared Documents: Tutors can upload and manage the learning resources for this course.
4. Chats Panel
• Confirmation: The chat panel remains identical, simulating the look and functionality of Teams.
• Functionality: Tutors will use the same unified chat interface to:
o Private Chats: Respond to one-on-one chats initiated by students.
o Public Channels: Participate in the public forums for the courses they tutor, providing expert answers and guidance.
o Peer-to-Peer Chats: They can also use this feature to chat with other peer tutors or students.
5. Chatbot
• Confirmation: The floating chatbot icon and its basic functionality remain the same for the tutor, as they might also have questions about platform navigation or FAQs. The key difference is that when the chatbot escalates a query, it will now route it to the appropriate tutor.
Admin:
1. Login Page
• Admins will use the same login page, but their accounts will have a specific Admin role assigned in the database. The system will recognize this role and redirect them to the Admin Dashboard upon successful login.
2. Main Dashboard (Admin Panel)
• Top Navigation Bar: This will be a distinct navigation bar for admins.
o Logo: The CampusLearn™ logo on the left.
o Navigation Links:
▪ Dashboard: The main landing page with system metrics.
▪ Users: To manage student and tutor accounts.
▪ Courses: To manage the academic subjects and associated content.
▪ Moderation: To handle flagged content in the public forums.
▪ Analytics: To view detailed reports on platform usage.
o User Profile: A user icon on the right with a dropdown for Profile Settings and Logout.
• Main Content Area:
o The main panel will be a comprehensive dashboard showing a snapshot of the platform's health. It will feature widgets and charts for key metrics.
o Key Widgets:
▪ User Summary: A quick count of Total Students, Total Tutors, and New Registrations Today.
▪ Forum Activity: A chart showing forum posts and replies over time.
▪ Content Uploads: A metric showing the number of new documents or resources uploaded in the past week.
▪ System Status: A panel showing the status of integrated services, like the API notifications (e.g., ""Twilio API: Online"").
▪ Pending Actions: A list of items requiring immediate attention, such as New Tutor Applications or Flagged Forum Posts.
3. User Management Page
• Functionality: This page is the central hub for managing all users on the platform.
• Layout: A searchable and filterable table of all user accounts.
• Table Columns: Name, Email, Role (Student, Tutor, Admin), Account Status (Active, Suspended), Last Login.
• Actions: Admins can click on a user's name to view their profile and perform actions like:
o Approve Tutor Application: Elevate a student's role to a tutor.
o Suspend/Ban User: Disable an account.
o Reset Password: For user support.
4. Courses Management Page
• Functionality: This page allows admins to control the courses available on the platform.
• Layout: A list of all academic subjects.
• Actions: Admins can:
o Add/Edit/Delete Courses: Manage the subjects available to students and tutors.
o Assign Tutors: Manually assign a tutor to a specific course.
o View Course Content: Review the shared documents and forum posts for a specific course.
5. Moderation Page
• Functionality: This is for maintaining a safe and respectful environment.
• Layout: A queue of flagged content, such as forum posts or messages.
• Table Columns: Content, Reason for Flag, User who Flagged, Date.
• Actions: Admins can review flagged content and take action:
o Delete Content: Remove the post or message.
o Send Warning: Notify the user about their violation.
o Suspend User: Take action against repeat offenders.
6. Analytics and Reporting Page
• Functionality: Provides detailed insights into platform usage.
• Layout: A dashboard of interactive charts and graphs.
• Metrics to Display:
o User Growth: Charts showing student and tutor registration over time.
o Engagement: Metrics like active users per day, average session duration, and tutor response times.
o Content Popularity: A list of the most viewed shared documents or most active public forums.
Use ONLY the information in the dataset below to answer questions. 
If the dataset doesn’t contain the answer, politely say you don’t have that information.

DATASET:
{datasetSummary}

USER QUESTION:
{userMessage}
";

            var requestBody = new
            {
                model = model,
                messages = new[]
                {
                    new { role = "system", content = "You are a CampusLearn assistant trained on the app dataset." },
                    new { role = "user", content = prompt }
                }
            };

            var json = JsonSerializer.Serialize(requestBody);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync(endpoint, content);
            var responseString = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                Console.WriteLine($"❌ Chatbot API error: {response.StatusCode} - {responseString}");
                return $"⚠️ Error: Chatbot API returned {response.StatusCode}";
            }

            using var doc = JsonDocument.Parse(responseString);

            // Handle OpenAI and Hugging Face response formats
            if (doc.RootElement.TryGetProperty("choices", out var choices))
            {
                // OpenAI response
                return choices[0].GetProperty("message").GetProperty("content").GetString()?.Trim()
                       ?? "⚠️ Empty response.";
            }
            else if (doc.RootElement.ValueKind == JsonValueKind.Array &&
                     doc.RootElement[0].TryGetProperty("generated_text", out var generated))
            {
                // Hugging Face response
                return generated.GetString()?.Trim() ?? "⚠️ Empty response.";
            }

            return "⚠️ Unexpected response format from chatbot API.";
        }
    }
}
