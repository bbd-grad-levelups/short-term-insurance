--changeset johan:dml:mockData:personas
INSERT INTO personas (persona_id, electronics, last_payment_date, debit_order_id) VALUES
(1, 3, "01|01|01", 123312),
(2, 1, "01|01|01", 1289582);
--rollback DELETE FROM "personas";
