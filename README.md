# README: LM Orders - Microserviço de Pedidos

Este documento detalha as decisões técnicas, a arquitetura e as tecnologias utilizadas no desenvolvimento do microserviço **LM Orders**, conforme solicitado no Teste Prático de Desenvolvedor Back-end .Net Sênior.

---

## Arquitetura e Padrões Adotados

O projeto foi construído seguindo rigorosos padrões de arquitetura para garantir manutenibilidade, escalabilidade e clareza de responsabilidades.

| Padrão | Aplicação | Benefício |
| :--- | :--- | :--- |
| **Domain-Driven Design (DDD)** | Estrutura de Domínio, Agregados (**Order**), Value Objects (**OrderItem**) e Serviços de Domínio (**OrderDomainService**). | Garante que a lógica de negócio esteja encapsulada e protegida, desacoplando o core da aplicação da infraestrutura. |
| **CQRS** | Uso do **MediatR** para separar operações de escrita (Comandos) e leitura (Queries). | Otimiza o desempenho, permitindo diferentes stacks de persistência e repositórios. |
| **SOLID** | Forte separação de interfaces e responsabilidades. Ex: Repositórios de Escrita (NHibernate) vs Repositórios de Leitura (Dapper). | Melhora a testabilidade e flexibilidade do código. |
| **Persistência Híbrida** | Uso de **SQL Server** para o Agregado Raiz (`Order`) e **MongoDB** para a coleção interna (`OrderItem`). | Atende ao requisito de armazenamento híbrido, aproveitando a transacionalidade do SQL e a flexibilidade do NoSQL para dados de lista. |

---

## Tecnologias Escolhidas

| Camada / Função | Tecnologia | Justificativa da Escolha |
| :--- | :--- | :--- |
| **Linguagem / Framework** | **.NET 8 / ASP.NET Core** | Alto desempenho, robustez e ecossistema nativo para microsserviços. |
| **Orquestração** | **MediatR** | Implementa o padrão *In-Process Messaging* (CQRS e Eventos de Domínio). |
| **Persistência SQL** | **NHibernate** e **Dapper** | NHibernate para o **Unit of Work** (Transação) e Dapper para Queries de Leitura Otimizada. |
| **Persistência NoSQL** | **MongoDB** | Flexibilidade para o Value Object `OrderItem`. |
| **Comunicação Assíncrona** | **Confluent.Kafka** | Tecnologia moderna e altamente escalável para comunicação assíncrona inter-serviços (Faturamento). |
| **Cache** | **Redis** | Cache distribuído de alto desempenho para o requisito de 2 minutos nas consultas `GET`. |
| **Migração** | **FluentMigrator** | Ferramenta de migração de banco de dados para SQL Server. |

---

## Decisões Técnicas Chave

### 1. Modelagem do Domínio e Persistência Híbrida

* **Agregado Raiz (`Order`):** Persistido no **SQL Server** para garantir a integridade transacional e relacional.
* **Value Object (`OrderItem`):** Modelado como um Value Object, refletindo sua dependência exclusiva do Agregado. Persistido separadamente no **MongoDB**, referenciando o `OrderId`.

### 2. Transacionalidade e Eventos de Domínio

O fluxo de escrita é orquestrado pelo `CreateOrderCommandHandler`, garantindo a consistência:

1.  O **`IUnitOfWork`** (via NHibernate) inicia uma transação no SQL.
2.  O Agregado `Order` é criado e o evento de domínio (`OrderCreatedEvent`) é registrado internamente.
3.  As alterações do `Order` são adicionadas ao contexto do SQL.
4.  Os itens são salvos no **MongoDB** (`IOrderItemRepository`).
5.  Os **Eventos de Domínio** registrados são **disparados** via MediatR (`_mediator.Publish(...)`).
6.  O **`OrderCreatedEventHandler`** recebe a notificação e o publica na fila **Kafka**.
7.  O **`IUnitOfWork`** comita a transação SQL.

> **Importante:** Em caso de falha na persistência MongoDB ou no disparo do evento via MediatR, o `catch` aciona o **`IUnitOfWork.RollbackAsync()`**, revertendo a transação no SQL Server para manter a consistência.

### 3. Otimização de Leitura (Query)

O endpoint `GET /pedidos/{id}` implementa o padrão *Cache-Aside* para eficiência:

1.  A consulta **primeiro verifica o cache Redis** (`IOrderCacheService`).
2.  Se houver *cache miss*:
    * O **Dapper** busca o `Order` principal e o `CreatedByName` no SQL.
    * O **MongoDB** busca a lista de `OrderItem`.
    * Os dados são integrados no DTO `OrderResponse`.
    * O resultado final é **cacheado no Redis por 2 minutos**.

---

## Comportamentos da Aplicação

| Comportamento | Ação | Persistência / Comunicação |
| :--- | :--- | :--- |
| **Criação** (`POST /api/Orders`) | Cria e valida o agregado `Order`. | **SQL Server** (Order), **MongoDB** (Items). |
| **Validação** | Regras de Domínio (`EnsureOrderIsValid`). | Lógica de Domínio. |
| **Assíncrono** | Publica o evento `OrderCreatedEvent`. | **Kafka** (Fila externa). |
| **Leitura** (`GET /api/Orders/{id}`) | Busca o pedido completo. | **Redis** (Cache), **SQL Server** (Dapper), **MongoDB** (Items). |

