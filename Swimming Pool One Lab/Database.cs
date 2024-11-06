using Dapper;
using MySqlConnector;
using Swimming_Pool_One_Lab.Models;
using System.Collections.ObjectModel;

namespace Swimming_Pool_One_Lab;

public static class Database
{
    private static readonly string MYSQL_CONNECTION_STRING = "Server=127.0.0.1;Database=swimming_pool_first_lab;Uid=root;Pwd=;";

    #region Client Queries

    public static async Task<ObservableCollection<Client>> GetAllClients()
    {
        using MySqlConnection connection = new(MYSQL_CONNECTION_STRING);
        string sql = "SELECT * FROM client;";
        IEnumerable<Client> clients = await connection.QueryAsync<Client>(sql);
        ObservableCollection<Client> result = [.. clients];
        return result;
    }

    public static async Task CreateClient(string firstName, string lastName, int age, string phoneNumber, string emailAddress)
    {
        using MySqlConnection connection = new(MYSQL_CONNECTION_STRING);
        string sql = @"INSERT INTO client (first_name, last_name, age, phone_number, email_address)
                            VALUES (@FirstName, @LastName, @Age, @PhoneNumber, @EmailAddress);";
        await connection.ExecuteAsync(sql, new { FirstName = firstName, LastName = lastName, Age = age, PhoneNumber = phoneNumber, EmailAddress = emailAddress });
    }

    public static async Task DeleteClient(int clientId)
    {
        using MySqlConnection connection = new(MYSQL_CONNECTION_STRING);
        string sql = "DELETE FROM client WHERE client_id = @ClientId;";
        await connection.ExecuteAsync(sql, new { ClientId = clientId });
    }

    public static async Task UpdateClient(int clientId, string firstName, string lastName, int age, string phoneNumber, string emailAddress)
    {
        using MySqlConnection connection = new(MYSQL_CONNECTION_STRING);
        string sql = @"UPDATE client 
                               SET first_name = @FirstName, last_name = @LastName, age = @Age, 
                                   phone_number = @PhoneNumber, email_address = @EmailAddress 
                               WHERE client_id = @ClientId;";
        await connection.ExecuteAsync(sql, new { ClientId = clientId, FirstName = firstName, LastName = lastName, Age = age, PhoneNumber = phoneNumber, EmailAddress = emailAddress });
    }

    #endregion

    #region Training Queries

    public static async Task CreateTraining(DateTime date, string training_type, string pool_name)
    {
        using MySqlConnection connection = new(MYSQL_CONNECTION_STRING);
        string sql = "SELECT client_id, instructor_id FROM client, instructor LIMIT 1";
        var result = await connection.QueryFirstAsync(sql);
        int client_id = result.client_id;
        int instructor_id = result.instructor_id;

        DateTime dateFormatted = new(date.Year, date.Month, date.Day);
        sql = @"INSERT INTO training (date, training_type, pool_name, client_id, instructor_id)
                        VALUES (@date, @training_type, @pool_name, @client_id, @instructor_id);";
        await connection.ExecuteAsync(sql, new { date = dateFormatted,training_type, pool_name, client_id, instructor_id });
    }

    public static async Task DeleteTraining(int trainingId)
    {
        using MySqlConnection connection = new(MYSQL_CONNECTION_STRING);
        string sql = "DELETE FROM training WHERE training_id = @TrainingId;";
        await connection.ExecuteAsync(sql, new { TrainingId = trainingId });
    }

    public static async Task UpdateTraining(int trainingId, DateTime date_training, string training_type, string pool_name, int client_id, int instructor_id)
    {
        using MySqlConnection connection = new(MYSQL_CONNECTION_STRING);

        string sql = @"UPDATE training 
                       SET date = @date, 
                           training_type = @training_type, 
                           pool_name = @pool_name, 
                           client_id = @client_id,
                           instructor_id = @instructor_id
                       WHERE training_id = @TrainingId;";

        await connection.ExecuteAsync(sql, new { TrainingId = trainingId, date = date_training, training_type, pool_name, client_id, instructor_id });
    }

    public static async Task<ObservableCollection<Training>> GetAllTrainings()
    {
        using MySqlConnection connection = new(MYSQL_CONNECTION_STRING);
        string sql = "SELECT training.*, client.first_name AS clientName, instructor.first_name AS instructorName FROM training LEFT JOIN client ON training.client_id = client.client_id LEFT JOIN instructor ON training.instructor_id = instructor.instructor_id ORDER BY training_id;";
        IEnumerable<Training> trainings = await connection.QueryAsync<Training>(sql);
        ObservableCollection<Training> result = [.. trainings];
        return result;
    }

    #endregion

    #region Instructor Queries

    public static async Task<ObservableCollection<Instructor>> GetAllInstructors()
    {
        using MySqlConnection connection = new(MYSQL_CONNECTION_STRING);
        string sql = "SELECT * FROM instructor;";
        IEnumerable<Instructor> instructors = await connection.QueryAsync<Instructor>(sql);
        ObservableCollection<Instructor> result = [.. instructors];
        return result;
    }

    public static async Task CreateInstructor(string firstName, string lastName, int age, string phoneNumber, string emailAddress, string specialization)
    {
        using MySqlConnection connection = new(MYSQL_CONNECTION_STRING);
        string sql = @"INSERT INTO instructor (first_name, last_name, age, phone_number, email_address, specialization)
                        VALUES (@FirstName, @LastName, @Age, @PhoneNumber, @EmailAddress, @Specialization);";
        await connection.ExecuteAsync(sql, new { FirstName = firstName, LastName = lastName, Age = age, PhoneNumber = phoneNumber, EmailAddress = emailAddress, Specialization = specialization });
    }

    public static async Task DeleteInstructor(int instructorId)
    {
        using MySqlConnection connection = new(MYSQL_CONNECTION_STRING);
        string sql = "DELETE FROM instructor WHERE instructor_id = @InstructorId;";
        await connection.ExecuteAsync(sql, new { InstructorId = instructorId });
    }

    public static async Task UpdateInstructor(int instructorId, string firstName, string lastName, int age, string phoneNumber, string emailAddress, string specialization)
    {
        using MySqlConnection connection = new(MYSQL_CONNECTION_STRING);
        string sql = @"UPDATE instructor 
                           SET first_name = @FirstName, last_name = @LastName, age = @Age, 
                               phone_number = @PhoneNumber, email_address = @EmailAddress, specialization = @Specialization
                           WHERE instructor_id = @InstructorId;";
        await connection.ExecuteAsync(sql, new { InstructorId = instructorId, FirstName = firstName, LastName = lastName, Age = age, PhoneNumber = phoneNumber, EmailAddress = emailAddress, Specialization = specialization });
    }

    #endregion
}