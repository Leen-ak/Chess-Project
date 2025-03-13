use master;
GO
Alter database ChessDatabase set single_user with rollback immediate;
GO
Drop Database ChessDatabase;
GO
Create Database ChessDatabase;
GO
use ChessDatabase;
GO

CREATE TABLE UserInfo(
Id INT IDENTITY,
Firstname NVARCHAR(60) NOT NULL,
Lastname NVARCHAR(60) NOT NULL,
UserName NVARCHAR(60) NOT NULL,
Email NVARCHAR(100) NOT NULL,
Password NVARCHAR(MAX) NOT NULL,
Picture VARBINARY(MAX), 
Timer ROWVERSION,
CONSTRAINT PK_UserID PRIMARY KEY(Id)
);
GO
CREATE UNIQUE INDEX IX_UserInfo_UserName ON UserInfo(UserName);
GO
CREATE UNIQUE INDEX IX_UserInfo_Email ON UserInfo(Email);
GO


CREATE TABLE Account(
Id INT IDENTITY,
UserID INT,
Username NVARCHAR(60) NOT NULL,
Password NVARCHAR(60) NOT NULL,
Timer ROWVERSION,
CONSTRAINT PK_UserAccountID PRIMARY KEY(Id),
CONSTRAINT FK_UserID_ FOREIGN KEY(UserID) REFERENCES UserInfo(Id)
);
GO
CREATE INDEX IX_Account_Username ON Account(Username);
GO
CREATE INDEX IX_Account_Password on Account(Password); 
GO

CREATE TABLE Followers(
Id INT IDENTITY,
FollowerId INT,
FollowingId INT,
Status NVARCHAR(30),
Timer ROWVERSION,
CONSTRAINT PK_Id PRIMARY KEY (Id),
CONSTRAINT FK_Follower FOREIGN KEY (FollowerId) REFERENCES UserInfo(Id),
CONSTRAINT FK_Following FOREIGN KEY (FollowingId) REFERENCES UserInfo(Id),
CONSTRAINT UQ_Follow UNIQUE (FollowerId, FollowingId)
);
GO
CREATE INDEX IX_FollowerId on Followers (FollowerId);
GO
CREATE INDEX IX_FollowingId on Followers (FollowingId);
GO
CREATE INDEX IX_Status on Followers (Status);
GO

CREATE TABLE PasswordResetToken(
Id INT IDENTITY,
UserId INT NOT NULL,
ResetToken NVARCHAR(255) NOT NULL,
RestTokenExpiry DATETIME NOT NULL,
Timer ROWVERSION,
CONSTRAINT PI_Id PRIMARY KEY (Id),
CONSTRAINT FK_PasswordResetTokens_User FOREIGN KEY (UserId) REFERENCES UserInfo(Id) ON DELETE CASCADE  
);
GO

/* Create a stored procedure to retrieve all the username that is in the databse */
CREATE PROCEDURE dbo.GetAllUserName
AS
BEGIN
	Select UserName
	from UserInfo;
End;
GO


SELECT COUNT (*) FROM Followers WHERE Status = 'Pending'; 

EXEC dbo.GetAllUserName;
GO

SELECT * FROM dbo.Followers;
GO

SELECT * FROM UserInfo;
GO

SELECT * FROM PasswordResetToken;
GO


/*DELETE from UserInfo where Id = 1;*/

