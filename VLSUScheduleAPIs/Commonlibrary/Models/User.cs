using System;
using System.Collections.Generic;
using System.Text;

namespace CommonLibrary.Models
{
    public enum UserType
    {
        Teacher,
        Student
    }

    public class User
    {
        public int ID { get; set; }
        public string FIO { get; set; }
        public DateTime Birthday { get; set; }
        public DateTime LastActiveDateTime { get; set; }
        public string Login { get; set; }
        public UserType UserType { get; set; }
    }
}
