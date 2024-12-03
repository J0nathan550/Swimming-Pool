using Dapper;
using MySqlConnector;
using Swimming_Pool.Models;
using System.Collections.ObjectModel;

namespace Swimming_Pool;

public static class Database
{
    public static readonly string MYSQL_CONNECTION_STRING = "Server=127.0.0.1;Database=swimming_pool;Uid=root;Pwd=;";

    #region Subscription Queries

    public static async Task<ObservableCollection<Subscription>> GetSubscriptionsFiltered(string subscriptionType, string visitCount, string price, string startDate, string endDate, string clientName)
    {
        using MySqlConnection connection = new(MYSQL_CONNECTION_STRING);

        // Start the base SQL query for subscriptions
        string sql = "SELECT * FROM subscription WHERE 1=1";

        // Add filters based on the inputs
        if (!string.IsNullOrEmpty(subscriptionType))
            sql += " AND subscription_type LIKE @SubscriptionType";
        if (!string.IsNullOrEmpty(visitCount))
            sql += " AND visit_count LIKE @VisitCount";
        if (!string.IsNullOrEmpty(price))
            sql += " AND price LIKE @Price";
        if (!string.IsNullOrEmpty(startDate))
            sql += " AND start_date LIKE @StartDate";
        if (!string.IsNullOrEmpty(endDate))
            sql += " AND end_date LIKE @EndDate";

        // Filter by client name
        if (!string.IsNullOrEmpty(clientName))
        {
            // Subquery to get client_id by client name
            sql += " AND client_id IN (SELECT client_id FROM client WHERE CONCAT(first_name, ' ', last_name) LIKE @ClientName)";
        }

        // Execute the query
        IEnumerable<Subscription> subscriptions = await connection.QueryAsync<Subscription>(sql, new
        {
            SubscriptionType = $"%{subscriptionType}%",
            VisitCount = $"%{visitCount}%",
            Price = $"%{price}%",
            StartDate = $"%{startDate}%",
            EndDate = $"%{endDate}%",
            ClientName = $"%{clientName}%"
        });

        foreach (Subscription subscription in subscriptions)
        {
            await subscription.SetClientNameAsync();
        }

        return new ObservableCollection<Subscription>(subscriptions);
    }

    public static async Task DeleteSubscription(int subscriptionId)
    {
        using MySqlConnection connection = new(MYSQL_CONNECTION_STRING);
        string sql = "DELETE FROM subscription WHERE subscription_id = @SubscriptionId";
        await connection.ExecuteAsync(sql, new { SubscriptionId = subscriptionId });
    }

    public static async Task<ObservableCollection<Subscription>> GetAllSubscriptions()
    {
        using MySqlConnection connection = new(MYSQL_CONNECTION_STRING);
        string sql = "SELECT * FROM subscription";
        IEnumerable<Subscription> subscriptions = await connection.QueryAsync<Subscription>(sql);
        foreach (Subscription subscription in subscriptions)
        {
            await subscription.SetClientNameAsync();
        }
        return new ObservableCollection<Subscription>(subscriptions);
    }

    public static async Task CreateSubscription(string subscriptionType, int visitCount, float price, DateTime startDate, DateTime endDate, int clientId)
    {
        using MySqlConnection connection = new(MYSQL_CONNECTION_STRING);
        string sql = @"INSERT INTO subscription (subscription_type, visit_count, price, start_date, end_date, client_id)
                   VALUES (@SubscriptionType, @VisitCount, @Price, @StartDate, @EndDate, @ClientId)";
        await connection.ExecuteAsync(sql, new
        {
            SubscriptionType = subscriptionType,
            VisitCount = visitCount,
            Price = price,
            StartDate = startDate,
            EndDate = endDate,
            ClientId = clientId
        });
    }

