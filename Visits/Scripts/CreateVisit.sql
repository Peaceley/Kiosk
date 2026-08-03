-- Active: 1784723522577@@localhost@5432@kioskvisit
CREATE TABLE Visits
(
    VisitId INT GENERATED ALWAYS AS IDENTITY,
    VisitNo TEXT NOT NULL,
    PatientId TEXt NOT NULL,
    MedicalId INT NOT NULL,
    VisitDate TIMESTAMP NOT NULL,
    CreatedAt TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    PRIMARY KEY(VisitId),
    CONSTRAINT fk_patient 
        FOREIGN KEY (PatientId) 
        REFERENCES Patients(PatientId)
    
);
--working
CREATE TABLE Visits
(
    VisitId INT GENERATED ALWAYS AS IDENTITY,
    VisitNo TEXT NOT NULL,
    PatientId INT NOT NULL,
    MedicalId INT NOT NULL,
    VisitDate TIMESTAMP NOT NULL,
    CreatedAt TIMESTAMP DEFAULT CURRENT_TIMESTAMP,

    PRIMARY KEY (VisitId),

    CONSTRAINT fk_patient
        FOREIGN KEY (PatientId)
        REFERENCES Patients(PatientId)
);


SELECT *FROM Visits


