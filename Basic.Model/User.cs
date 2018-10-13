using Mongo.Repository;
using System.Collections.Generic;

namespace Basic.Model
{   
    [CollectionName("Users")]
    public class User : BaseModel
    {
        public string Email { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Phone { get; set; }
        public string PasswordHash { get; set; }
        public UserType Type { get; set; }
        public List<string> RefreshTokens { get; set; }
    }

    public enum UserType
    {
        SuperAdmin = 1,
        Admin = 2,
        Doctor = 3,
        Cl = 4
    }
}
