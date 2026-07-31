CREATE TABLE Visits
(
    VisitId INT GENERATED ALWAYS AS IDENTITY,
    VisitNo TEXT NOT NULL,
    PatientId TEXt NOT NULL,
    MedicalId TEXT NOT NULL,
    VisitDate DATETIME NOT NULL,
    CreatedAt DATETIME DEFAULT CURRENT_TIMESTAMP
    PRIMARY KEY(VisitId),
    CONSTRAINT fk_patient 
        FOREIGN KEY (PatientId) 
        REFERENCES Patients(PatientId),
    CONSTRAINT fk_medical 
        FOREIGN KEY (MedicalId) 
        REFERENCES Medical_services(MedicalId)
    
)