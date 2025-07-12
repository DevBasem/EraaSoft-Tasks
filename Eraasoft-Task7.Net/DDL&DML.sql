CREATE TABLE PharmaceuticalCompany (
    Name NVARCHAR(100) PRIMARY KEY,
    Address NVARCHAR(255) NOT NULL,
    Phone NVARCHAR(20) NOT NULL
);

CREATE TABLE Doctor (
    ID INT IDENTITY(1,1) PRIMARY KEY,
    Name NVARCHAR(100) NOT NULL,
    Email NVARCHAR(100) NOT NULL,
    Phone NVARCHAR(20) NOT NULL,
    Specialty NVARCHAR(50) NOT NULL,
    YearsOfExperience INT NOT NULL
);

CREATE TABLE Drug (
    DrugID INT IDENTITY(1,1) PRIMARY KEY,
    TradeName NVARCHAR(100) NOT NULL,
    DrugStrength NVARCHAR(50) NOT NULL,
    CompanyName NVARCHAR(100) NOT NULL,
    FOREIGN KEY (CompanyName) REFERENCES PharmaceuticalCompany(Name) ON DELETE CASCADE
);

CREATE TABLE Patient (
    UR NVARCHAR(20) PRIMARY KEY,
    Name NVARCHAR(100) NOT NULL,
    Address NVARCHAR(255) NOT NULL,
    Age INT NOT NULL CHECK (Age >= 0),
    Email NVARCHAR(100),
    Phone NVARCHAR(20),
    MedicareCardNumber NVARCHAR(20),
    PrimaryDoctorID INT NOT NULL,
    FOREIGN KEY (PrimaryDoctorID) REFERENCES Doctor(ID)
);

CREATE TABLE Prescription (
    PrescriptionID INT IDENTITY(1,1) PRIMARY KEY,
    PatientUR NVARCHAR(20) NOT NULL,
    DoctorID INT NOT NULL,
    DrugID INT NOT NULL,
    PrescriptionDate DATE NOT NULL,
    Quantity INT NOT NULL CHECK (Quantity > 0),
    FOREIGN KEY (PatientUR) REFERENCES Patient(UR),
    FOREIGN KEY (DoctorID) REFERENCES Doctor(ID),
    FOREIGN KEY (DrugID) REFERENCES Drug(DrugID)
);

-- SELECT: Retrieve all columns from the Doctor table.
SELECT * FROM Doctor;

-- ORDER BY: List patients in the Patient table in ascending order of their ages.
SELECT * FROM Patient ORDER BY Age ASC;

-- OFFSET FETCH: Retrieve the first 10 patients from the Patient table, starting from the 5th record
SELECT * FROM Patient 
ORDER BY UR 
OFFSET 4 ROWS 
FETCH NEXT 10 ROWS ONLY;

-- SELECT TOP: Retrieve the top 5 doctors from the Doctor table
SELECT TOP 5 * FROM Doctor;

-- SELECT DISTINCT: Get a list of unique addresses from the Patient table
SELECT DISTINCT Address FROM Patient;

-- WHERE: Retrieve patients from the Patient table who are aged 25
SELECT * FROM Patient WHERE Age = 25;

-- NULL: Retrieve patients from the Patient table whose email is not provided
SELECT * FROM Patient WHERE Email IS NULL;

-- AND: Retrieve doctors from the Doctor table who have experience greater than 5 years and specialize in 'Cardiology'
SELECT * FROM Doctor WHERE YearsOfExperience > 5 AND Specialty = 'Cardiology';

-- IN: Retrieve doctors from the Doctor table whose speciality is either 'Dermatology' or 'Oncology'
SELECT * FROM Doctor WHERE Specialty IN ('Dermatology', 'Oncology');

-- BETWEEN: Retrieve patients from the Patient table whose ages are between 18 and 30
SELECT * FROM Patient WHERE Age BETWEEN 18 AND 30;

-- LIKE: Retrieve doctors from the Doctor table whose names start with 'Dr.'
SELECT * FROM Doctor WHERE Name LIKE 'Dr.%';

-- Column & Table Aliases: Select the name and email of doctors, aliasing them as 'DoctorName' and 'DoctorEmail'
SELECT Name AS DoctorName, Email AS DoctorEmail FROM Doctor;

-- Joins: Retrieve all prescriptions with corresponding patient names
SELECT p.PrescriptionID, pat.Name AS PatientName, d.Name AS DoctorName, dr.TradeName AS DrugName, p.PrescriptionDate, p.Quantity
FROM Prescription p
JOIN Patient pat ON p.PatientUR = pat.UR
JOIN Doctor d ON p.DoctorID = d.ID
JOIN Drug dr ON p.DrugID = dr.DrugID;

