CREATE TABLE MedicalServices
(
    MedicalId INT GENERATED ALWAYS AS IDENTITY PRIMARY KEY ,
    MedicalServiceName TEXT NOT NULL,
    MedicalServiceCode TEXT NOT NULL UNIQUE,
    CreatedAt TIMESTAMP DEFAULT CURRENT_TIMESTAMP

);

--THIS CHECKS FOR THE TABLES 
SELECT table_name
FROM information_schema.tables
WHERE table_schema='public';

SELECT * FROM medicalservices;
SELECT * FROM MedicalServices;

INSERT INTO medicalservices
(
    medicalservicename,
    medicalservicecode
)
VALUES
(
    'General Consultation',
    'GEN'

),
( 'Laboratory',
 'LAB'
 ),
(
    'Dental',
    'DEN'
),
(
    'Phamarcy',
    'PHA'
)

--restarting the medicalservice table

TRUNCATE TABLE MedicalServices RESTART IDENTITY CASCADE;
DROP TABLE MedicalServices