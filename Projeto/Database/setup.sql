IF DB_ID('PIM_DB') IS NULL
BEGIN
    CREATE DATABASE PIM_DB;
END
GO

USE PIM_DB;
GO

IF OBJECT_ID('tbitemvenda', 'U') IS NOT NULL DROP TABLE tbitemvenda;
IF OBJECT_ID('tbvenda', 'U') IS NOT NULL DROP TABLE tbvenda;
IF OBJECT_ID('tbproduto', 'U') IS NOT NULL DROP TABLE tbproduto;
IF OBJECT_ID('tbcadastrocliente', 'U') IS NOT NULL DROP TABLE tbcadastrocliente;
IF OBJECT_ID('tbcondicaopagamento', 'U') IS NOT NULL DROP TABLE tbcondicaopagamento;
GO

CREATE TABLE tbcadastrocliente (
    id_cliente INT IDENTITY(1,1) PRIMARY KEY,
    nome_cliente NVARCHAR(150) NOT NULL,
    email NVARCHAR(120) NOT NULL UNIQUE,
    telefone_celular NVARCHAR(20) NULL,
    senha NVARCHAR(255) NOT NULL,
    data_cadastro DATETIME NOT NULL DEFAULT GETDATE(),
    ativo BIT NOT NULL DEFAULT 1
);
GO

CREATE TABLE tbproduto (
    id_produto INT IDENTITY(1,1) PRIMARY KEY,
    nome_produto NVARCHAR(150) NOT NULL,
    descricao NVARCHAR(MAX) NULL,
    preco_venda DECIMAL(10,2) NOT NULL,
    estoque INT NOT NULL DEFAULT 0,
    ativo BIT NOT NULL DEFAULT 1,
    imagem_url NVARCHAR(255) NULL
);
GO

CREATE TABLE tbvenda (
    id_venda INT IDENTITY(1,1) PRIMARY KEY,
    id_cliente INT NOT NULL,
    data_venda DATETIME NOT NULL DEFAULT GETDATE(),
    status_venda NVARCHAR(30) NOT NULL DEFAULT 'APROVADO',
    status_pagamento NVARCHAR(30) NOT NULL DEFAULT 'PENDENTE',
    valor_total DECIMAL(10,2) NOT NULL DEFAULT 0,
    CONSTRAINT fk_venda_cliente FOREIGN KEY (id_cliente) REFERENCES tbcadastrocliente(id_cliente)
);
GO

CREATE TABLE tbitemvenda (
    id_item_venda INT IDENTITY(1,1) PRIMARY KEY,
    id_venda INT NOT NULL,
    id_produto INT NOT NULL,
    quantidade INT NOT NULL CHECK (quantidade > 0),
    preco_unitario DECIMAL(10,2) NOT NULL CHECK (preco_unitario >= 0),
    desconto_percentual DECIMAL(5,2) NOT NULL DEFAULT 0,
    CONSTRAINT fk_itemvenda_venda FOREIGN KEY (id_venda) REFERENCES tbvenda(id_venda),
    CONSTRAINT fk_itemvenda_produto FOREIGN KEY (id_produto) REFERENCES tbproduto(id_produto)
);
GO

CREATE TABLE tbcondicaopagamento (
    id_condicao_pagamento INT IDENTITY(1,1) PRIMARY KEY,
    descricao NVARCHAR(100) NOT NULL,
    numero_parcelas INT NOT NULL DEFAULT 1,
    taxa_juros_percentual DECIMAL(5,2) NOT NULL DEFAULT 0,
    dias_primeira_parcela INT NOT NULL DEFAULT 0,
    ativo BIT NOT NULL DEFAULT 1
);
GO

INSERT INTO tbcondicaopagamento (descricao, numero_parcelas, taxa_juros_percentual, dias_primeira_parcela) VALUES
('À vista', 1, 0, 0),
('Pix', 1, 0, 0),
('2x sem juros', 2, 0, 30),
('3x sem juros', 3, 0, 30);
GO

INSERT INTO tbproduto (nome_produto, descricao, preco_venda, estoque, imagem_url) VALUES
('Mouse Gamer', 'Mouse gamer com alta precisão', 90.00, 30, 'teclado.jpg'),
('Teclado Gamer', 'Teclado mecânico para jogos', 300.00, 20, 'teclado.jpg'),
('Mouse Pad', 'Mouse pad grande', 50.00, 50, 'teclado.jpg'),
('Cadeira Gamer', 'Cadeira confortável para setup gamer', 5000.00, 5, 'teclado.jpg');
GO
