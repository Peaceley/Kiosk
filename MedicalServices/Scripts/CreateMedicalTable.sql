-- Active: 1784723522577@@localhost@5432@queuekiosk
CREATE TABLE MedicalServices
(
    Id INT GENERATED ALWAYS AS IDENTITY ,
    ServiceCode TEXT NOT NULL,
    ServiceName TEXT NOT NULL
)