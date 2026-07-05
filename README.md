# SinistrosApi

API RESTful para gestão de sinistros de seguros, desenvolvida como desafio técnico. Clean Architecture, DDD, CQRS-lite com MediatR, .NET 10 e PostgreSQL.

## Como rodar

Primeiro, clona o repositório e entra na pasta:

```
git clone https://github.com/liam-stori/Sinistros.git
```

```
cd Sinistros
```

### Via Docker (recomendado)

Só precisa de **Docker** e **Docker Compose** instalados. Não precisa ter .NET, PostgreSQL ou qualquer SDK na máquina, mesmo o banco é criado e populado automaticamente, sem precisar rodar nada manualmente.

```
docker-compose up --build
```

Isso sobe dois containers: o banco Postgres e a API. Na primeira subida, a API cria o schema do banco automaticamente e popula com dados de teste (clientes, apólices e sinistros já em diferentes estados).

Acesse **`http://localhost:7070/scalar/v1`** — é por ali que tudo é testado. O Scalar já traz todos os endpoints, os modelos de request/response, e permite montar e enviar as requisições direto pelo navegador. Não precisa de Postman, Insomnia ou qualquer ferramenta externa.

Pra derrubar tudo e deletar do Docker (incluindo o volume do banco, resetando os dados):

```
docker-compose down -v
```

### Rodando local (sem Docker)

Precisa ter o **.NET 10 SDK** e o **PostgreSQL** instalados na máquina. Assim como no Docker, não é necessário criar o banco manualmente — a aplicação cria o schema e popula os dados na primeira execução.

