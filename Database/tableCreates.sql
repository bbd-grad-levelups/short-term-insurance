--changeset johan:ddl:createTable:personas
CREATE TABLE personas
(
  id  SERIAL PRIMARY KEY,
  persona_id  BIGINT NOT NULL,
  electronics INTEGER NOT NULL DEFAULT 0,
  last_payment_date VARCHAR(16),
  debit_order_id INT NOT NULL
);
--rollback DROP TABLE "personas";

--changeset johan:ddl:createTable:logs
CREATE TABLE logs
(
  id  SERIAL PRIMARY KEY,
  log_date VARCHAR(16),
  log_message VARCHAR(255)
);
--rollback DROP TABLE "logs";



