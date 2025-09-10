
-- Create User table
CREATE TABLE "User" (
    userID SERIAL PRIMARY KEY,
    firstName VARCHAR(50) NOT NULL,
    lastName VARCHAR(50) NOT NULL,
    email VARCHAR(100) UNIQUE NOT NULL,
    passwordHash VARCHAR(255) NOT NULL,
    role VARCHAR(10) CHECK (role IN ('Student', 'Tutor', 'Admin')) NOT NULL
);

-- Create Module table
CREATE TABLE Module (
    moduleID SERIAL PRIMARY KEY,
    moduleName VARCHAR(100) NOT NULL,
    moduleCode VARCHAR(20) UNIQUE NOT NULL
);

-- Create StudentModule table
CREATE TABLE StudentModule (
    studentModuleID SERIAL PRIMARY KEY,
    userID INTEGER NOT NULL,
    moduleID INTEGER NOT NULL,
    FOREIGN KEY (userID) REFERENCES "User"(userID) ON DELETE CASCADE,
    FOREIGN KEY (moduleID) REFERENCES Module(moduleID) ON DELETE CASCADE
);

-- Create TutorModule table
CREATE TABLE TutorModule (
    tutorModuleID SERIAL PRIMARY KEY,
    userID INTEGER NOT NULL,
    moduleID INTEGER NOT NULL,
    FOREIGN KEY (userID) REFERENCES "User"(userID) ON DELETE CASCADE,
    FOREIGN KEY (moduleID) REFERENCES Module(moduleID) ON DELETE CASCADE
);

-- Create Topic table
CREATE TABLE Topic (
    topicID SERIAL PRIMARY KEY,
    title VARCHAR(150) NOT NULL,
    description TEXT,
    creationDate DATE NOT NULL,
    userID INTEGER NOT NULL,
    moduleID INTEGER NOT NULL,
    FOREIGN KEY (userID) REFERENCES "User"(userID) ON DELETE CASCADE,
    FOREIGN KEY (moduleID) REFERENCES Module(moduleID) ON DELETE CASCADE
);

-- Create LearningMaterial table
CREATE TABLE LearningMaterial (
    materialID SERIAL PRIMARY KEY,
    fileName VARCHAR(150),
    fileType VARCHAR(50),
    uploadDate DATE,
    topicID INTEGER NOT NULL,
    FOREIGN KEY (topicID) REFERENCES Topic(topicID) ON DELETE CASCADE
);

-- Create Message table
CREATE TABLE Message (
    messageID SERIAL PRIMARY KEY,
    senderID INTEGER NOT NULL,
    receiverID INTEGER NOT NULL,
    content TEXT NOT NULL,
    sentDate TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    FOREIGN KEY (senderID) REFERENCES "User"(userID) ON DELETE CASCADE,
    FOREIGN KEY (receiverID) REFERENCES "User"(userID) ON DELETE CASCADE
);

-- Create Notification table
CREATE TABLE Notification (
    notificationID SERIAL PRIMARY KEY,
    message VARCHAR(255) NOT NULL,
    createdAt TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    userID INTEGER NOT NULL,
    FOREIGN KEY (userID) REFERENCES "User"(userID) ON DELETE CASCADE
);

-- Create indexes for better performance
CREATE INDEX idx_user_email ON "User"(email);
CREATE INDEX idx_user_role ON "User"(role);
CREATE INDEX idx_module_code ON Module(moduleCode);
CREATE INDEX idx_student_module_user ON StudentModule(userID);
CREATE INDEX idx_student_module_module ON StudentModule(moduleID);
CREATE INDEX idx_tutor_module_user ON TutorModule(userID);
CREATE INDEX idx_tutor_module_module ON TutorModule(moduleID);
CREATE INDEX idx_topic_user ON Topic(userID);
CREATE INDEX idx_topic_module ON Topic(moduleID);
CREATE INDEX idx_topic_creation_date ON Topic(creationDate);
CREATE INDEX idx_learning_material_topic ON LearningMaterial(topicID);
CREATE INDEX idx_message_sender ON Message(senderID);
CREATE INDEX idx_message_receiver ON Message(receiverID);
CREATE INDEX idx_message_sent_date ON Message(sentDate);
CREATE INDEX idx_notification_user ON Notification(userID);
CREATE INDEX idx_notification_created_at ON Notification(createdAt);
