
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