    public static async Task UpdateSubscription(int subscriptionId, string subscriptionType, int visitCount, float price, DateTime startDate, DateTime endDate, int clientId)
    {
        using MySqlConnection connection = new(MYSQL_CONNECTION_STRING);
        string sql = @"UPDATE subscription 
                   SET subscription_type = @SubscriptionType, 
                       visit_count = @VisitCount, 
                       price = @Price, 
                       start_date = @StartDate, 
                       end_date = @EndDate, 
                       client_id = @ClientId 
                   WHERE subscription_id = @SubscriptionId";
        await connection.ExecuteAsync(sql, new
        {
            SubscriptionId = subscriptionId,
            SubscriptionType = subscriptionType,
            VisitCount = visitCount,
            Price = price,
            StartDate = startDate,
            EndDate = endDate,
            ClientId = clientId
        });
    }

    public static async Task<Subscription?> GetSubscriptionById(int subscriptionID)
    {
        using MySqlConnection connection = new(MYSQL_CONNECTION_STRING);
        string sql = "SELECT * FROM subscription WHERE subscription_id = @SubscriptionID";
        return await connection.QueryFirstOrDefaultAsync<Subscription>(sql, new { SubscriptionID = subscriptionID });
    }

    #endregion

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

    public static async Task<string> GetClientNameByIdAsync(int clientId)
    {
        using MySqlConnection connection = new(MYSQL_CONNECTION_STRING);
        string sql = "SELECT CONCAT(first_name, ' ', last_name) FROM client WHERE client_id = @ClientId;";
        string? clientName = await connection.ExecuteScalarAsync<string>(sql, new { ClientId = clientId });
        return clientName ?? string.Empty;
    }

    #endregion

    #region Training Queries

    public static async Task CreateTraining(DateTime dateActual, string training_type, int pool_id, int instructor_id, List<int> client_ids)
    {
        using MySqlConnection connection = new(MYSQL_CONNECTION_STRING);

        // Insert the training
        string trainingSql = @"
        INSERT INTO training (date, training_type, pool_id, instructor_id)
        VALUES (@Date, @TrainingType, @PoolId, @InstructorId);
        SELECT LAST_INSERT_ID();";

        int trainingId = await connection.ExecuteScalarAsync<int>(trainingSql, new
        {
            Date = dateActual,
            TrainingType = training_type,
            PoolId = pool_id,
            InstructorId = instructor_id
        });

        // Insert the client enrollments
        string enrollmentSql = "INSERT INTO client_training_enrollment (training_id, client_id) VALUES (@TrainingId, @ClientId);";

        foreach (int clientId in client_ids)
        {
            await connection.ExecuteAsync(enrollmentSql, new { TrainingId = trainingId, ClientId = clientId });
        }
    }

    public static async Task DeleteTraining(int trainingId)
    {
        using MySqlConnection connection = new(MYSQL_CONNECTION_STRING);

        // Delete client enrollments
        string deleteEnrollmentsSql = "DELETE FROM client_training_enrollment WHERE training_id = @TrainingId;";
        await connection.ExecuteAsync(deleteEnrollmentsSql, new { TrainingId = trainingId });

        // Delete the training
        string deleteTrainingSql = "DELETE FROM training WHERE training_id = @TrainingId;";
        await connection.ExecuteAsync(deleteTrainingSql, new { TrainingId = trainingId });
    }

    public static async Task UpdateTraining(int trainingId, DateTime date_training, string training_type, int pool_id, int instructor_id)
    {
        using MySqlConnection connection = new(MYSQL_CONNECTION_STRING);

        // Update training details, including training_type
        string updateTrainingSql = @"
        UPDATE training
        SET 
            date = @Date,
            training_type = @TrainingType,
            pool_id = @PoolId,
            instructor_id = @InstructorId
        WHERE 
            training_id = @TrainingId;";

        await connection.ExecuteAsync(updateTrainingSql, new
        {
            TrainingId = trainingId,
            Date = date_training,
            TrainingType = training_type,
            PoolId = pool_id,
            InstructorId = instructor_id
        });
    }

