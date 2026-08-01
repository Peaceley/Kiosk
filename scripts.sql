-- Active: 1784723522577@@localhost@5432@kiosk
CREATE TABLE medical_services
(
    id SERIAL PRIMARY KEY,

    service_name VARCHAR(100) NOT NULL,

    prefix VARCHAR(10) UNIQUE NOT NULL
);



INSERT INTO medical_services
(service_name,prefix)

VALUES

('Out Patient Department','OPD'),

('Laboratory','LAB'),

('Dental','DEN');



CREATE TABLE visits
(
    id SERIAL PRIMARY KEY,

    patient_id INT NOT NULL,

    medical_service_id INT NOT NULL,

    status VARCHAR(20)
    DEFAULT 'WAITING',

    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);



CREATE TABLE token_sequences
(
    id SERIAL PRIMARY KEY,

    prefix VARCHAR(10) UNIQUE NOT NULL,

    last_number INT DEFAULT 0
);



INSERT INTO token_sequences(prefix)

VALUES

('OPD'),

('LAB'),

('DEN');



CREATE TABLE tokens
(
    id SERIAL PRIMARY KEY,

    visit_id INT NOT NULL,

    token_number VARCHAR(20) NOT NULL,

    status VARCHAR(20)
    DEFAULT 'WAITING',

    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);