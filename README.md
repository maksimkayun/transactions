# Transactions: Микросервисное решение для финансовых операций

<div align="center">
  <img src="https://img.shields.io/badge/.NET%20Core-512BD4?style=for-the-badge&logo=dotnet&logoColor=white" alt=".NET Core" />
  <img src="https://img.shields.io/badge/C%23-239120?style=for-the-badge&logo=c-sharp&logoColor=white" alt="C#" />
  <img src="https://img.shields.io/badge/Apache%20Kafka-231F20?style=for-the-badge&logo=apache-kafka&logoColor=white" alt="Apache Kafka" />
  <img src="https://img.shields.io/badge/PostgreSQL-316192?style=for-the-badge&logo=postgresql&logoColor=white" alt="PostgreSQL" />
  <img src="https://img.shields.io/badge/Docker-2496ED?style=for-the-badge&logo=docker&logoColor=white" alt="Docker" />
  <img src="https://img.shields.io/badge/Kubernetes-326CE5?style=for-the-badge&logo=kubernetes&logoColor=white" alt="Kubernetes" />
</div>

<div align="center">
  <img src="https://mermaid.ink/img/pako:eNptkU1PwzAMhv9KlBMgsTZ0Y1O3w3YAiQNCQuLiJaaxlsRR4gxWVf3vJN1HBfXk2I_fV7bjHhtjoYYzrh0jRrKG3j08PjWkCbQP5NZNYOk9dUbE3ZyVQm2VtmTnrmbpodWKhKRu8YYpvkI4Jqnhd9MZFIZbcI71QLLDlUvWWHXO0m-oGM4Sh1cUJ-gvzrSGwzDhm_mFZx3_DMfY9h25DQrhbdW_5VOE4_gMuT6iH1IetTrpBQdKKzZ-5YYlSB-XkF19QttjCHWxudkUm12x3eX7fbErYrOaUHa6a41U82RXWYfAP6a_TdOK3e2ueOXZbAq8zSdDh4zQEkKGZEzqQrK6c9w_pjdG0BPlrh4x5zRggWFcCz3aZtBiHdG1Sc09gy8YLv5jLj_1C6YdvBI?" alt="Transactions Architecture" width="700px" />
</div>

## 📋 Описание

**Transactions** — это современное микросервисное решение для обработки финансовых транзакций, разработанное с применением принципов Event-Driven архитектуры. Проект демонстрирует построение масштабируемой и отказоустойчивой системы на базе .NET Core и контейнерных технологий с использованием Apache Kafka в качестве платформы обмена сообщениями.

### 🌟 Ключевые возможности

- **Высокая пропускная способность** обработки финансовых операций
- **Горизонтальное масштабирование** для роста нагрузки
- **Отказоустойчивость** за счет асинхронного взаимодействия
- **Распределенная трассировка** запросов через микросервисы
- **Event-driven модель** для слабого связывания компонентов

## 🏗️ Архитектура

Система построена на основе современной микросервисной архитектуры:

<table>
  <tr>
    <td align="center"><img src="https://cdn.jsdelivr.net/gh/devicons/devicon/icons/dotnetcore/dotnetcore-original.svg" width="40" height="40"/><br/><b>API Gateway</b></td>
    <td>Централизованная точка входа, обеспечивающая маршрутизацию и аутентификацию запросов</td>
  </tr>
  <tr>
    <td align="center"><img src="https://cdn.jsdelivr.net/gh/devicons/devicon/icons/dotnetcore/dotnetcore-original.svg" width="40" height="40"/><br/><b>Aggregator</b></td>
    <td>Сервис для сбора и агрегации данных из различных микросервисов</td>
  </tr>
  <tr>
    <td align="center"><img src="https://cdn.jsdelivr.net/gh/devicons/devicon/icons/dotnetcore/dotnetcore-original.svg" width="40" height="40"/><br/><b>Domain</b></td>
    <td>Ядро системы, содержащее бизнес-логику и доменные модели</td>
  </tr>
  <tr>
    <td align="center"><img src="https://cdn.jsdelivr.net/gh/devicons/devicon/icons/dotnetcore/dotnetcore-original.svg" width="40" height="40"/><br/><b>Common</b></td>
    <td>Библиотека общих компонентов и утилит</td>
  </tr>
  <tr>
    <td align="center"><img src="https://cdn.jsdelivr.net/gh/devicons/devicon/icons/dotnetcore/dotnetcore-original.svg" width="40" height="40"/><br/><b>ESB Adapter</b></td>
    <td>Адаптер для интеграции с корпоративной сервисной шиной</td>
  </tr>
