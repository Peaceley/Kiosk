<<<<<<< HEAD
<<<<<<< HEAD
CREATE TABLE MedicalServices
(
=======
CREATE TABLE MedicalServies(
>>>>>>> origin/main
    MedicalId INT GENERATED ALWAYS AS IDENTITY ,
=======
CREATE TABLE MedicalServices
(
    Id INT GENERATED ALWAYS AS IDENTITY ,
>>>>>>> 201874d44ee97f316f9cd555777ede2e3bb35cd2
    ServiceCode TEXT NOT NULL,
    ServiceName TEXT NOT NULL
)