-- GROUP BY: Retrieve the count of patients grouped by their cities
SELECT 
    CASE 
        WHEN Address LIKE '%Geelong VIC 3220%' THEN 'Geelong 3220'
        WHEN Address LIKE '%Geelong VIC 3218%' THEN 'Geelong 3218'
        ELSE 'Other'
    END AS City,
    COUNT(*) AS PatientCount
FROM Patient
GROUP BY 
    CASE 
        WHEN Address LIKE '%Geelong VIC 3220%' THEN 'Geelong 3220'
        WHEN Address LIKE '%Geelong VIC 3218%' THEN 'Geelong 3218'
        ELSE 'Other'
    END;

-- HAVING: Retrieve cities with more than 3 patients
SELECT 
    CASE 
        WHEN Address LIKE '%Geelong VIC 3220%' THEN 'Geelong 3220'
        WHEN Address LIKE '%Geelong VIC 3218%' THEN 'Geelong 3218'
        ELSE 'Other'
    END AS City,
    COUNT(*) AS PatientCount
FROM Patient
GROUP BY 
    CASE 
        WHEN Address LIKE '%Geelong VIC 3220%' THEN 'Geelong 3220'
        WHEN Address LIKE '%Geelong VIC 3218%' THEN 'Geelong 3218'
        ELSE 'Other'
    END
HAVING COUNT(*) > 3;

-- GROUPING SETS: Retrieve counts of patients grouped by cities and ages
SELECT 
    CASE 
        WHEN Address LIKE '%Geelong VIC 3220%' THEN 'Geelong 3220'
        WHEN Address LIKE '%Geelong VIC 3218%' THEN 'Geelong 3218'
        ELSE 'Other'
    END AS City,
    CASE 
        WHEN Age BETWEEN 18 AND 30 THEN '18-30'
        WHEN Age BETWEEN 31 AND 50 THEN '31-50'
        WHEN Age BETWEEN 51 AND 70 THEN '51-70'
        ELSE '70+'
    END AS AgeGroup,
    COUNT(*) AS PatientCount
FROM Patient
GROUP BY GROUPING SETS (
    (CASE 
        WHEN Address LIKE '%Geelong VIC 3220%' THEN 'Geelong 3220'
        WHEN Address LIKE '%Geelong VIC 3218%' THEN 'Geelong 3218'
        ELSE 'Other'
    END),
    (CASE 
        WHEN Age BETWEEN 18 AND 30 THEN '18-30'
        WHEN Age BETWEEN 31 AND 50 THEN '31-50'
        WHEN Age BETWEEN 51 AND 70 THEN '51-70'
        ELSE '70+'
    END),
    ()
);

-- CUBE: Retrieve counts of patients considering all possible combinations of city and age
SELECT 
    CASE 
        WHEN Address LIKE '%Geelong VIC 3220%' THEN 'Geelong 3220'
        WHEN Address LIKE '%Geelong VIC 3218%' THEN 'Geelong 3218'
        ELSE 'Other'
    END AS City,
    CASE 
        WHEN Age BETWEEN 18 AND 30 THEN '18-30'
        WHEN Age BETWEEN 31 AND 50 THEN '31-50'
        WHEN Age BETWEEN 51 AND 70 THEN '51-70'
        ELSE '70+'
    END AS AgeGroup,
    COUNT(*) AS PatientCount
FROM Patient
GROUP BY CUBE (
    (CASE 
        WHEN Address LIKE '%Geelong VIC 3220%' THEN 'Geelong 3220'
        WHEN Address LIKE '%Geelong VIC 3218%' THEN 'Geelong 3218'
        ELSE 'Other'
    END),
    (CASE 
        WHEN Age BETWEEN 18 AND 30 THEN '18-30'
        WHEN Age BETWEEN 31 AND 50 THEN '31-50'
        WHEN Age BETWEEN 51 AND 70 THEN '51-70'
        ELSE '70+'
    END)
);

-- ROLLUP: Retrieve counts of patients rolled up by city
SELECT 
    CASE 
        WHEN Address LIKE '%Geelong VIC 3220%' THEN 'Geelong 3220'
        WHEN Address LIKE '%Geelong VIC 3218%' THEN 'Geelong 3218'
        ELSE 'Other'
    END AS City,
    COUNT(*) AS PatientCount
FROM Patient
GROUP BY ROLLUP (
    CASE 
        WHEN Address LIKE '%Geelong VIC 3220%' THEN 'Geelong 3220'
        WHEN Address LIKE '%Geelong VIC 3218%' THEN 'Geelong 3218'
        ELSE 'Other'
    END
);

-- EXISTS: Retrieve patients who have at least one prescription
SELECT * FROM Patient p
WHERE EXISTS (
    SELECT 1 FROM Prescription pr WHERE pr.PatientUR = p.UR
);

