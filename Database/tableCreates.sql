--changeset johan:ddl:createTable:personas
CREATE TABLE personas
(
  id  SERIAL PRIMARY KEY,
  persona_id  BIGINT NOT NULL,
  electronics INTEGER NOT NULL DEFAULT 0,
  last_payment_date VARCHAR(10),
  debit_order_id INT NOT NULL
);
--rollback DROP TABLE "personas";