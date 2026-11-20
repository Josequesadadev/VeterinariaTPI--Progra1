USE [master]
GO

IF NOT EXISTS (SELECT name FROM sys.databases WHERE name = N'db_veterinaria')
BEGIN
    CREATE DATABASE [db_veterinaria];
END
GO

USE [db_veterinaria]
GO

IF OBJECT_ID('dbo.Atenciones', 'U')    IS NOT NULL DROP TABLE dbo.Atenciones;
IF OBJECT_ID('dbo.Mascotas', 'U')      IS NOT NULL DROP TABLE dbo.Mascotas;
IF OBJECT_ID('dbo.Clientes', 'U')      IS NOT NULL DROP TABLE dbo.Clientes;
IF OBJECT_ID('dbo.TiposMascota', 'U')  IS NOT NULL DROP TABLE dbo.TiposMascota;
IF OBJECT_ID('dbo.TiposAtencion', 'U') IS NOT NULL DROP TABLE dbo.TiposAtencion;
IF OBJECT_ID('dbo.Usuarios', 'U')      IS NOT NULL DROP TABLE dbo.Usuarios;
GO

-- ===== Tablas =====

CREATE TABLE dbo.Usuarios (
    id_usuario      INT IDENTITY(1,1) PRIMARY KEY,
    nombre_usuario  VARCHAR(50)  NOT NULL UNIQUE,
    email           VARCHAR(100) NOT NULL UNIQUE,
    password_hash   VARCHAR(255) NOT NULL,
    rol             VARCHAR(20)  NOT NULL DEFAULT 'Empleado' CHECK (rol IN ('Admin', 'Empleado')),
    fecha_alta      DATETIME     NOT NULL DEFAULT GETDATE(),
    activo          BIT          NOT NULL DEFAULT 1
);
GO

CREATE TABLE dbo.TiposMascota (
    id_tipo_mascota INT IDENTITY(1,1) PRIMARY KEY,
    nombre          VARCHAR(50) NOT NULL UNIQUE,
    activo          BIT         NOT NULL DEFAULT 1
);
GO

CREATE TABLE dbo.TiposAtencion (
    id_tipo_atencion INT IDENTITY(1,1) PRIMARY KEY,
    nombre           VARCHAR(50) NOT NULL UNIQUE,
    activo           BIT         NOT NULL DEFAULT 1
);
GO

CREATE TABLE dbo.Clientes (
    id_cliente  INT IDENTITY(1,1) PRIMARY KEY,
    codigo      VARCHAR(10) NOT NULL UNIQUE,
    nombre      VARCHAR(50) NOT NULL,
    apellido    VARCHAR(50) NOT NULL,
    sexo        CHAR(1)     NOT NULL CHECK (sexo IN ('M', 'F', 'X')),
    telefono    VARCHAR(20) NULL,
    activo      BIT         NOT NULL DEFAULT 1
);
GO

CREATE TABLE dbo.Mascotas (
    id_mascota      INT IDENTITY(1,1) PRIMARY KEY,
    nombre          VARCHAR(50) NOT NULL,
    edad            INT         NOT NULL CHECK (edad >= 0),
    id_tipo_mascota INT         NOT NULL,
    id_cliente      INT         NOT NULL,
    activo          BIT         NOT NULL DEFAULT 1,
    CONSTRAINT FK_Mascotas_TiposMascota FOREIGN KEY (id_tipo_mascota) REFERENCES dbo.TiposMascota(id_tipo_mascota),
    CONSTRAINT FK_Mascotas_Clientes FOREIGN KEY (id_cliente) REFERENCES dbo.Clientes(id_cliente)
);
GO

CREATE TABLE dbo.Atenciones (
    id_atencion      INT IDENTITY(1,1) PRIMARY KEY,
    id_mascota       INT           NOT NULL,
    id_tipo_atencion INT           NOT NULL,
    fecha            DATETIME      NOT NULL DEFAULT GETDATE(),
    descripcion      VARCHAR(255)  NOT NULL,
    importe          DECIMAL(10,2) NOT NULL CHECK (importe > 0),
    activo           BIT           NOT NULL DEFAULT 1,
    CONSTRAINT FK_Atenciones_Mascotas FOREIGN KEY (id_mascota) REFERENCES dbo.Mascotas(id_mascota),
    CONSTRAINT FK_Atenciones_TiposAtencion FOREIGN KEY (id_tipo_atencion) REFERENCES dbo.TiposAtencion(id_tipo_atencion)
);
GO

-- ===== Datos de ejemplo =====
-- Los usuarios NO se cargan aca: la API crea los usuarios iniciales al arrancar
-- (admin / Admin123 y empleado / Empleado123) porque la contraseña se guarda con hash.

