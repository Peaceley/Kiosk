CREATE TABLE Visits
(
    VisitId INT GENERATED ALWAYS AS IDENTITY,
    VisitNo TEXT NOT NULL,
    PatientId TEXt NOT NULL,
    MedicalServicesCode TEXT NOT NULL,
    VisitDate DATETIME NOT NULL,
    CreatedAt DATETIME DEFAULT CURRENT_TIMESTAMP
    PRIMARY KEY(id),
    CONSTRAINT fk_patient 
        FOREIGN KEY (PatientId) 
        REFERENCES Patients(PatientId)
    
)