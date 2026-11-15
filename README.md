# Desafio Expresso - Sistema de Cafeteria

Aplicação desenvolvida como parte do processo seletivo/desafio técnico para vaga de Desenvolvedor .NET. O sistema consiste em uma solução completa para gerenciamento de pedidos de uma cafeteria, contendo um Totem de Autoatendimento, um Painel de Produção para Baristas e uma Tela de TV para clientes.

## Tecnologias Utilizadas
- **Backend:** ASP.NET Core (Minimal APIs), Entity Framework Core, SQLite.
- **Frontend:** HTML5, Tailwind CSS (via CDN), JavaScript puro (Fetch API).
- **Integrações:** API ViaCEP para preenchimento automático de endereços de entrega.

## Estrutura do Projeto
- `/Backend`: Contém a API em .NET, o contexto do banco de dados e as regras de negócio.
- `/Frontend`: Contém a interface do usuário em arquivo único (`index.html`).

## Como Executar o Projeto

### 1. Rodando o Backend (API)
Abra o terminal na pasta `Backend` e execute o comando:
```bash
dotnet run --urls="http://localhost:5000"