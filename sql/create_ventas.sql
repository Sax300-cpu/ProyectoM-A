USE [VentasBD]
GO
/****** Object:  Table [dbo].[DetalleVenta]    Script Date: 4/4/2026 19:15:45 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[DetalleVenta](
	[DetalleID] [int] IDENTITY(1,1) NOT NULL,
	[VentaID] [int] NOT NULL,
	[ProductoID] [int] NOT NULL,
	[Cantidad] [int] NOT NULL,
	[PrecioUnitario] [decimal](10, 2) NOT NULL,
	[Subtotal] [decimal](10, 2) NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[DetalleID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Productos]    Script Date: 4/4/2026 19:15:45 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Productos](
	[ProductoID] [int] IDENTITY(1,1) NOT NULL,
	[Nombre] [nvarchar](100) NOT NULL,
	[Precio] [decimal](10, 2) NOT NULL,
	[Stock] [int] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[ProductoID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Ventas]    Script Date: 4/4/2026 19:15:45 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Ventas](
	[VentaID] [int] IDENTITY(1,1) NOT NULL,
	[ClienteID] [int] NOT NULL,
	[Fecha] [datetime] NOT NULL,
	[Subtotal] [decimal](10, 2) NOT NULL,
	[IVA] [decimal](10, 2) NOT NULL,
	[Total] [decimal](10, 2) NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[VentaID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
ALTER TABLE [dbo].[Ventas] ADD  DEFAULT (getdate()) FOR [Fecha]
GO
ALTER TABLE [dbo].[DetalleVenta]  WITH CHECK ADD  CONSTRAINT [chk_cantidad] CHECK  (([Cantidad]>(0)))
GO
ALTER TABLE [dbo].[DetalleVenta] CHECK CONSTRAINT [chk_cantidad]
GO
ALTER TABLE [dbo].[DetalleVenta]  WITH CHECK ADD  CONSTRAINT [chk_precio] CHECK  (([PrecioUnitario]>(0)))
GO
ALTER TABLE [dbo].[DetalleVenta] CHECK CONSTRAINT [chk_precio]
GO
ALTER TABLE [dbo].[DetalleVenta]  WITH CHECK ADD  CONSTRAINT [chk_subtotal] CHECK  (([Subtotal]>=(0)))
GO
ALTER TABLE [dbo].[DetalleVenta] CHECK CONSTRAINT [chk_subtotal]
GO
ALTER TABLE [dbo].[Ventas]  WITH CHECK ADD  CONSTRAINT [chk_iva] CHECK  (([IVA]>=(0)))
GO
ALTER TABLE [dbo].[Ventas] CHECK CONSTRAINT [chk_iva]
GO
ALTER TABLE [dbo].[Ventas]  WITH CHECK ADD  CONSTRAINT [chk_subtotal_ventas] CHECK  (([Subtotal]>=(0)))
GO
ALTER TABLE [dbo].[Ventas] CHECK CONSTRAINT [chk_subtotal_ventas]
GO
ALTER TABLE [dbo].[Ventas]  WITH CHECK ADD  CONSTRAINT [chk_total] CHECK  (([Total]>=(0)))
GO
ALTER TABLE [dbo].[Ventas] CHECK CONSTRAINT [chk_total]
GO
ALTER TABLE [dbo].[Ventas]  WITH CHECK ADD  CONSTRAINT [chk_total_formula] CHECK  (([Total]=([Subtotal]+[IVA])))
GO
ALTER TABLE [dbo].[Ventas] CHECK CONSTRAINT [chk_total_formula]
GO
