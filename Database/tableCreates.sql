--changeset johan:ddl:createTable:personas
CREATE TABLE personas
(
  id  SERIAL PRIMARY KEY,
  persona_id  BIGINT NOT NULL,
  electronics INTEGER NOT NULL DEFAULT 0,
  blacklisted BOOLEAN NOT NULL DEFAULT FALSE,
  bank_account VARCHAR(100) NOT NULL DEFAULT ''
);
--rollback DROP TABLE "personas";