using Dapper;
using MySqlConnector;
using Swimming_Pool.Models;
using System.Collections.ObjectModel;
using System.Windows.Controls;
using Xceed.Wpf.Toolkit;

namespace Swimming_Pool;

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

    public static async Task<Client?> GetClientById(int clientId)
    {
        using MySqlConnection connection = new(MYSQL_CONNECTION_STRING);
        string sql = "SELECT * FROM client WHERE client_id = @ClientId;";
        Client? client = await connection.QueryFirstOrDefaultAsync<Client>(sql, new { ClientId = clientId });
        return client;
    }

    public static async Task<ObservableCollection<Client>> GetClientsFiltered(string firstName, string lastName, string age, string phoneNumber, string email)
    {
        bool first = true;
        using MySqlConnection connection = new(MYSQL_CONNECTION_STRING);
        string sql = "SELECT * FROM client";
        if (!string.IsNullOrEmpty(firstName))
        {
            sql += " WHERE first_name LIKE @FirstName";
            first = false;
            firstName = "%" + firstName + "%";
        }
        if (!string.IsNullOrEmpty(lastName))
        {
            if (first)
            {
                sql += " WHERE last_name LIKE @LastName";
            }
            else
            {
                sql += " AND last_name LIKE @LastName";
            }
            lastName = "%" + lastName + "%";
            first = false;
        }
        if (!string.IsNullOrEmpty(age))
        {
            if (first)
            {
                sql += " WHERE age LIKE @Age";
            }
            else
            {
                sql += " AND age LIKE @Age";
            }
            age = "%" + age + "%";
            first = false;
        }
        if (!string.IsNullOrEmpty(phoneNumber))
        {
            if (first)
            {
                sql += " WHERE phone_number LIKE @PhoneNumber";
            }
            else
            {
                sql += " AND phone_number LIKE @PhoneNumber";
            }
            phoneNumber = "%" + phoneNumber + "%";
            first = false;
        }
        if (!string.IsNullOrEmpty(email))
        {
            if (first)
            {
                sql += " WHERE email_address LIKE @EmailAddress";
            }
            else
            {
                sql += " AND email_address LIKE @EmailAddress";
            }
            email = "%" + email + "%";
            first = false;
        }
        IEnumerable<Client> clients = await connection.QueryAsync<Client>(sql, new { FirstName = firstName, LastName = lastName, Age = age, PhoneNumber = phoneNumber, EmailAddress = email });
        ObservableCollection<Client> result = [.. clients];
        return result;
    }

    #endregion

    #region Training Queries

    public static async Task CreateTraining(DateTime dateActual, string training_type, string pool_name, int client_id, int instructor_id)
    {
        using MySqlConnection connection = new(MYSQL_CONNECTION_STRING);
        //string sql = "SELECT client_id, instructor_id FROM client, instructor LIMIT 1";
        //dynamic result = await connection.QueryFirstAsync(sql);
        //int client_id = result.client_id;
        //int instructor_id = result.instructor_id;

        string sql = @"INSERT INTO training (date, training_type, pool_name, client_id, instructor_id)
                        VALUES (@date, @training_type, @pool_name, @client_id, @instructor_id);";
        await connection.ExecuteAsync(sql, new { date = dateActual, training_type, pool_name, client_id, instructor_id });
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

    public static async Task<Training?> GetTrainingById(int trainingId)
    {
        using MySqlConnection connection = new(MYSQL_CONNECTION_STRING);
        string sql = "SELECT * FROM training WHERE training_id = @TrainingId;";
        Training? training = await connection.QueryFirstOrDefaultAsync<Training>(sql, new { TrainingId = trainingId });
        return training;
    }

    public static async Task<ObservableCollection<Training>> GetTrainingFiltered(int? year, int? month, int? day, string trainingType, string poolName, int? clientId, int? instructorId)
    {
        bool first = true;
        using MySqlConnection connection = new(MYSQL_CONNECTION_STRING);
        string sql = "SELECT training.*, client.first_name AS clientName, instructor.first_name AS instructorName FROM training LEFT JOIN client ON training.client_id = client.client_id LEFT JOIN instructor ON training.instructor_id = instructor.instructor_id";
        string date = string.Empty;
        if (year != null)
        {
            if (first)
            {
                sql += " WHERE YEAR(date) = @Year";
            }
            else
            {
                sql += " AND YEAR(date) = @Year";
            }
            first = false;
        }
        if (month != null)
        {
            if (first)
            {
                sql += " WHERE MONTH(date) = @Month";
            }
            else
            {
                sql += " AND MONTH(date) = @Month";
            }
            first = false;
        }
        if (day != null)
        {
            if (first)
            {
                sql += " WHERE DAY(date) = @Day";
            }
            else
            {
                sql += " AND DAY(date) = @Day";
            }
            first = false;
        }
        if (!string.IsNullOrEmpty(trainingType))
        {
            if (first)
            {
                sql += " WHERE training_type LIKE @TrainingType";
            }
            else
            {
                sql += " AND training_type LIKE @TrainingType";
            }
            trainingType = "%" + trainingType + "%";
            first = false;
        }
        if (!string.IsNullOrEmpty(poolName))
        {
            if (first)
            {
                sql += " WHERE pool_name LIKE @PoolName";
            }
            else
            {
                sql += " AND pool_name LIKE @PoolName";
            }
            poolName = "%" + poolName + "%";
            first = false;
        }
        if (clientId != null && clientId != -1)
        {
            if (first)
            {
                sql += " WHERE client.client_id = @ClientId";
            }
            else
            {
                sql += " AND client.client_id = @ClientId";
            }
            first = false;
        }
        if (instructorId != null && instructorId != -1)
        {
            if (first)
            {
                sql += " WHERE instructor.instructor_id = @InstructorId";
            }
            else
            {
                sql += " AND instructor.instructor_id = @InstructorId";
            }
            first = false;
        }
        sql += " ORDER BY training_id";
        IEnumerable<Training> trainings = await connection.QueryAsync<Training>(sql, new { Year = year, Month = month, Day = day, TrainingType = trainingType, PoolName = poolName, ClientId = clientId, InstructorId = instructorId });
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

    public static async Task<Instructor?> GetInstructorById(int instructorId)
    {
        using MySqlConnection connection = new(MYSQL_CONNECTION_STRING);
        string sql = "SELECT * FROM instructor WHERE instructor_id = @InstructorId;";
        Instructor? instructor = await connection.QueryFirstOrDefaultAsync<Instructor>(sql, new { InstructorId = instructorId });
        return instructor;
    }

    public static async Task<ObservableCollection<Instructor>> GetInstructorsFiltered(string firstName, string lastName, string age, string phoneNumber, string email, string specialization)
    {
        bool first = true;
        using MySqlConnection connection = new(MYSQL_CONNECTION_STRING);
        string sql = "SELECT * FROM instructor";
        if (!string.IsNullOrEmpty(firstName))
        {
            sql += " WHERE first_name LIKE @FirstName";
            first = false;
            firstName = "%" + firstName + "%";
        }
        if (!string.IsNullOrEmpty(lastName))
        {
            if (first)
            {
                sql += " WHERE last_name LIKE @LastName";
            }
            else
            {
                sql += " AND last_name LIKE @LastName";
            }
            lastName = "%" + lastName + "%";
            first = false;
        }
        if (!string.IsNullOrEmpty(age))
        {
            if (first)
            {
                sql += " WHERE age LIKE @Age";
            }
            else
            {
                sql += " AND age LIKE @Age";
            }
            age = "%" + age + "%";
            first = false;
        }
        if (!string.IsNullOrEmpty(phoneNumber))
        {
            if (first)
            {
                sql += " WHERE phone_number LIKE @PhoneNumber";
            }
            else
            {
                sql += " AND phone_number LIKE @PhoneNumber";
            }
            phoneNumber = "%" + phoneNumber + "%";
            first = false;
        }
        if (!string.IsNullOrEmpty(email))
        {
            if (first)
            {
                sql += " WHERE email_address LIKE @EmailAddress";
            }
            else
            {
                sql += " AND email_address LIKE @EmailAddress";
            }
            email = "%" + email + "%";
            first = false;
        }
        if (!string.IsNullOrEmpty(specialization))
        {
            if (first)
            {
                sql += " WHERE specialization LIKE @Specialization";
            }
            else
            {
                sql += " AND specialization LIKE @Specialization";
            }
            specialization = "%" + specialization + "%";
            first = false;
        }
        IEnumerable<Instructor> instructors = await connection.QueryAsync<Instructor>(sql, new { FirstName = firstName, LastName = lastName, Age = age, PhoneNumber = phoneNumber, EmailAddress = email });
        ObservableCollection<Instructor> result = [.. instructors];
        return result;
    }

    #endregion

    #region Statistics

    public static async Task<string> CalculateClientAgeStatistics()
    {
        using MySqlConnection connection = new(MYSQL_CONNECTION_STRING);
        string sql = "SELECT AVG(age) FROM client;";
        var tmp = await connection.ExecuteScalarAsync(sql);
        if (tmp is decimal result) return result.ToString("F2");
        return "";
    }

    public static async Task<string> CalculateInstructorAgeStatistics()
    {
        using MySqlConnection connection = new(MYSQL_CONNECTION_STRING);
        string sql = "SELECT AVG(age) FROM instructor;";
        var tmp = await connection.ExecuteScalarAsync(sql);
        if (tmp is decimal result) return result.ToString("F2");
        return "";
    }

    public static async Task<string> CalculateNumClientStatistics(int instructorId)
    {
        using MySqlConnection connection = new(MYSQL_CONNECTION_STRING);
        string sql = "SELECT COUNT(training.client_id) FROM training WHERE training.instructor_id = @InstructorId";
        var tmp = await connection.ExecuteScalarAsync(sql, new { InstructorId = instructorId });
        if (tmp is long result) return result.ToString();
        return "";
    }

    public static async Task<string> CalculateNumTrainingsPerMonth(int month)
    {
        using MySqlConnection connection = new(MYSQL_CONNECTION_STRING);
        string sql = "SELECT COUNT(training.training_id) FROM training WHERE MONTH(training.date) = @Month";
        var tmp = await connection.ExecuteScalarAsync(sql, new { Month = month });
        if (tmp is long result) return result.ToString();
        return "";
    }

    #endregion

    public static async Task<IEnumerable<dynamic>> ExecuteQuery(string query)
    {
        using MySqlConnection connection = new(MYSQL_CONNECTION_STRING);
        return await connection.QueryAsync(query);
    }
}