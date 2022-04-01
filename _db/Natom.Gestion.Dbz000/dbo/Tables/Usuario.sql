CREATE TABLE [dbo].[Usuario] (
    [UsuarioId]                  INT           IDENTITY (1, 1) NOT NULL,
    [Nombre]                     VARCHAR (50)  NOT NULL,
    [Apellido]                   VARCHAR (50)  NOT NULL,
    [Email]                      VARCHAR (150) NOT NULL,
    [Clave]                      VARCHAR (32)  NULL,
    [FechaHoraConfirmacionEmail] DATETIME      NULL,
    [SecretConfirmacion]         CHAR (32)     NULL,
    [FechaHoraAlta]              DATETIME      NOT NULL,
    [FechaHoraBaja]              DATETIME      DEFAULT (NULL) NULL,
    PRIMARY KEY CLUSTERED ([UsuarioId] ASC)
);

