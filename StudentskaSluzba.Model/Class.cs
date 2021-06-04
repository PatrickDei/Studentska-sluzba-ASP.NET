using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StudentskaSluzba.Model
{
    public class Class
    {
        public Class()
        {
            this.Professors = new HashSet<Professor>();
            this.Students = new HashSet<Student>();
        }

        [Key]
        public int ID { get; set; }

        [ForeignKey(nameof(Course))]
        public int CourseId { get; set; }

        [Required]
        public string Name { get; set; }

        public virtual ICollection<Professor> Professors { get; set; }

        public virtual ICollection<Student> Students { get; set; }
    }
}
