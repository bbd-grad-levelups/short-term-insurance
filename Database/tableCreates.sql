--changeset johan:ddl:createTable:personas
CREATE TABLE personas
(
  id  SERIAL PRIMARY KEY,
  persona_id  INTEGER NOT NULL,
  electronics INTEGER NOT NULL DEFAULT 0,
  blacklisted BOOLEAN NOT NULL DEFAULT FALSE
);
--rollback DROP TABLE "personas";