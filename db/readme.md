clickhouse-client --query "CREATE TABLE transaction_created_queue (Id UUID, SenderAccount String, RecipientAccount String, Amount String, timestamp UInt64) ENGINE = Kafka('broker:9092', 'TransactionsCreated', 'consumer-group-2', 'JSONEachRow');"

clickhouse-client --query "CREATE TABLE daily ( day Date, total UInt64 ) ENGINE = SummingMergeTree() ORDER BY (day);"

clickhouse-client --query "CREATE MATERIALIZED VIEW consumer TO daily AS SELECT toDate(toDateTime(timestamp)) AS day, count() as total FROM default.transaction_created_queue GROUP BY day;"

===============================

clickhouse-client --query "CREATE TABLE transactions_cancelled_queue (Id UUID, timestamp UInt64) ENGINE = Kafka('broker:9092', 'TransactionsCancelled', 'consumer-group-2', 'JSONEachRow');"

clickhouse-client --query "CREATE TABLE daily_cancelled ( day Date, total UInt64 ) ENGINE = SummingMergeTree() ORDER BY (day);"

clickhouse-client --query "CREATE MATERIALIZED VIEW consumer_cancelled TO daily_cancelled AS SELECT toDate(toDateTime(timestamp)) AS day, count() as total FROM default.transactions_cancelled_queue GROUP BY day;"


===============================

clickhouse-client --query "CREATE TABLE transactions_completed_queue (Id UUID, timestamp UInt64) ENGINE = Kafka('broker:9092', 'TransactionsCompleted', 'consumer-group-2', 'JSONEachRow');"

clickhouse-client --query "CREATE TABLE daily_completed ( day Date, total UInt64 ) ENGINE = SummingMergeTree() ORDER BY (day);"

clickhouse-client --query "CREATE MATERIALIZED VIEW consumer_completed TO daily_completed AS SELECT toDate(toDateTime(timestamp)) AS day, count() as total FROM default.transactions_completed_queue GROUP BY day;"


===============================

clickhouse-client --query "CREATE TABLE daily_summary ( day Date, cancelled UInt64, completed UInt64 ) ENGINE = SummingMergeTree() ORDER BY (day);"

clickhouse-client --query "SELECT cancelled.day, cancelled.total AS cancelled, completed.total AS completed FROM daily_cancelled AS cancelled FULL JOIN daily_completed AS completed ON cancelled.day = completed.day;"