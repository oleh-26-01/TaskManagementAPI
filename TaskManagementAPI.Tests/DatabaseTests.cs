using Microsoft.EntityFrameworkCore;
using TaskManagementAPI.Data;
using TaskManagementAPI.Models;

public class DatabaseTests
{
    [Fact]
    public void CanAddUserAndRetrieveIt()
    {
        // Create an in-memory database for testing
        var options = new DbContextOptionsBuilder<TaskManagementDbContext>()
            .UseInMemoryDatabase(databaseName: "TestDatabase")
            .Options;

        // Create a new DbContext instance
        using (var context = new TaskManagementDbContext(options))
        {
            // Add a new user
            var user = new User { Username = "testuser", Email = "test@example.com", PasswordHash = "hashed_password" };
            context.Users.Add(user);
            context.SaveChanges();

            // Retrieve the user
            var retrievedUser = context.Users.Find(user.Id);

            // Assert that the retrieved user is the same as the added user
            Assert.Equal(user.Username, retrievedUser.Username);
        }
    }
}