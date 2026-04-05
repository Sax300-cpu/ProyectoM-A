USE [ClientesBD]
GO
SET IDENTITY_INSERT [dbo].[Clientes] ON 

INSERT [dbo].[Clientes] ([ClienteID], [Cedula], [Nombre], [Apellido], [Direccion], [Telefono], [Email]) VALUES (1, N'0102030405', N'Juan', N'Pérez', N'Av. Siempre Viva 123', N'0999999999', N'juanperez@mail.com')
INSERT [dbo].[Clientes] ([ClienteID], [Cedula], [Nombre], [Apellido], [Direccion], [Telefono], [Email]) VALUES (2, N'1122334455', N'María', N'López', N'Calle Central 45', N'0988888888', N'marialopez@mail.com')
SET IDENTITY_INSERT [dbo].[Clientes] OFF
GO
