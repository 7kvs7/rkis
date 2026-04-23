CREATE TABLE [dbo].[Users] (
    [Id_user]   INT            NOT NULL,
    [Фамилия]   NVARCHAR (MAX) NOT NULL,
    [Имя]       NVARCHAR (MAX) NOT NULL,
    [Отчество] NVARCHAR (MAX) NOT NULL,
    [Логин]     NVARCHAR (MAX) NOT NULL,
    [Пароль]    NVARCHAR (MAX) NOT NULL,
    [Id_role]   INT            NOT NULL,
    CONSTRAINT [PK_Users] PRIMARY KEY CLUSTERED ([Id_user] ASC),
    CONSTRAINT [FK_Users_Role] FOREIGN KEY ([Id_role]) REFERENCES [dbo].[Role] ([Id_role])
);
