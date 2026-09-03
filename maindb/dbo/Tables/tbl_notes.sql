CREATE TABLE [dbo].[tbl_notes] (
    [note_id]    INT           IDENTITY (1, 1) NOT NULL,
    [title]      VARCHAR (100) NOT NULL,
    [content]    TEXT          NULL,
    [created_dt] DATETIME      DEFAULT (getdate()) NOT NULL,
    [updated_dt] DATETIME      DEFAULT (getdate()) NOT NULL,
    [user_id]    INT           NOT NULL,
    PRIMARY KEY CLUSTERED ([note_id] ASC),
    CONSTRAINT [FK_tbl_notes_tbl_auth] FOREIGN KEY ([user_id]) REFERENCES [dbo].[tbl_auth] ([user_id]) ON DELETE CASCADE
);


GO
CREATE NONCLUSTERED INDEX [IX_tbl_notes_user_id]
    ON [dbo].[tbl_notes]([user_id] ASC);

