
CREATE PROCEDURE PromoteStudents @Studies NVARCHAR(100), @Semester INT
AS
BEGIN
	SET XACT_ABORT ON;
	BEGIN TRAN
	DECLARE @IdStudies INT = (SELECT IdStudy FROM studies WHERE Name=@Studies);
	DECLARE @start DATE = '2020-10-01';

	IF @IdStudies IS NULL
	BEGIN
		RETURN;
	END

	DECLARE @IdEnrollment INT = (SELECT IdEnrollment FROM enrollment WHERE idStudy=@IdStudies AND Semester=@Semester);
	IF @IdEnrollment IS NULL
	BEGIN
		SET @IdEnrollment = (SELECT MAX(IdEnrollment) FROM enrollment);
		INSERT INTO Enrollment (IdEnrollment, IdStudy, Semester, StartDate) VALUES (@IdEnrollment+1, @IdStudies, @Semester, @start);
		SET @IdEnrollment=@IdEnrollment+1;
	END
	
	DECLARE @PrevIdEnrollment INT = (SELECT IdEnrollment FROM enrollment WHERE idStudy=@IdStudies AND Semester=@Semester-1);
	IF @PrevIdEnrollment IS NOT NULL
	BEGIN
			UPDATE student SET IdEnrollment=@IdEnrollment WHERE IdEnrollment=@PrevIdEnrollment;
	END
	COMMIT
END

/* uruchomienie z ręki np.

EXEC PromoteStudents 'Informatyka', 2;

*/