PostgreSQL usado nos testes: versão **17.10**, disponível em [enterprisedb.com/downloads/postgres-postgresql-downloads](https://www.enterprisedb.com/downloads/postgres-postgresql-downloads). Não testei em outras versões — se tiver algum problema de compatibilidade, a recomendação é seguir pelo Docker, que já elimina essa variável.

```
dotnet run --project SinistrosApi.Api
```

Acesse **`http://localhost:5119/scalar/v1`**.

**Sobre o banco:** tanto rodando local quanto via Docker, o Postgres escuta na porta `5432`, é o mesmo banco/mesma configuração nos dois cenários, a menos que você já tenha algo rodando nessa porta localmente e precise ajustar.

**Sobre configuração:** existe só um `appsettings.json`, não tem `appsettings.Development.json` nem separação de ambiente. Pra um teste de 3 dias, achei desnecessário simular múltiplos ambientes que não existem de verdade aqui.

**Sobre segurança:** a API não tem autenticação/autorização. Não fazia parte do escopo pedido, e adicionar isso só pra ter seria complexidade sem propósito real no contexto do teste.

## Decisões de arquitetura

O desafio deixa a arquitetura em aberto, então antes de decidir qualquer coisa, montei um cenário de negócio fictício pra não tomar decisão aleatória. É fácil justificar qualquer padrão dizendo "isso escala melhor" sem nunca dizer escala pra quanto. Imaginei uma corretora de porte médio, com cerca de 50 mil apólices ativas na base e um volume de em torno de 5 mil sinistros abertos por mês (aproximadamente 10% das apólices gerando sinistro), distribuídos em 4 ramos (Auto, Residencial, Vida, Saúde), com uso interno por poucos analistas, não sendo um público externo massivo. Esse cenário virou a régua usada em toda decisão daqui pra frente. Vale separar uma coisa da outra: esses números servem só pra guiar a decisão de arquitetura, então o banco de teste que vem populado no seed usa uma massa de dados bem menor, o suficiente pra exercitar os endpoints e as queries pedidas sem gerar volume desnecessário.

O projeto segue Clean Architecture em 4 camadas: Domain, Application, Infrastructure e Api.

**Por que o código está em PT-BR:** o próprio enunciado do desafio já descreve os endpoints, campos e regras em português (`/api/sinistros`, `ValorEstimado`, `HistoricoSinistros`, etc.). Manter entidades, Commands, Handlers e pastas também, foram mantidos alguns locais em inglÊs pela tradução ficar esquisita ou já ser uma convenção a utilização em inglês.

**CQRS-lite:** cada caso de uso (abrir sinistro, atualizar status, listar, consultar histórico) virou um Command ou Query isolado, com seu próprio Handler via MediatR. Isso deixa cada operação pequena e fácil de entender sozinha, sem um Service genérico acumulando lógica de vários fluxos diferentes. Tudo aponta pro mesmo banco e mesmo modelo, não existe necessidade de um CQRS total com separação física entre escrita e leitura, porque nenhum dos dois lados tem carga que justifique isso.

**Entidades e agregado, direto do que o desafio pedia:** Sinistro é o Aggregate Root, porque é ele quem concentra as regras citadas no enunciado, só abre com apólice Ativa, segue o fluxo de status unidirecional, e toda mudança gera histórico. Por isso Sinistro é o único ponto de entrada pra abrir (Sinistro.Abrir) e pra mudar status (AtualizarStatus): acessando as invariantes fica impossível criar um sinistro passando por cima da apólice inativa, ou mudar o status sem gerar o registro correspondente em HistoricoSinistro. O fluxo de status é Aberto -> EmAnalise -> Aprovado -> Encerrado com valor aprovado, OU Aberto -> EmAnalise -> Negado na negativa com motivo obrigatório — sem caminho de volta em nenhum dos dois. Apolice e Cliente viraram entidades próprias porque as regras e as consultas do queries.sql (ranking por cliente, por ramo) exigem identificadores estáveis, não texto solto repetido em cada registro.

**Por que existe um ValidationBehavior:** existe uma diferença entre validar se o dado chegou no formato certo (campo obrigatório preenchido, enumerador com valor válido) e validar uma regra de negócio (que já mora dentro das entidades/agregados, por exemplo "só pode negar com motivo"). A primeira parte não deveria ficar espalhada, repetida à mão, dentro de cada Command, por isso existe o ValidationBehavior, uma peça do MediatR que intercepta toda requisição antes dela chegar no Handler e roda essa validação de formato automaticamente via FluentValidation. Se falhar, o Handler nem chega a ser chamado. Isso mantém o Handler no papel exclusivo de orquestrador, a chamada pro agregado (buscar a entidade, chamar o método de domínio, salvar) a regra de negócio em si nunca sai de dentro da entidade.

**Por que existe um Result Pattern:** casos como "sinistro não encontrado" ou "transição de status inválida" fazem parte do uso normal da API, não são bugs nem falhas de sistema. Usar exceção pra isso tem custo de performance em C# e mistura sinal de erro real com fluxo esperado. Por isso os Handlers e o próprio domínio retornam um Resultado/Resultado<T> (sucesso ou falha com motivo), e quem decide o que devolver pro cliente HTTP é sempre o Controller, olhando esse resultado, nunca um catch genérico tentando adivinhar o que aconteceu.

**Por que existe um Middleware de exceção:** mesmo com o Result Pattern cobrindo os erros esperados, ainda existe espaço pra algo realmente não previsto (erro de conexão com banco, bug não coberto). Pra esses casos existe um middleware global que captura qualquer exceção que escape do fluxo normal e devolve uma resposta HTTP padronizada, em vez de vazar stack trace pro cliente.

**Banco:** PostgreSQL com EF Core. Enumeradores são salvos como texto (não número), pra facilitar leitura direta no banco e escrita do `queries.sql`. Value Objects são mapeados como Owned Types, na mesma tabela do dono.

**Coisas que ficaram de fora intencionalmente:**
- Front-end — o enunciado deixa como opcional. Preferi usar o tempo em arquitetura, testes e documentação, que são os critérios de avaliação citados explicitamente no desafio. A API pode ser testada inteira pelo Scalar.