-- UNION: Retrieve a combined list of doctors and patients
SELECT Name, Email, Phone, 'Doctor' AS PersonType FROM Doctor
UNION
SELECT Name, Email, Phone, 'Patient' AS PersonType FROM Patient;

-- Common Table Expression (CTE): Retrieve patients along with their doctors using a CTE
WITH PatientDoctorCTE AS (
    SELECT 
        p.UR,
        p.Name AS PatientName,
        p.Age,
        d.Name AS PrimaryDoctorName,
        d.Specialty
    FROM Patient p
    JOIN Doctor d ON p.PrimaryDoctorID = d.ID
)
SELECT * FROM PatientDoctorCTE;

-- INSERT: Insert a new doctor into the Doctor table
INSERT INTO Doctor (Name, Email, Phone, Specialty, YearsOfExperience)
VALUES ('Dr. Amanda White', 'amanda.white@barwonhealth.org.au', '03-4215-1011', 'Radiology', 9);

-- INSERT Multiple Rows: Insert multiple patients into the Patient table
INSERT INTO Patient (UR, Name, Address, Age, Email, Phone, MedicareCardNumber, PrimaryDoctorID)
VALUES 
    ('UR013456', 'Kevin Murphy', '159 Bellarine Highway, Geelong VIC 3220', 41, 'kevin.murphy@email.com', '0412-555-001', '4123456789', 2),
    ('UR014567', 'Helen Carter', '267 Moorabool Street, Geelong VIC 3220', 33, 'helen.carter@email.com', '0423-555-002', '5234567890', 3),
    ('UR015678', 'Tony Garcia', '385 Malop Street, Geelong VIC 3220', 47, 'tony.garcia@email.com', '0434-555-003', '6345678901', 4);

-- UPDATE: Update the phone number of a doctor
UPDATE Doctor 
SET Phone = '03-4215-2001' 
WHERE Name = 'Dr. Sarah Mitchell';

-- UPDATE JOIN: Update the city of patients who have a prescription from a specific doctor
UPDATE Patient 
SET Address = REPLACE(Address, 'Geelong VIC 3220', 'Geelong VIC 3221')
WHERE UR IN (
    SELECT DISTINCT PatientUR 
    FROM Prescription 
    WHERE DoctorID = (SELECT ID FROM Doctor WHERE Name = 'Dr. Sarah Mitchell')
);

-- DELETE: Delete a patient from the Patient table
DELETE FROM Patient WHERE UR = 'UR015678';

-- Transaction: Insert a new doctor and a patient, ensuring both operations succeed or fail together
BEGIN TRANSACTION;
BEGIN TRY
    INSERT INTO Doctor (Name, Email, Phone, Specialty, YearsOfExperience)
    VALUES ('Dr. Rachel Green', 'rachel.green@barwonhealth.org.au', '03-4215-1012', 'Gastroenterology', 11);
    
    DECLARE @NewDoctorID INT = SCOPE_IDENTITY();
    
    INSERT INTO Patient (UR, Name, Address, Age, Email, Phone, MedicareCardNumber, PrimaryDoctorID)
    VALUES ('UR016789', 'Chris Evans', '412 Shannon Avenue, Geelong VIC 3220', 39, 'chris.evans@email.com', '0445-555-004', '7456789012', @NewDoctorID);
    
    COMMIT TRANSACTION;
    PRINT 'Transaction completed successfully';
END TRY
BEGIN CATCH
    ROLLBACK TRANSACTION;
    PRINT 'Transaction failed: ' + ERROR_MESSAGE();
END CATCH;

--
GO

-- View: Create a view that combines patient and doctor information for easy access
CREATE VIEW PatientDoctorView AS
SELECT 
    p.UR,
    p.Name AS PatientName,
    p.Age,
    p.Email AS PatientEmail,
    p.Phone AS PatientPhone,
    d.Name AS PrimaryDoctorName,
    d.Specialty,
    d.Email AS DoctorEmail,
    d.Phone AS DoctorPhone
FROM Patient p
JOIN Doctor d ON p.PrimaryDoctorID = d.ID;

--
GO

-- Query the view
SELECT * FROM PatientDoctorView;

-- Index: Create an index on the 'phone' column of the Patient table to improve search performance
CREATE INDEX IX_Patient_Phone ON Patient(Phone);

-- Backup: Perform a backup of the entire database to ensure data safety
-- Note: Replace 'BarwonHealth' with your actual database name and adjust the path as needed
BACKUP DATABASE task7 
TO DISK = 'C:\Backups\task7.bak'
WITH FORMAT, INIT, 
NAME = 'task7 Backup',
DESCRIPTION = 'task7 database';