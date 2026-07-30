CREATE TABLE MedicalServies(
    MedicalId INT GENERATED ALWAYS AS IDENTITY ,
    ServiceCode TEXT NOT NULL,
    ServiceName TEXT NOT NULL
)