INSERT INTO dbo.TiposMascota (nombre) VALUES ('Perro'), ('Gato'), ('Araña'), ('Iguana');
GO

INSERT INTO dbo.TiposAtencion (nombre) VALUES ('Consulta'), ('Tratamiento'), ('Vacuna'), ('Control');
GO

INSERT INTO dbo.Clientes (codigo, nombre, apellido, sexo, telefono) VALUES
('C001', 'Juan',     'Pérez',     'M', '351-1111111'),  -- 1
('C002', 'María',    'Gómez',     'F', '351-2222222'),  -- 2
('C003', 'Lucas',    'Fernández', 'M', '351-3333333'),  -- 3
('C004', 'Sofía',    'López',     'F', '351-4444444'),  -- 4
('C005', 'Martín',   'Díaz',      'M', '351-5555555'),  -- 5
('C006', 'Valentina','Romero',    'F', '351-6666666'),  -- 6
('C007', 'Agustín',  'Sosa',      'M', '351-7777777'),  -- 7
('C008', 'Camila',   'Torres',    'F', '351-8888888');  -- 8
GO

INSERT INTO dbo.Mascotas (nombre, edad, id_tipo_mascota, id_cliente) VALUES
('Firulais', 5, 1, 1),  -- 1
('Pelusa',   2, 2, 1),  -- 2
('Michi',    3, 2, 2),  -- 3
('Spidey',   1, 3, 3),  -- 4
('Rex',      2, 4, 4),  -- 5
('Toby',     7, 1, 5),  -- 6
('Luna',     4, 2, 6),  -- 7
('Rocky',    9, 1, 6),  -- 8
('Coco',     1, 4, 7),  -- 9
('Nina',     6, 1, 8),  -- 10
('Simba',    3, 2, 8),  -- 11
('Tarántula',2, 3, 5);  -- 12
GO

INSERT INTO dbo.Atenciones (id_mascota, id_tipo_atencion, fecha, descripcion, importe) VALUES
(1,  3, '2025-12-05', 'Vacuna antirrábica',                    15000),
(3,  1, '2025-12-12', 'Consulta por vómitos',                  12000),
(6,  2, '2025-12-20', 'Tratamiento para otitis',               18000),
(8,  4, '2026-01-08', 'Control anual',                          9000),
(10, 3, '2026-01-15', 'Vacuna séxtuple',                       17000),
(2,  1, '2026-01-27', 'Consulta por pérdida de apetito',       12000),
(5,  2, '2026-02-03', 'Tratamiento de vitamina D y calcio',    11000),
(7,  3, '2026-02-14', 'Vacuna triple felina',                  16000),
(1,  4, '2026-02-25', 'Control de peso',                        8000),
(4,  1, '2026-03-04', 'Consulta por muda incompleta',          10000),
(11, 3, '2026-03-11', 'Vacuna antirrábica',                    15000),
(6,  2, '2026-03-22', 'Tratamiento antiparasitario',           14000),
(9,  4, '2026-04-02', 'Control de temperatura del terrario',    7000),
(3,  2, '2026-04-16', 'Desparasitación interna',                9000),
(8,  1, '2026-04-28', 'Consulta por cojera',                   13000),
(10, 2, '2026-05-06', 'Tratamiento dermatológico',             21000),
(1,  3, '2026-05-19', 'Refuerzo de vacuna séxtuple',           17000),
(12, 4, '2026-05-30', 'Control general',                        7500),
(7,  1, '2026-06-09', 'Consulta por estornudos',               12000),
(6,  4, '2026-06-21', 'Control post tratamiento',               9000),
(2,  3, '2026-07-03', 'Vacuna triple felina',                  16000),
(5,  4, '2026-07-15', 'Control de crecimiento',                 8000),
(8,  2, '2026-07-26', 'Tratamiento antiinflamatorio',          19000),
(11, 1, '2026-08-07', 'Consulta por decaimiento',              12500),
(10, 4, '2026-08-18', 'Control anual',                          9000),
(3,  3, '2026-08-29', 'Vacuna antirrábica',                    15000),
(4,  4, '2026-09-05', 'Control de muda',                        7000),
(6,  1, '2026-09-17', 'Consulta por tos',                      13000),
(1,  2, '2026-09-28', 'Tratamiento de piel',                   20000),
(9,  2, '2026-10-06', 'Tratamiento de vitamina D',             11000),
(7,  4, '2026-10-14', 'Control de peso',                        8500),
(8,  3, '2026-10-23', 'Vacuna antirrábica',                    15500),
(10, 1, '2026-11-02', 'Consulta por decaimiento',              13000),
(2,  2, '2026-11-08', 'Desparasitación externa',                9500),
(6,  3, '2026-11-12', 'Vacuna contra la tos de las perreras',  18000),
(11, 4, '2026-11-17', 'Control general',                        8000);
GO
