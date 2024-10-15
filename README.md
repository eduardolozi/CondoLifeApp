As variáveis de ambiente do projeto não foram protegidas pois este não é o repositório final e sim o de teste.

## Tecnologias Utilizadas

- **.NET**: Framework principal para desenvolvimento da aplicação, incluindo API e serviços de background.
- **Entity Framework + SQL Server**: ORM utilizado para interagir com o banco de dados relacional SQL Server.
- **RavenDB (NoSQL)**: Banco de dados orientado a documentos para armazenamento de mídias (imagens, documentos, etc.).
- **RabbitMQ**: Sistema de mensageria para comunicação assíncrona entre serviços.
- **Worker BackgroundServices**: Serviços de background para execução de tarefas em segundo plano.
- **Autenticação e Autorização JWT**: Segurança baseada em tokens para autenticação e controle de acesso.
- **Docker Compose**: Utilizado para orquestrar e gerenciar contêineres de serviços como RabbitMQ e RavenDB.