    public static async Task<ObservableCollection<Training>> GetAllTrainings()
    {
        using MySqlConnection connection = new(MYSQL_CONNECTION_STRING);
        string sql = @"
            SELECT 
                training.*, 
                instructor.first_name AS instructorName, 
                GROUP_CONCAT(client.first_name SEPARATOR ', ') AS clientNames 
            FROM 
                training 
            LEFT JOIN 
                instructor ON training.instructor_id = instructor.instructor_id
            LEFT JOIN 
                client_training_enrollment ON training.training_id = client_training_enrollment.training_id
            LEFT JOIN 
                client ON client_training_enrollment.client_id = client.client_id
            GROUP BY 
                training.training_id
            ORDER BY 
                training.training_id;
        ";

        IEnumerable<Training> trainings = await connection.QueryAsync<Training>(sql);

        foreach (Training training in trainings) 
        {
            sql = "SELECT CONCAT(c.first_name, \" \", c.last_name) AS clientName FROM client as c LEFT JOIN client_training_enrollment AS e ON c.client_id = e.client_id WHERE e.training_id = @TrainingId";
            var result = await connection.QueryAsync<string>(sql, new { training.TrainingId });
            string clientNames = "";
            foreach (var name in result)
            {
                if (clientNames.Length > 0)
                {
                    clientNames += ", ";
                }
                clientNames += name;
            }
            training.ClientNames = clientNames;

            sql = "SELECT name FROM pool WHERE pool_id = @PoolId";
            string? poolName = await connection.QueryFirstAsync<string>(sql, new { training.PoolId });
            training.PoolName = poolName;
        }

        return new ObservableCollection<Training>(trainings);
    }

    public static async Task<Training?> GetTrainingById(int trainingId)
    {
        using MySqlConnection connection = new(MYSQL_CONNECTION_STRING);

        string trainingSql = @"
            SELECT 
                training.*, 
                instructor.first_name AS instructorName, 
                GROUP_CONCAT(client.first_name SEPARATOR ', ') AS clientNames 
            FROM 
                training 
            LEFT JOIN 
                instructor ON training.instructor_id = instructor.instructor_id
            LEFT JOIN 
                client_training_enrollment ON training.training_id = client_training_enrollment.training_id
            LEFT JOIN 
                client ON client_training_enrollment.client_id = client.client_id
            WHERE 
                training.training_id = @TrainingId
            GROUP BY 
                training.training_id;";

        return await connection.QueryFirstOrDefaultAsync<Training>(trainingSql, new { TrainingId = trainingId });
    }

