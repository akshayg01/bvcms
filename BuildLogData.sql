CREATE TABLE [dbo].[RequestLog](
	[dbname] [varchar](50) NULL,
	[dt] [datetime] NOT NULL,
	[userid] [varchar](50) NOT NULL,
	[method] [varchar](10) NULL,
	[controller] [varchar](25) NOT NULL,
	[action] [varchar](25) NOT NULL,
	[duration] [float] NOT NULL
) ON [PRIMARY]
GO
CREATE PROCEDURE [dbo].[LogRequest](
	@dbname VARCHAR(20),
	@method VARCHAR(10), 
	@controller VARCHAR(25), 
	@action VARCHAR(25), 
	@userid VARCHAR(50),
	@duration FLOAT
	)
AS
BEGIN
	SET NOCOUNT ON;
	INSERT INTO dbo.RequestLog
	        ( dbname, dt, method, controller, [action], userid, duration)
	VALUES  ( @dbname, GETDATE(), @method, @controller, @action, @userid, @duration)
	END
GO
