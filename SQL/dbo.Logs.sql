CREATE TABLE [dbo].[Logs] (
    [Id]         INT        NOT NULL,
    [Time_ms]    REAL       NOT NULL,
    [StateAfter] TINYINT    NOT NULL,
    [MsgClass]   SMALLINT   NOT NULL,
	[MsgNumber] SMALLINT NOT NULL,
    [TimeString] DATETIME   NOT NULL,
    [MsgText]    NCHAR (50) NOT NULL,
    PRIMARY KEY CLUSTERED ([Id] ASC)
);

