CREATE TABLE [dbo].[tbl_auth] (
    [user_id]      INT           IDENTITY (1, 1) NOT NULL,
    [userName]     VARCHAR (50)  NOT NULL,
    [email]        VARCHAR (255) NOT NULL,
    [userPassword] VARCHAR (255) NOT NULL,
    [is_active]    BIT           DEFAULT ((1)) NOT NULL,
    [created_dt]   DATETIME      DEFAULT (getdate()) NOT NULL,
    [updated_dt]   DATETIME      DEFAULT (getdate()) NOT NULL,
    PRIMARY KEY CLUSTERED ([user_id] ASC)
);

