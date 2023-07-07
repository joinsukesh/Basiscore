/*CREATES AUDIT TABLE FOR CMS ITEMS*/
CREATE TABLE [dbo].[Basiscore_Cms_Audit_Items](
    [RowId] [bigint] IDENTITY(1,1) NOT NULL,
    [ItemId] [uniqueidentifier] NULL,
    [ItemName] [varchar](300) NULL,
	[ItemPath] [varchar](2000) NULL,
	[TemplateId] [uniqueidentifier] NULL,
	[ItemLanguage] [varchar](50) NULL,
	[ItemVersion] [int] NULL,
	[Event] [varchar](100) NULL,
	[ActionedBy] [varchar](100) NULL,
	[ItemDataBeforeSave] [nvarchar](MAX) NULL,	
	[ItemDataAfterSave] [nvarchar](MAX) NULL,	
	[Comments] [nvarchar](4000) NULL,
    [LoggedTime] [datetime] NULL,
 CONSTRAINT [PK_Basiscore_Cms_Audit_Items] PRIMARY KEY CLUSTERED 
(
    [RowId] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON)
) ON [PRIMARY]
GO

/*CREATES STORED PROCEDURE WHICH INSERTS CMS ITEM AUDIT LOG*/
CREATE PROCEDURE usp_Basiscore_InsertCmsItemAuditLog
	@ItemId uniqueidentifier,
    @ItemName varchar(300),
	@ItemPath varchar(2000),
	@TemplateId uniqueidentifier,
	@ItemLanguage varchar(50),
	@ItemVersion int,
	@Event varchar(100),
	@ActionedBy varchar(100),
	@ItemDataBeforeSave nvarchar(MAX),	
	@ItemDataAfterSave nvarchar(MAX),	
	@Comments nvarchar(4000),
    @LoggedTime datetime
AS
BEGIN
	SET NOCOUNT ON;

    INSERT INTO Basiscore_Cms_Audit_Items(
		ItemId,
		ItemName,
		ItemPath,
		TemplateId,
		ItemLanguage,
		ItemVersion,
		[Event],
		ActionedBy,
		ItemDataBeforeSave,
		ItemDataAfterSave,
		Comments,
		LoggedTime
	)
	VALUES(
		@ItemId,
		@ItemName,
		@ItemPath,
		@TemplateId,
		@ItemLanguage,
		@ItemVersion,
		@Event,
		@ActionedBy,
		@ItemDataBeforeSave,
		@ItemDataAfterSave,
		@Comments,
		@LoggedTime
	)
END
GO

