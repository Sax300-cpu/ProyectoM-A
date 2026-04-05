USE [VentasBD]
GO
SET IDENTITY_INSERT [dbo].[Productos] ON 

INSERT [dbo].[Productos] ([ProductoID], [Nombre], [Precio], [Stock]) VALUES (1, N'Laptop Dell', CAST(800.00 AS Decimal(10, 2)), 10)
INSERT [dbo].[Productos] ([ProductoID], [Nombre], [Precio], [Stock]) VALUES (2, N'Mouse Logitech', CAST(25.00 AS Decimal(10, 2)), 50)
INSERT [dbo].[Productos] ([ProductoID], [Nombre], [Precio], [Stock]) VALUES (3, N'Teclado Mecánico', CAST(60.00 AS Decimal(10, 2)), 30)
SET IDENTITY_INSERT [dbo].[Productos] OFF
GO