    public static async Task<ObservableCollection<Training>> GetTrainingFiltered(
    int? year,
    int? month,
    int? day,
    string trainingType,
    string clientNames,
    int? poolId,
    int? instructorId)
    {
        using MySqlConnection connection = new(MYSQL_CONNECTION_STRING);

        string sql = @"
    SELECT 
        training.*, 
        instructor.first_name AS instructorName
    FROM 
        training 
    LEFT JOIN 
        instructor ON training.instructor_id = instructor.instructor_id";

        // Apply filters
        List<string> conditions = [];
        if (year.HasValue) conditions.Add("YEAR(training.date) = @Year");
        if (month.HasValue) conditions.Add("MONTH(training.date) = @Month");
        if (day.HasValue) conditions.Add("DAY(training.date) = @Day");
        if (!string.IsNullOrWhiteSpace(trainingType)) conditions.Add("training.training_type LIKE @TrainingType");
        if (poolId.HasValue && poolId != -1) conditions.Add("training.pool_id LIKE @PoolId");
        if (instructorId.HasValue && instructorId != -1) conditions.Add("training.instructor_id = @InstructorId");

        if (conditions.Count > 0)
        {
            sql += " WHERE " + string.Join(" AND ", conditions);
        }

        sql += " GROUP BY training.training_id ORDER BY training.training_id;";

        IEnumerable<Training> trainings = await connection.QueryAsync<Training>(sql, new
        {
            Year = year,
            Month = month,
            Day = day,
            TrainingType = $"%{trainingType}%",
            PoolId = poolId,
            InstructorId = instructorId
        });

        List<Training> trainingResult = [];

        foreach (Training training in trainings)
        {
            // Get client names for each training
            sql = "SELECT CONCAT(c.first_name, ' ', c.last_name) AS clientName FROM client as c LEFT JOIN client_training_enrollment AS e ON c.client_id = e.client_id WHERE e.training_id = @TrainingId";
            var clientNamesResult = await connection.QueryAsync<string>(sql, new { training.TrainingId });

            // Concatenate client names
            string clientNamesConcatenated = string.Join(", ", clientNamesResult);

            // Only filter by client names if provided
            bool clientNameMatch = string.IsNullOrEmpty(clientNames) || clientNamesConcatenated.Contains(clientNames, StringComparison.OrdinalIgnoreCase);

            // If a match is found for client names, add to the result list
            if (clientNameMatch)
            {
                training.ClientNames = clientNamesConcatenated;

                // Get pool name
                sql = "SELECT name FROM pool WHERE pool_id = @PoolId";
                string? poolName = await connection.QueryFirstAsync<string>(sql, new { training.PoolId });
                training.PoolName = poolName;

                trainingResult.Add(training);
            }
        }

        return new ObservableCollection<Training>(trainingResult);
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

    #region Pool Queries

    public static async Task UpdatePool(Pool pool)
    {
        using MySqlConnection connection = new(MYSQL_CONNECTION_STRING);
        string sql = @"UPDATE pool 
                   SET name = @Name, 
                       lane_count = @LaneCount, 
                       length = @Length, 
                       depth = @Depth, 
                       address = @Address 
                   WHERE pool_id = @PoolId";
        await connection.ExecuteAsync(sql, new
        {
            pool.PoolId,
            pool.Name,
            pool.LaneCount,
            pool.Length,
            pool.Depth,
            pool.Address
        });
    }

    public static async Task<ObservableCollection<Pool>> GetPoolsFiltered(string poolName, string laneCount, string poolLength, string poolDepth, string poolAddress)
    {
        using MySqlConnection connection = new(MYSQL_CONNECTION_STRING);
        string sql = "SELECT * FROM pool WHERE 1=1";

        // Apply filters if the respective parameters are provided
        if (!string.IsNullOrEmpty(poolName))
            sql += " AND name LIKE @PoolName";
        if (!string.IsNullOrEmpty(laneCount))
            sql += " AND lane_count LIKE @LaneCount";
        if (!string.IsNullOrEmpty(poolLength))
            sql += " AND length LIKE @PoolLength";
        if (!string.IsNullOrEmpty(poolDepth))
            sql += " AND depth LIKE @PoolDepth";
        if (!string.IsNullOrEmpty(poolAddress))
            sql += " AND address LIKE @PoolAddress";

        // Execute the query with the specified parameters
        IEnumerable<Pool> pools = await connection.QueryAsync<Pool>(sql, new
        {
            PoolName = $"%{poolName}%",
            LaneCount = $"%{laneCount}%",
            PoolLength = $"%{poolLength}%",
            PoolDepth = $"%{poolDepth}%",
            PoolAddress = $"%{poolAddress}%"
        });

        // Return the result as an ObservableCollection
        return new ObservableCollection<Pool>(pools);
    }

    public static async Task DeletePool(int poolId)
    {
        using MySqlConnection connection = new(MYSQL_CONNECTION_STRING);
        string sql = "DELETE FROM pool WHERE pool_id = @PoolId";
        await connection.ExecuteAsync(sql, new { PoolId = poolId });
    }

    public static async Task<ObservableCollection<Pool>> GetAllPools()
    {
        using MySqlConnection connection = new(MYSQL_CONNECTION_STRING);
        string sql = "SELECT * FROM pool";
        IEnumerable<Pool> pools = await connection.QueryAsync<Pool>(sql);
        return new ObservableCollection<Pool>(pools);
    }

    public static async Task CreatePool(string name, int laneCount, float length, float depth, string address)
    {
        using MySqlConnection connection = new(MYSQL_CONNECTION_STRING);
        string sql = @"INSERT INTO pool (name, lane_count, length, depth, address)
                   VALUES (@Name, @LaneCount, @Length, @Depth, @Address)";
        await connection.ExecuteAsync(sql, new
        {
            Name = name,
            LaneCount = laneCount,
            Length = length,
            Depth = depth,
            Address = address
        });
    }

    public static async Task<Pool?> GetPoolById(int poolID)
    {
        using MySqlConnection connection = new(MYSQL_CONNECTION_STRING);
        string sql = "SELECT * FROM pool WHERE pool_id = @PoolId";
        return await connection.QueryFirstOrDefaultAsync<Pool>(sql, new { PoolId = poolID });
    }

    #endregion

    #region Statistics

    public static async Task<string> CalculateClientAgeStatistics()
    {
        using MySqlConnection connection = new(MYSQL_CONNECTION_STRING);
        string sql = "SELECT AVG(age) FROM client;";
        object? tmp = await connection.ExecuteScalarAsync(sql);
        if (tmp is decimal result) return result.ToString("F2");
        return "";
    }

    public static async Task<string> CalculateInstructorAgeStatistics()
    {
        using MySqlConnection connection = new(MYSQL_CONNECTION_STRING);
        string sql = "SELECT AVG(age) FROM instructor;";
        object? tmp = await connection.ExecuteScalarAsync(sql);
        if (tmp is decimal result) return result.ToString("F2");
        return "";
    }

    public static async Task<string> CalculateNumClientStatistics(int instructorId)
    {
        using MySqlConnection connection = new(MYSQL_CONNECTION_STRING);
        string sql = @"
        SELECT COUNT(DISTINCT cte.client_id) 
        FROM training t
        JOIN client_training_enrollment cte ON t.training_id = cte.training_id
        WHERE t.instructor_id = @InstructorId";
        int result = await connection.ExecuteScalarAsync<int>(sql, new { InstructorId = instructorId });
        return result.ToString();
    }


    public static async Task<string> CalculateNumTrainingsPerMonth(int month)
    {
        using MySqlConnection connection = new(MYSQL_CONNECTION_STRING);
        string sql = "SELECT COUNT(training.training_id) FROM training WHERE MONTH(training.date) = @Month";
        object? tmp = await connection.ExecuteScalarAsync(sql, new { Month = month });
        if (tmp is long result) return result.ToString();
        return "";
    }

    #endregion

    public static async Task<IEnumerable<dynamic>> ExecuteQuery(string query)
    {
        using MySqlConnection connection = new(MYSQL_CONNECTION_STRING);
        return await connection.QueryAsync(query);
    }

    public static async Task<ObservableCollection<ClientTrainingEnrollment>> GetAllEnrollments(int trainingID)
    {
        using MySqlConnection connection = new(MYSQL_CONNECTION_STRING);
        string sql = "SELECT e.*, CONCAT(c.first_name, \" \", c.last_name) AS clientName FROM client_training_enrollment AS e LEFT JOIN client AS c ON e.client_id = c.client_id WHERE e.training_id = @TrainingId";
        IEnumerable<ClientTrainingEnrollment> clientTrainingEnrollments = await connection.QueryAsync<ClientTrainingEnrollment>(sql, new { TrainingId = trainingID });
        ObservableCollection<ClientTrainingEnrollment> result = [.. clientTrainingEnrollments];
        return result;
    }

    public static async Task<bool> CheckIfEnrollmentContainsClientAsync(int trainingID, int clientId)
    {
        using MySqlConnection connection = new(MYSQL_CONNECTION_STRING);
        string sql = "SELECT COUNT(*) FROM client_training_enrollment WHERE training_id = @TrainingId AND client_id = @ClientId";
        long response = await connection.ExecuteScalarAsync<long>(sql, new { TrainingId = trainingID, ClientId = clientId });
        return response == 0;
    }

    public static async Task AddEnrollmentAsync(int trainingID, int clientId)
    {
        using MySqlConnection connection = new(MYSQL_CONNECTION_STRING);
        string sql = @"INSERT INTO client_training_enrollment (training_id, client_id)
                        VALUES (@TrainingId, @ClientId);";
        await connection.ExecuteAsync(sql, new { TrainingId = trainingID, ClientId = clientId });
    }

    public static async Task DeleteTrainingEnrollmentAsync(int trainingId, int clientId)
    {
        using MySqlConnection connection = new(MYSQL_CONNECTION_STRING);
        string sql = @"DELETE FROM client_training_enrollment WHERE training_id = @TrainingId AND client_id = @ClientId";
        await connection.ExecuteAsync(sql, new { TrainingId = trainingId, ClientId = clientId });
    }
}