</table>

## 🛠️ Технологический стек

<table>
  <tr>
    <td><img src="https://cdn.jsdelivr.net/gh/devicons/devicon/icons/csharp/csharp-original.svg" width="20" height="20"/>&nbsp; <b>C#</b></td>
    <td>Основной язык программирования</td>
  </tr>
  <tr>
    <td><img src="https://cdn.jsdelivr.net/gh/devicons/devicon/icons/dotnetcore/dotnetcore-original.svg" width="20" height="20"/>&nbsp; <b>.NET Core</b></td>
    <td>Кроссплатформенный фреймворк для создания микросервисов</td>
  </tr>
  <tr>
    <td><img src="https://cdn.jsdelivr.net/gh/devicons/devicon/icons/apache/apache-original.svg" width="20" height="20"/>&nbsp; <b>Apache Kafka</b></td>
    <td>Распределенная платформа обработки событий для асинхронной коммуникации</td>
  </tr>
  <tr>
    <td><img src="https://cdn.jsdelivr.net/gh/devicons/devicon/icons/docker/docker-original.svg" width="20" height="20"/>&nbsp; <b>Docker</b></td>
    <td>Контейнеризация приложений и сервисов</td>
  </tr>
  <tr>
    <td><img src="https://cdn.jsdelivr.net/gh/devicons/devicon/icons/postgresql/postgresql-original.svg" width="20" height="20"/>&nbsp; <b>PostgreSQL</b></td>
    <td>Хранение данных о транзакциях и бизнес-сущностях</td>
  </tr>
</table>

## 🚀 Запуск проекта

### Предварительные требования

- Docker
- Docker Compose
- .NET Core SDK 6.0+

### Шаги для запуска

1. **Клонирование репозитория**

```bash
git clone https://github.com/maksimkayun/transactions.git
cd transactions
```

2. **Запуск с использованием Docker Compose**

```bash
docker-compose up --build
```

Это развернет все необходимые сервисы, включая:
- Микросервисы приложения
- PostgreSQL базу данных
- Apache Kafka для обмена сообщениями
- Schema Registry для управления схемами сообщений
- Инструменты мониторинга

3. **Доступ к API**

API будет доступен по адресу: `http://localhost:5000`

### Запуск тестов

```bash
dotnet test
```

## 📈 Системные требования и производительность

- **Минимальные требования**: 2 CPU, 4 GB RAM для запуска в режиме разработки
- **Рекомендуемые**: 4+ CPU, 8+ GB RAM для продакшн-окружения
- **Производительность**: Обработка до 1000 транзакций в секунду на одном узле

## 📊 Примеры использования

```csharp
// Пример создания новой транзакции через API
var transactionRequest = new TransactionRequest
{
    Amount = 1500.50m,
    Currency = "USD",
    SourceAccountId = "ACC123456",
    DestinationAccountId = "ACC789012",
    Description = "Monthly payment"
};

var response = await httpClient.PostAsJsonAsync("/api/transactions", transactionRequest);
var transactionId = await response.Content.ReadFromJsonAsync<string>();
```

## 📋 Документация API

| Endpoint | Метод | Описание |
|----------|-------|----------|
| `/api/transactions` | POST | Создание новой транзакции |
| `/api/transactions/{id}` | GET | Получение информации о транзакции |
| `/api/transactions/accounts/{accountId}` | GET | Получение всех транзакций по счету |
| `/api/reports/daily` | GET | Получение отчета за день |

## 🔄 Диаграмма последовательности для обработки транзакции

```mermaid
sequenceDiagram
    participant Client
    participant API Gateway
    participant Transaction Service
    participant Kafka
    participant Validation Service
    participant Account Service
    participant DB

    Client->>API Gateway: POST /transactions
    API Gateway->>Transaction Service: Create Transaction
    Transaction Service->>Kafka: Publish TransactionCreated
    Transaction Service-->>API Gateway: Transaction ID
    API Gateway-->>Client: Transaction ID
    
    Kafka->>Validation Service: Consume TransactionCreated
    Validation Service->>Kafka: Publish TransactionValidated
    
    Kafka->>Account Service: Consume TransactionValidated
    Account Service->>DB: Update Account Balance
    Account Service->>Kafka: Publish TransactionCompleted
```

## 📜 Лицензия

Проект распространяется под лицензией MIT.
---

<div align="center">
  <sub>Построено с ❤️ в России</sub>
</div>
