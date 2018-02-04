CREATE TABLE [dbo].[Table]
(
	[Id] INT NOT NULL PRIMARY KEY, 
    [Time_ms] REAL NULL, 
    [StateAfter] TINYINT NULL, 
    [MsgClass] SMALLINT NULL, 
    [TimeString] DATETIME NULL, 
    [MsgText] NCHAR(50) NULL
)
