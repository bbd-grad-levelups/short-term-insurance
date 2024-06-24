--changeset johan:dml:mockData:personas
INSERT INTO personas (persona_id, electronics, blacklisted) VALUES
(1, 3, false),
(2, 1, false);
--rollback DELETE FROM "personas";
