USE [CuentaDb]
GO
/****** Object:  Table [dbo].[__EFMigrationsHistory]    Script Date: 12/10/2024 18:37:06 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[__EFMigrationsHistory](
	[MigrationId] [nvarchar](150) NOT NULL,
	[ProductVersion] [nvarchar](32) NOT NULL,
 CONSTRAINT [PK___EFMigrationsHistory] PRIMARY KEY CLUSTERED 
(
	[MigrationId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Cuentas]    Script Date: 12/10/2024 18:37:06 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Cuentas](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[NumeroCuenta] [nvarchar](max) NOT NULL,
	[TipoCuenta] [nvarchar](max) NOT NULL,
	[SaldoInicial] [decimal](18, 2) NOT NULL,
	[Estado] [nvarchar](max) NOT NULL,
	[ClienteId] [nvarchar](max) NOT NULL,
 CONSTRAINT [PK_Cuentas] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Movimientos]    Script Date: 12/10/2024 18:37:06 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Movimientos](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Fecha] [datetime2](7) NOT NULL,
	[TipoMovimiento] [nvarchar](max) NOT NULL,
	[Valor] [decimal](18, 2) NOT NULL,
	[Saldo] [decimal](18, 2) NOT NULL,
	[CuentaId] [int] NOT NULL,
 CONSTRAINT [PK_Movimientos] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
INSERT [dbo].[__EFMigrationsHistory] ([MigrationId], [ProductVersion]) VALUES (N'20241011142703_InitialMigration', N'8.0.10')
INSERT [dbo].[__EFMigrationsHistory] ([MigrationId], [ProductVersion]) VALUES (N'20241011143044_UpdateDecimalPrecision', N'8.0.10')
INSERT [dbo].[__EFMigrationsHistory] ([MigrationId], [ProductVersion]) VALUES (N'20241012153950_CambiarClienteIdACadena', N'8.0.10')
GO
SET IDENTITY_INSERT [dbo].[Cuentas] ON 

INSERT [dbo].[Cuentas] ([Id], [NumeroCuenta], [TipoCuenta], [SaldoInicial], [Estado], [ClienteId]) VALUES (4, N'123456', N'Ahorro', CAST(2882.00 AS Decimal(18, 2)), N'Activa', N'e58c609a-02f2-454d-a85b-6b0c22ae7eea')
SET IDENTITY_INSERT [dbo].[Cuentas] OFF
GO
SET IDENTITY_INSERT [dbo].[Movimientos] ON 

INSERT [dbo].[Movimientos] ([Id], [Fecha], [TipoMovimiento], [Valor], [Saldo], [CuentaId]) VALUES (1, CAST(N'2024-10-12T23:27:56.8027099' AS DateTime2), N'Deposito', CAST(1000.00 AS Decimal(18, 2)), CAST(3000.00 AS Decimal(18, 2)), 4)
INSERT [dbo].[Movimientos] ([Id], [Fecha], [TipoMovimiento], [Valor], [Saldo], [CuentaId]) VALUES (2, CAST(N'2024-10-12T23:28:09.6977567' AS DateTime2), N'Deposito', CAST(50.00 AS Decimal(18, 2)), CAST(3050.00 AS Decimal(18, 2)), 4)
INSERT [dbo].[Movimientos] ([Id], [Fecha], [TipoMovimiento], [Valor], [Saldo], [CuentaId]) VALUES (3, CAST(N'2024-10-12T23:28:31.5442650' AS DateTime2), N'Retiro', CAST(168.00 AS Decimal(18, 2)), CAST(2882.00 AS Decimal(18, 2)), 4)
SET IDENTITY_INSERT [dbo].[Movimientos] OFF
GO
ALTER TABLE [dbo].[Movimientos]  WITH CHECK ADD  CONSTRAINT [FK_Movimientos_Cuentas_CuentaId] FOREIGN KEY([CuentaId])
REFERENCES [dbo].[Cuentas] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[Movimientos] CHECK CONSTRAINT [FK_Movimientos_Cuentas_CuentaId]
GO
