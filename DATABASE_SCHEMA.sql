-- Criar banco de dados
CREATE DATABASE back_educonnect;
GO

USE back_educonnect;
GO

-- Tabela de Perfis/Roles
CREATE TABLE perfis (
    id INT PRIMARY KEY IDENTITY(1,1),
    nome NVARCHAR(100) NOT NULL,
    descricao NVARCHAR(255),
    data_criacao DATETIME DEFAULT GETDATE(),
    data_atualizacao DATETIME
);
GO

-- Tabela de Usuários
CREATE TABLE usuarios (
    id INT PRIMARY KEY IDENTITY(1,1),
    nome NVARCHAR(150) NOT NULL,
    email NVARCHAR(150) NOT NULL UNIQUE,
    senha_hash NVARCHAR(255) NOT NULL,
    id_perfil INT NOT NULL,
    ativo BIT DEFAULT 1,
    tentativas_falhas INT DEFAULT 0,
    bloqueado_ate DATETIME NULL,
    ultimo_login DATETIME NULL,
    data_aceite_termos DATETIME NULL,
    versao_termos INT NULL,
    data_criacao DATETIME DEFAULT GETDATE(),
    data_atualizacao DATETIME,
    FOREIGN KEY (id_perfil) REFERENCES perfis(id)
);
GO

-- Índices para melhor performance
CREATE INDEX idx_usuarios_email ON usuarios(email);
CREATE INDEX idx_usuarios_id_perfil ON usuarios(id_perfil);
CREATE INDEX idx_usuarios_ativo ON usuarios(ativo);
GO

-- Inserir perfis padrão
INSERT INTO perfis (nome, descricao) VALUES
('Administrador', 'Acesso total ao sistema'),
('Professor', 'Acesso ao módulo acadêmico'),
('Aluno', 'Acesso limitado para alunos'),
('Financeiro', 'Acesso ao módulo financeiro'),
('Bibliotecário', 'Acesso ao módulo biblioteca');
GO

-- Inserir usuário administrador (senha: 123456)
-- Hash BCrypt da senha "123456": $2a$11$xrqKqMfvLIe/RvIQ1nphL.Z9hkWQpK7.8vJzFCQXXL3pB2h3YrI3e
INSERT INTO usuarios (nome, email, senha_hash, id_perfil, ativo, data_aceite_termos, versao_termos)
VALUES (
    'Administrador do Sistema',
    'adm@edu.com',
    '$2a$11$xrqKqMfvLIe/RvIQ1nphL.Z9hkWQpK7.8vJzFCQXXL3pB2h3YrI3e',
    1,
    1,
    GETDATE(),
    1
);
GO
