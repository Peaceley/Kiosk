-- Active: 1784723522577@@localhost@5432@queuekiosk
CREATE TABLE Visits
(
    VisitId INT GENERATED ALWAYS AS IDENTITY PRIMARY KEY,
    VisitNo TEXT NOT NULL,
    PatientId INT NOT NULL,
    MedicalServicesCode TEXT NOT NULL,
<<<<<<< HEAD
    VisitDate TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    CreatedAt TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
   
=======
    VisitDate TIMESTAMP NOT NULL,
    CreatedAt TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    PRIMARY KEY(VisitId),
>>>>>>> 201874d44ee97f316f9cd555777ede2e3bb35cd2
    CONSTRAINT fk_patient 
        FOREIGN KEY (PatientId) 
        REFERENCES Patients(PatientId)
    
<<<<<<< HEAD
);
=======
)

CREATE TABLE Visits
(
    VisitId INT GENERATED ALWAYS AS IDENTITY,
    VisitNo TEXT NOT NULL,
    PatientId INT NOT NULL,
    MedicalServicesCode TEXT NOT NULL,
    VisitDate TIMESTAMP NOT NULL,
    CreatedAt TIMESTAMP DEFAULT CURRENT_TIMESTAMP,

    PRIMARY KEY (VisitId),

    CONSTRAINT fk_patient
        FOREIGN KEY (PatientId)
        REFERENCES Patients(PatientId)
);

SELECT * FROM Visits



>>>>>>> 201874d44ee97f316f9cd555777ede2e3bb35cd2
