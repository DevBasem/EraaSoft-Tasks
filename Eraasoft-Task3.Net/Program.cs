namespace Eraasoft_Task3.Net
{
    class Student
    {
        public string Name;
        int age;
        List<Course> courses;
        public int StudentId;

        public Student(string name, int age, List<Course> courses, int studentId)
        {
            this.Name = name;
            this.age = age;
            this.courses = courses;
            this.StudentId = studentId;
        }

        public bool Enroll(Course course)
        {
            if (courses == null)
                courses = new List<Course>();
            courses.Add(course);
            Console.WriteLine($"Student {Name} enrolled in course {course.Title}.");
            return true;
        }

        public bool isEnrolledIn(Course course)
        {
            if (courses == null) return false;
            for (int i = 0; i < courses.Count; i++)
            {
                if (courses[i].CourseId == course.CourseId)
                {
                    return true;
                }
            }
            return false;
        }

        public void PrintDetails()
        {
            Console.WriteLine($"Student Name: {Name}, Age: {age}, ID: {StudentId}");
            if (courses == null || courses.Count == 0)
            {
                Console.WriteLine("No courses enrolled.");
                return;
            }
            Console.WriteLine("Courses Enrolled:");
            for (int i = 0; i < courses.Count; i++)
            {
                Console.WriteLine($"- {courses[i].Title} (ID: {courses[i].CourseId})");
            }
        }
    }

    class Instructor
    {
        public string Name;
        string specialization;
        public int InstructorId;
        public List<Course> Courses;

        public Instructor(string name, string specialization, int instructorId, List<Course> courses)
        {
            this.Name = name;
            this.specialization = specialization;
            this.InstructorId = instructorId;
            this.Courses = courses;
        }

        public void PrintDetails()
        {
            Console.WriteLine($"Instructor Name: {Name}, Specialization: {specialization}, ID: {InstructorId}");
        }
    }

    class Course
    {
        public string Title;
        public int CourseId;
        public Instructor instructor;

        public Course(string title, int courseId, Instructor instructor)
        {
            this.Title = title;
            this.CourseId = courseId;
            this.instructor = instructor;
        }

        public void PrintDetails()
        {
            Console.WriteLine($"Course Title: {Title}, Course ID: {CourseId}");
            instructor.PrintDetails();
        }
    }

    class StudentManager
    {
        List<Student> students;
        List<Course> courses;
        List<Instructor> instructors;

        public StudentManager()
        {
            students = new List<Student>();
            courses = new List<Course>();
            instructors = new List<Instructor>();
        }

        public bool AddStudent(Student student)
        {
            if (students == null)
                students = new List<Student>();
            students.Add(student);
            return true;
        }

        public bool AddCourse(Course course)
        {
            if (courses == null)
                courses = new List<Course>();
            courses.Add(course);
            return true;
        }

        public bool AddInstructor(Instructor instructor)
        {
            if (instructors == null)
                instructors = new List<Instructor>();
            instructors.Add(instructor);
            return true;
        }

        public bool AddCourseToInstructor(int instructorId, Course course)
        {
            Instructor instructor = FindInstructor(instructorId);
            if (instructor == null)
            {
                Console.WriteLine("Instructor not found.");
                return false;
            }
            if (instructor.Courses == null)
                instructor.Courses = new List<Course>();
            instructor.Courses.Add(course);
            Console.WriteLine($"Course {course.Title} added to Instructor {instructor.Name}.");
            return true;
        }

        public Student FindStudent(int studentId)
        {
            if (students == null) return null;

            for (int i = 0; i < students.Count; i++)
            {
                if (students[i].StudentId == studentId)
                {
                    return students[i];
                }
            }

            return null;
        }

        public Student FindStudent(string studentName)
        {
            if (students == null) return null;
            if (studentName == null || studentName == "") return null;

            string nameToFind = studentName.ToLower();

            for (int i = 0; i < students.Count; i++)
            {
                if (students[i].Name != null && students[i].Name.ToLower() == nameToFind)
                {
                    return students[i];
                }
            }

            return null;
        }

        public Course FindCourse(int courseId)
        {
            if (courses == null) return null;

            for (int i = 0; i < courses.Count; i++)
            {
                if (courses[i].CourseId == courseId)
                {
                    return courses[i];
                }
            }

            return null;
        }

        public Course FindCourse(string courseName)
        {
            if (courses == null) return null;
            if (courseName == null || courseName == "") return null;

            string titleToFind = courseName.ToLower();

            for (int i = 0; i < courses.Count; i++)
            {
                if (courses[i].Title != null && courses[i].Title.ToLower() == titleToFind)
                {
                    return courses[i];
                }
            }

            return null;
        }

        public Instructor FindInstructor(int instructorId)
        {
            if (instructors == null) return null;

            for (int i = 0; i < instructors.Count; i++)
            {
                if (instructors[i].InstructorId == instructorId)
                {
                    return instructors[i];
                }
            }

            return null;
        }

        public Instructor FindInstructor(string instructorName)
        {
            if (instructors == null) return null;
            if (instructorName == null || instructorName == "") return null;

            string nameToFind = instructorName.ToLower();

            for (int i = 0; i < instructors.Count; i++)
            {
                if (instructors[i].Name != null && instructors[i].Name.ToLower() == nameToFind)
                {
                    return instructors[i];
                }
            }

            return null;
        }

        public bool EnrollStudentInCourse(int studentId, int courseId)
        {
            Student student = FindStudent(studentId);
            Course course = FindCourse(courseId);

            if (student == null || course == null)
            {
                Console.WriteLine("Student or Course not found.");
                return false;
            }

            return student.Enroll(course);
        }

        public void ShowAllStudents()
        {
            if (students == null || students.Count == 0)
            {
                Console.WriteLine("No students found.\n");
                return;
            }

            Console.WriteLine("All Students:");
            for (int i = 0; i < students.Count; i++)
            {
                students[i].PrintDetails();
                Console.WriteLine();
            }
        }

        public void ShowAllCourses()
        {
            if (courses == null || courses.Count == 0)
            {
                Console.WriteLine("No courses found.\n");
                return;
            }

            Console.WriteLine("All Courses:");
            for (int i = 0; i < courses.Count; i++)
            {
                courses[i].PrintDetails();
                Console.WriteLine();
            }
        }

        public void ShowAllInstructors()
        {
            if (instructors == null || instructors.Count == 0)
            {
                Console.WriteLine("No instructors found.\n");
                return;
            }

            Console.WriteLine("All Instructors:");
            for (int i = 0; i < instructors.Count; i++)
            {
                instructors[i].PrintDetails();
                Console.WriteLine();
            }
        }
    }

    internal class Program
    {
        static void Main(string[] args)
        {
            StudentManager studentManager = new StudentManager();
            bool isAppRunning = true;

            do
            {
                Console.WriteLine("Student Manager Console Menu");
                Console.WriteLine("===============================");
                Console.WriteLine("1. Add Student");
                Console.WriteLine("2. Add Instructor");
                Console.WriteLine("3. Add Course");
                Console.WriteLine("4. Enroll Student in Course");
                Console.WriteLine("5. Show All Students");
                Console.WriteLine("6. Show All Courses");
                Console.WriteLine("7. Show All Instructors");
                Console.WriteLine("8. Find the Student by id or name");
                Console.WriteLine("9. Find the Course by id or title");
                Console.WriteLine("10. Assign Course to Instructor");
                Console.WriteLine("11. Check if Student is enrolled in a Course");
                Console.WriteLine("12. Get Instructor name by course name");
                Console.WriteLine("0. Exit");
                Console.WriteLine("===============================");

                Console.Write("Enter your choice: ");
                int choice = int.Parse(Console.ReadLine());

                switch (choice)
                {
                    case 1:
                        Console.Write("Enter Student Name: ");
                        string studentName = Console.ReadLine();
                        Console.Write("Enter Student Age: ");
                        int studentAge = int.Parse(Console.ReadLine());
                        Console.Write("Enter Student ID: ");
                        int studentId = int.Parse(Console.ReadLine());
                        studentManager.FindStudent(studentId);
                        if (studentManager.FindStudent(studentId) != null)
                        {
                            Console.WriteLine("Student with this ID already exists. Please use a different ID.\n");
                            break;
                        }
                        Student student = new Student(studentName, studentAge, null, studentId);
                        studentManager.AddStudent(student);
                        Console.WriteLine("Student Added\n");
                        break;
                    case 2:
                        Console.Write("Enter Instructor Name: ");
                        string instructorName = Console.ReadLine();
                        Console.Write("Enter Instructor Specialization: ");
                        string instructorSpecialization = Console.ReadLine();
                        Console.Write("Enter Instructor ID: ");
                        int instructorId = int.Parse(Console.ReadLine());
                        if (studentManager.FindInstructor(instructorId) != null)
                        {
                            Console.WriteLine("Instructor with this ID already exists. Please use a different ID.\n");
                            break;
                        }
                        Instructor instructor = new Instructor(instructorName, instructorSpecialization, instructorId, null);
                        studentManager.AddInstructor(instructor);
                        Console.WriteLine("Instructor Added\n");
                        break;
                    case 3:
                        Console.Write("Enter Course Title: ");
                        string courseTitle = Console.ReadLine();
                        Console.Write("Enter Course ID: ");
                        int courseId = int.Parse(Console.ReadLine());
                        Console.Write("Enter Instructor ID for this Course: ");
                        int courseInstructorId = int.Parse(Console.ReadLine());
                        Instructor courseInstructor = studentManager.FindInstructor(courseInstructorId);
                        if (courseInstructor == null)
                        {
                            Console.WriteLine("Instructor not found. Course not added.\n");
                            return;
                        }
                        Course course = new Course(courseTitle, courseId, courseInstructor);
                        studentManager.AddCourse(course);
                        Console.WriteLine("Course Added\n");
                        break;
                    case 4:
                        Console.Write("Enter Student ID to enroll: ");
                        int enrollStudentId = int.Parse(Console.ReadLine());
                        Console.Write("Enter Course ID to enroll in: ");
                        int enrollCourseId = int.Parse(Console.ReadLine());

                        if (!studentManager.EnrollStudentInCourse(enrollStudentId, enrollCourseId))
                        {
                            Console.WriteLine("Enrollment failed.\n");
                        }
                        break;
                    case 5:
                        studentManager.ShowAllStudents();
                        break;
                    case 6:
                        studentManager.ShowAllCourses();
                        break;
                    case 7:
                        studentManager.ShowAllInstructors();
                        break;
                    case 8:
                        Console.WriteLine("Find Student By: ");
                        Console.WriteLine("1. ID");
                        Console.WriteLine("2. Name");
                        int findStudentChoice = int.Parse(Console.ReadLine());
                        if (findStudentChoice == 1)
                        {
                            Console.Write("Enter Student ID: ");
                            int findStudentId = int.Parse(Console.ReadLine());
                            Student foundStudentById = studentManager.FindStudent(findStudentId);
                            if (foundStudentById != null)
                            {
                                foundStudentById.PrintDetails();
                            }
                            else
                            {
                                Console.WriteLine("Student not found.\n");
                            }
                        }
                        else if (findStudentChoice == 2)
                        {
                            Console.Write("Enter Student Name: ");
                            string findStudentName = Console.ReadLine();
                            Student foundStudentByName = studentManager.FindStudent(findStudentName);
                            if (foundStudentByName != null)
                            {
                                foundStudentByName.PrintDetails();
                            }
                            else
                            {
                                Console.WriteLine("Student not found.\n");
                            }
                        }
                        else
                        {
                            Console.WriteLine("Invalid choice.\n");
                        }
                        break;
                    case 9:
                        Console.WriteLine("Find Course By: ");
                        Console.WriteLine("1. ID");
                        Console.WriteLine("2. Title");
                        int findCourseChoice = int.Parse(Console.ReadLine());
                        if (findCourseChoice == 1)
                        {
                            Console.Write("Enter Course ID: ");
                            int findCourseId = int.Parse(Console.ReadLine());
                            Course foundCourseById = studentManager.FindCourse(findCourseId);
                            if (foundCourseById != null)
                            {
                                foundCourseById.PrintDetails();
                            }
                            else
                            {
                                Console.WriteLine("Course not found.\n");
                            }
                        }
                        else if (findCourseChoice == 2)
                        {
                            Console.Write("Enter Course Title: ");
                            string findCourseTitle = Console.ReadLine();
                            Course foundCourseByTitle = studentManager.FindCourse(findCourseTitle);
                            if (foundCourseByTitle != null)
                            {
                                foundCourseByTitle.PrintDetails();
                            }
                            else
                            {
                                Console.WriteLine("Course not found.\n");
                            }
                        }
                        else
                        {
                            Console.WriteLine("Invalid choice.\n");
                        }
                        break;
                    case 10:
                        Console.WriteLine("assign course to instructor");
                        Console.Write("Enter Instructor ID: ");
                        int assignInstructorId = int.Parse(Console.ReadLine());
                        Console.Write("Enter Course ID to assign: ");
                        int assignCourseId = int.Parse(Console.ReadLine());
                        if (!studentManager.AddCourseToInstructor(assignInstructorId, studentManager.FindCourse(assignCourseId)))
                        {
                            Console.WriteLine("Assignment failed.\n");
                            return;
                        }
                        break;
                    case 11:
                        Console.WriteLine("Enter Student ID to check enrollment: ");
                        int checkStudentId = int.Parse(Console.ReadLine());
                        Console.WriteLine("Enter Course ID to check enrollment: ");
                        int checkCourseId = int.Parse(Console.ReadLine());
                        Student checkStudent = studentManager.FindStudent(checkStudentId);
                        Course checkCourse = studentManager.FindCourse(checkCourseId);
                        if (checkStudent == null || checkCourse == null)
                        {
                            Console.WriteLine("Student or Course not found.\n");
                        }
                        else if (checkStudent.isEnrolledIn(checkCourse))
                        {
                            Console.WriteLine($"Student {checkStudent.Name} is enrolled in course {checkCourse.Title}.\n");
                        }
                        else
                        {
                            Console.WriteLine($"Student {checkStudent.Name} is NOT enrolled in course {checkCourse.Title}.\n");
                        }
                        break;

                    case 12:
                        Console.WriteLine("Enter Course Title to get Instructor name: ");
                        string courseTitleForInstructor = Console.ReadLine();
                        Course courseForInstructor = studentManager.FindCourse(courseTitleForInstructor);
                        if (courseForInstructor != null)
                        {
                            Console.WriteLine($"Instructor for course {courseForInstructor.Title} is {courseForInstructor.instructor.Name}.\n");
                        }
                        else
                        {
                            Console.WriteLine("Course not found.\n");
                        }
                        break;
                    case 0:
                        Console.WriteLine("Exiting...");
                        isAppRunning = false;
                        return;
                    default:
                        Console.WriteLine("Invalid choice. Please try again.\n");
                        break;
                }
            } while (isAppRunning);
        }
    }
}