USE [VentasBD]
GO
SET IDENTITY_INSERT [dbo].[Ventas] ON 

INSERT [dbo].[Ventas] ([VentaID], [ClienteID], [Fecha], [Subtotal], [IVA], [Total]) VALUES (1, 1, CAST(N'2026-04-04T17:34:46.280' AS DateTime), CAST(825.00 AS Decimal(10, 2)), CAST(99.00 AS Decimal(10, 2)), CAST(924.00 AS Decimal(10, 2)))
SET IDENTITY_INSERT [dbo].[Ventas] OFF
GO