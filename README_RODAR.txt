PROJETO ELECTRONEXT - API C# + FRONT HTML/JS

O que foi feito:
1. API ASP.NET Core em C# com Entity Framework e SQL Server LocalDB.
2. Front-end conectado na API usando fetch em JavaScript.
3. Cadastro, login, listagem de produtos, carrinho, finalização de venda e painel admin.
4. Script SQL pronto em Projeto/Database/setup.sql.

COMO RODAR:

1) Abra o SQL Server Management Studio.
2) Execute o arquivo:
   Projeto/Database/setup.sql

Esse script cria o banco PIM_DB, tabelas principais e produtos iniciais.

3) Abra o projeto no Visual Studio pela solução:
   PIM 3° SEMESTRE.sln

4) Verifique a conexão no arquivo:
   Projeto/appsettings.json

ConnectionString usada:
Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=PIM_DB;Integrated Security=True;Persist Security Info=False;Pooling=False;MultipleActiveResultSets=True;Encrypt=False;TrustServerCertificate=True;Application Name=ElectroNext;Command Timeout=30

5) Rode o projeto Projeto no Visual Studio ou terminal:
   cd Projeto
   dotnet restore
   dotnet run

6) Abra no navegador a URL que aparecer no terminal, por exemplo:
   https://localhost:7000

Páginas:
- /Telalogin.html: login
- /Teacadastr.html: cadastro
- /loja.html: loja com produtos do banco
- /Telacompra.html: checkout
- /finalizacao.html: confirmação
- /admin.html: painel para cadastrar/editar produtos e ver vendas
- /swagger: testar a API

OBSERVAÇÃO IMPORTANTE:
O bkp.txt original enviado foi preservado em Projeto/Database/bkp_original_enviado.txt.
Ele tinha partes avançadas com tabelas extras, procedures e referências que dependem de tabelas não enviadas junto. Para garantir que o projeto rode, deixei o setup.sql com as tabelas usadas pelo front e pela API.
