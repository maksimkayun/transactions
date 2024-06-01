clickhouse-client --query "CREATE TABLE transaction_created_queue (Id UUID, SenderAccount String, RecipientAccount String, Amount String, timestamp UInt64) ENGINE = Kafka('broker:19092', 'TransactionsCreated', 'consumer-group-2', 'JSONEachRow');"

clickhouse-client --query "CREATE TABLE daily ( day Date, total UInt64 ) ENGINE = SummingMergeTree() ORDER BY (day);"

clickhouse-client --query "CREATE MATERIALIZED VIEW consumer TO daily AS SELECT toDate(toDateTime(timestamp)) AS day, count() as total FROM default.transaction_created_queue GROUP BY day;"
