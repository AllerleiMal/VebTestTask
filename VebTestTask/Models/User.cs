﻿namespace VebTestTask.Models;

public class User
{
    public int Id { get; set; }
    public string Name { get; set; }
    public int Age { get; set; }
    public string Email { get; set; }
    public virtual ICollection<Role> Roles { get; set; }

    /// <summary>
    /// Copy all data except Id from another User instance
    /// </summary>
    /// <param name="user">Source User</param>
    public void ApplyChangesExceptId(User user)
    {
        Name = user.Name;
        Age = user.Age;
        Email = user.Email;
        Roles = user.Roles;
    }
}