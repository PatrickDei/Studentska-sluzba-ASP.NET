using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StudentskaSluzba.Model
{
    public class Student
    {
        public Student()
        {
            this.Classes = new HashSet<Class>();
        }

        [Key]
        public int ID { get; set; }

        [Required]
        public string FirstName { get; set; }

        [Required]
        public string LastName { get; set; }

        [Required]
        public DateTime DateOfBirth { get; set; }

        [Required]
        public DateTime DateOfEnrollment { get; set; }

        [Required]
        [RegularExpression("[0-9]{10}", ErrorMessage = "JMBAG must be exactly 10 numeric values")]
        public string JMBAG { get; set; }

        public virtual ICollection<Class> Classes { get; set; }

        [Required]
        [ForeignKey(nameof(Course))]
        public int CourseID { get; set; }
        public Course Course { get; set; }

        public void TryUpdateStudent(Student s)
        {
            this.JMBAG = s.JMBAG ?? this.JMBAG;
            this.FirstName = s.FirstName ?? this.FirstName;
            this.LastName = s.LastName ?? this.LastName;
            this.DateOfBirth = s.DateOfBirth ;
            this.DateOfEnrollment = s.DateOfEnrollment;
            this.CourseID = (s.CourseID != 0) ? this.CourseID : s.CourseID;
            this.Classes = s.Classes;
        }
    }
}
