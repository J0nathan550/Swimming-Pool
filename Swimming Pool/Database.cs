using Dapper;
using MySqlConnector;
using Swimming_Pool.Models;
using System.Collections.ObjectModel;
using System.Text.RegularExpressions;

namespace Swimming_Pool;

public static class Database
{
    public static readonly string MYSQL_CONNECTION_STRING = "Server=127.0.0.1;Database=swimming_pool;Uid=root;Pwd=;";

    #region Subscription Queries

    public static async Task<ObservableCollection<Subscription>> GetSubscriptionsFiltered(string subscriptionType, string price, string startDate, string endDate, string clientName)
    {
        using MySqlConnection connection = new(MYSQL_CONNECTION_STRING);

        string sql = "SELECT * FROM subscription LEFT JOIN subscription_type ON subscription.subscription_type_id = subscription_type.subscription_type_id WHERE 1=1";
        string price2 = "";

        if (!string.IsNullOrEmpty(subscriptionType))
            sql += " AND subscription_type.name LIKE @SubscriptionType";
        if (!string.IsNullOrEmpty(price))
            sql += " AND " + GenerateNumbersComparison(ref price, ref price2, "price", "@Price", "@Price2");
        if (!string.IsNullOrEmpty(startDate))
            sql += " AND " + GenerateDatesComparison(ref startDate, "start_date");
        if (!string.IsNullOrEmpty(endDate))
            sql += " AND " + GenerateDatesComparison(ref endDate, "end_date");

        if (!string.IsNullOrEmpty(clientName))
        {
            sql += " AND client_id IN (SELECT client_id FROM client WHERE CONCAT(first_name, ' ', last_name) LIKE @ClientName)";
        }

        IEnumerable<Subscription> subscriptions = await connection.QueryAsync<Subscription>(sql, new
        {
            SubscriptionType = $"%{subscriptionType}%",
            Price = price,
            Price2 = price2,
            ClientName = $"%{clientName}%"
        });

        foreach (Subscription subscription in subscriptions)
        {
            await subscription.SetClientNameAsync();
            await subscription.SetSubscriptionTypeNameAsync();
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
            await subscription.SetSubscriptionTypeNameAsync();
        }
        return new ObservableCollection<Subscription>(subscriptions);
    }

    public static async Task<ObservableCollection<SubscriptionType>> GetAllSubscriptionTypes()
    {
        using MySqlConnection connection = new(MYSQL_CONNECTION_STRING);
        string sql = "SELECT * FROM subscription_type";
        IEnumerable<SubscriptionType> subscriptions = await connection.QueryAsync<SubscriptionType>(sql);
        return new ObservableCollection<SubscriptionType>(subscriptions);
    }

    public static async Task CreateSubscription(int subscriptionTypeId, float price, DateTime startDate, DateTime endDate, int clientId)
    {
        using MySqlConnection connection = new(MYSQL_CONNECTION_STRING);
        string sql = @"INSERT INTO subscription (subscription_type_id, price, start_date, end_date, client_id)
                   VALUES (@SubscriptionTypeId, @Price, @StartDate, @EndDate, @ClientId)";
        await connection.ExecuteAsync(sql, new
        {
            SubscriptionTypeId = subscriptionTypeId,
            Price = price,
            StartDate = startDate,
            EndDate = endDate,
            ClientId = clientId
        });
    }

    public static async Task CreateSubscriptionType(string name, string description)
    {
        using MySqlConnection connection = new(MYSQL_CONNECTION_STRING);
        string sql = @"INSERT INTO subscription_type (name, description)
                   VALUES (@Name, @Description)";
        await connection.ExecuteAsync(sql, new
        {
            Name = name,
            Description = description
        });
    }

    public static async Task UpdateSubscription(int subscriptionId, int subscriptionTypeId, float price, DateTime startDate, DateTime endDate, int clientId)
    {
        using MySqlConnection connection = new(MYSQL_CONNECTION_STRING);
        string sql = @"UPDATE subscription 
                   SET subscription_type_id = @SubscriptionTypeId, 
                       price = @Price, 
                       start_date = @StartDate, 
                       end_date = @EndDate, 
                       client_id = @ClientId 
                   WHERE subscription_id = @SubscriptionId";
        await connection.ExecuteAsync(sql, new
        {
            SubscriptionId = subscriptionId,
            SubscriptionTypeId = subscriptionTypeId,
            Price = price,
            StartDate = startDate,
            EndDate = endDate,
            ClientId = clientId
        });
    }

    public static async Task UpdateSubscriptionType(int subscriptionTypeId, string name, string description)
    {
        using MySqlConnection connection = new(MYSQL_CONNECTION_STRING);
        string sql = @"UPDATE subscription_type SET name = @Name, description = @Description WHERE subscription_type_id = @SubscriptionTypeId";
        await connection.ExecuteAsync(sql, new
        {
            SubscriptionTypeId = subscriptionTypeId,
            Name = name,
            Description = description
        });
    }

    public static async Task DeleteSubscriptionType(int subscriptionTypeId)
    {
        using MySqlConnection connection = new(MYSQL_CONNECTION_STRING);
        string sql = "DELETE FROM subscription_type WHERE subscription_type_id = @SubscriptionTypeId";
        await connection.ExecuteAsync(sql, new { SubscriptionTypeId = subscriptionTypeId });
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

    public static async Task<SubscriptionType?> GetSubscriptionTypeById(int subscriptionTypeId)
    {
        using MySqlConnection connection = new(MYSQL_CONNECTION_STRING);
        string sql = "SELECT * FROM subscription_type WHERE subscription_type_id = @SubscriptionTypeId;";
        SubscriptionType? subscriptionType = await connection.QueryFirstOrDefaultAsync<SubscriptionType>(sql, new { SubscriptionTypeId = subscriptionTypeId });
        return subscriptionType;
    }

    private static readonly Regex ComparisonNumbersRegExEQ = new(@"^\s*=\s*([\d\.,]+)\s*");
    private static readonly Regex ComparisonNumbersRegExNE = new(@"^\s*!=\s*([\d\.,]+)\s*");
    private static readonly Regex ComparisonNumbersRegExLT = new(@"^\s*<\s*([\d\.,]+)\s*");
    private static readonly Regex ComparisonNumbersRegExLE = new(@"^\s*<=\s*([\d\.,]+)\s*");
    private static readonly Regex ComparisonNumbersRegExGT = new(@"^\s*>\s*([\d\.,]+)\s*");
    private static readonly Regex ComparisonNumbersRegExGE = new(@"^\s*>=\s*([\d\.,]+)\s*");
    private static readonly Regex ComparisonNumbersRegExBW = new(@"^\s*([\d\.,]+)\s*\-\s*([\d\.,]+)\s*");
    private static readonly Regex ComparisonNumbersRegExNB = new(@"^\s*!\s*([\d\.,]+)\s*\-\s*([\d\.,]+)\s*");
    private static readonly Regex ComparisonDatesRegExFULL   = new(@"(\d\d\d\d)[/\-](\d{0,1}\d)[/\-](\d{0,1}\d)");
    private static readonly Regex ComparisonDatesRegExNoDAY  = new(@"(\d\d\d\d)[/\-](\d{0,1}\d)");
    private static readonly Regex ComparisonDatesRegExNoMon  = new(@"(\d\d\d\d)[/\-][/\-](\d{0,1}\d)");
    private static readonly Regex ComparisonDatesRegExNoYear = new(@"(\d{0,1}\d)[/\-](\d{0,1}\d)");
    private static readonly Regex ComparisonDatesRegExYear4   = new(@"y\s*(\d\d\d\d)", RegexOptions.IgnoreCase);
    private static readonly Regex ComparisonDatesRegExYear2   = new(@"y\s*(\d\d)", RegexOptions.IgnoreCase);
    private static readonly Regex ComparisonDatesRegExMonth   = new(@"m\s*(\d{0,1}\d)", RegexOptions.IgnoreCase);
    private static readonly Regex ComparisonDatesRegExDay     = new(@"d\s*(\d{0,1}\d)", RegexOptions.IgnoreCase);
    private static readonly Regex ComparisonDatesRegExEQ = new(@"^\s*=\s*");
    private static readonly Regex ComparisonDatesRegExNE = new(@"^\s*!=\s*");
    private static readonly Regex ComparisonDatesRegExLT = new(@"^\s*<\s*");
    private static readonly Regex ComparisonDatesRegExLE = new(@"^\s*<=\s*");
    private static readonly Regex ComparisonDatesRegExGT = new(@"^\s*>\s*");
    private static readonly Regex ComparisonDatesRegExGE = new(@"^\s*>=\s*");

    public static async Task<ObservableCollection<Client>> GetClientsFiltered(string firstName, string lastName, string age, string phoneNumber, string email)
    {
        bool first = true;
        string age2 = "";
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
                sql += " WHERE ";
            }
            else
            {
                sql += " AND ";
            }
            sql += GenerateNumbersComparison(ref age, ref age2, "age", "@Age1", "@Age2");
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
        IEnumerable<Client> clients = await connection.QueryAsync<Client>(sql, new { FirstName = firstName, LastName = lastName, Age1 = age, Age2 = age2, PhoneNumber = phoneNumber, EmailAddress = email });
        ObservableCollection<Client> result = [.. clients];
        return result;
    }

    private static string GenerateNumbersComparison(ref string val1, ref string val2, string sqlField, string sqlVar1, string sqlVar2)
    {
        string result;
        var match = ComparisonNumbersRegExEQ.Match(val1);
        if (match.Success)
        {
            result = $"{sqlField} = {sqlVar1}";
            val1 = match.Groups[1].Value;
        }
        else
        {
            match = ComparisonNumbersRegExNE.Match(val1);
            if (match.Success)
            {
                result = $"{sqlField} != {sqlVar1}";
                val1 = match.Groups[1].Value;
            }
            else
            {
                match = ComparisonNumbersRegExLT.Match(val1);
                if (match.Success)
                {
                    result = $"{sqlField} < {sqlVar1}";
                    val1 = match.Groups[1].Value;
                }
                else
                {
                    match = ComparisonNumbersRegExLE.Match(val1);
                    if (match.Success)
                    {
                        result = $"{sqlField} <= {sqlVar1}";
                        val1 = match.Groups[1].Value;
                    }
                    else
                    {
                        match = ComparisonNumbersRegExGT.Match(val1);
                        if (match.Success)
                        {
                            result = $"{sqlField} > {sqlVar1}";
                            val1 = match.Groups[1].Value;
                        }
                        else
                        {
                            match = ComparisonNumbersRegExGE.Match(val1);
                            if (match.Success)
                            {
                                result = $"{sqlField} >= {sqlVar1}";
                                val1 = match.Groups[1].Value;
                            }
                            else
                            {
                                match = ComparisonNumbersRegExBW.Match(val1);
                                if (match.Success)
                                {
                                    result = $"({sqlField} >= {sqlVar1} AND {sqlField} <= {sqlVar2})";
                                    val1 = match.Groups[1].Value;
                                    val2 = match.Groups[2].Value;
                                }
                                else
                                {
                                    match = ComparisonNumbersRegExNB.Match(val1);
                                    if (match.Success)
                                    {
                                        result = $"({sqlField} < {sqlVar1} OR {sqlField} > {sqlVar2})";
                                        val1 = match.Groups[1].Value;
                                        val2 = match.Groups[2].Value;
                                    }
                                    else
                                    {
                                        result = $"{sqlField} = {sqlVar1}";
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }
        return result;
    }
    
    public enum Operand { EQ, NE, LT, LE, GT, GE };

    private static string GenerateDatesComparison(ref string val1, string sqlField)
    {
        string result = $"{sqlField} = {val1}";
        Operand operand = Operand.EQ;

        if (ComparisonDatesRegExEQ.Match(val1).Success) operand = Operand.EQ;
        if (ComparisonDatesRegExNE.Match(val1).Success) operand = Operand.NE;
        if (ComparisonDatesRegExLT.Match(val1).Success) operand = Operand.LT;
        if (ComparisonDatesRegExLE.Match(val1).Success) operand = Operand.LE;
        if (ComparisonDatesRegExGT.Match(val1).Success) operand = Operand.GT;
        if (ComparisonDatesRegExGE.Match(val1).Success) operand = Operand.GE;

        Match m = ComparisonDatesRegExFULL.Match(val1);
        if (m.Success)
        {
            switch (operand)
            {
                case Operand.EQ: return $"YEAR({sqlField}) =  {m.Groups[1].Value} AND MONTH({sqlField}) =  {m.Groups[2].Value} AND DAY({sqlField}) =  {m.Groups[3].Value}";
                case Operand.NE: return $"YEAR({sqlField}) != {m.Groups[1].Value} AND MONTH({sqlField}) != {m.Groups[2].Value} AND DAY({sqlField}) != {m.Groups[3].Value}";
                case Operand.LT: return $"{sqlField} <  '{m.Groups[1].Value}/{m.Groups[2].Value}/{m.Groups[3].Value}'";
                case Operand.LE: return $"{sqlField} <= '{m.Groups[1].Value}/{m.Groups[2].Value}/{m.Groups[3].Value}'";
                case Operand.GT: return $"{sqlField} >  '{m.Groups[1].Value}/{m.Groups[2].Value}/{m.Groups[3].Value}'";
                case Operand.GE: return $"{sqlField} >= '{m.Groups[1].Value}/{m.Groups[2].Value}/{m.Groups[3].Value}'";
            }
        }
        m = ComparisonDatesRegExNoDAY.Match(val1);
        if (m.Success)
        {
            switch (operand)
            {
                case Operand.EQ: return $"YEAR({sqlField}) = {m.Groups[1].Value} AND MONTH({sqlField}) = {m.Groups[2].Value}";
                case Operand.NE: return $"YEAR({sqlField}) != {m.Groups[1].Value} AND MONTH({sqlField}) != {m.Groups[2].Value}";
                case Operand.LT: return $"{sqlField} <  '{m.Groups[1].Value}/{m.Groups[2].Value}/01'";
                case Operand.LE: return $"{sqlField} <= '{m.Groups[1].Value}/{m.Groups[2].Value}/01'";
                case Operand.GT: return $"{sqlField} >  '{m.Groups[1].Value}/{m.Groups[2].Value}/01'";
                case Operand.GE: return $"{sqlField} >= '{m.Groups[1].Value}/{m.Groups[2].Value}/01'";
            }
        }
        m = ComparisonDatesRegExNoMon.Match(val1);
        if (m.Success)
        {
            switch (operand)
            {
                case Operand.EQ: return $"YEAR({sqlField}) = {m.Groups[1].Value} AND DAY({sqlField}) = {m.Groups[2].Value}";
                case Operand.NE: return $"YEAR({sqlField}) != {m.Groups[1].Value} AND DAY{sqlField}) != {m.Groups[2].Value}";
                case Operand.LT: return $"YEAR({sqlField}) < {m.Groups[1].Value} AND DAY({sqlField}) < {m.Groups[2].Value}";
                case Operand.LE: return $"YEAR({sqlField}) <= {m.Groups[1].Value} AND DAY{sqlField}) <= {m.Groups[2].Value}";
                case Operand.GT: return $"YEAR({sqlField}) > {m.Groups[1].Value} AND DAY({sqlField}) > {m.Groups[2].Value}";
                case Operand.GE: return $"YEAR({sqlField}) >= {m.Groups[1].Value} AND DAY{sqlField}) >= {m.Groups[2].Value}";
            }
        }
        m = ComparisonDatesRegExNoYear.Match(val1);
        if (m.Success)
        {
            switch (operand)
            {
                case Operand.EQ: return $"MONTH({sqlField}) = {m.Groups[1].Value} AND DAY({sqlField}) = {m.Groups[2].Value}";
                case Operand.NE: return $"MONTH({sqlField}) != {m.Groups[1].Value} AND DAY{sqlField}) != {m.Groups[2].Value}";
                case Operand.LT: return $"MONTH({sqlField}) < {m.Groups[1].Value} AND DAY({sqlField}) < {m.Groups[2].Value}";
                case Operand.LE: return $"MONTH({sqlField}) <= {m.Groups[1].Value} AND DAY{sqlField}) <= {m.Groups[2].Value}";
                case Operand.GT: return $"MONTH({sqlField}) > {m.Groups[1].Value} AND DAY({sqlField}) > {m.Groups[2].Value}";
                case Operand.GE: return $"MONTH({sqlField}) >= {m.Groups[1].Value} AND DAY{sqlField}) >= {m.Groups[2].Value}";
            }
        }
        m = ComparisonDatesRegExYear4.Match(val1);
        if (m.Success)
        {
            switch (operand)
            {
                case Operand.EQ: return $"YEAR({sqlField}) = {m.Groups[1].Value}";
                case Operand.NE: return $"YEAR({sqlField}) != {m.Groups[1].Value}";
                case Operand.LT: return $"YEAR({sqlField}) < {m.Groups[1].Value}";
                case Operand.LE: return $"YEAR({sqlField}) <= {m.Groups[1].Value}";
                case Operand.GT: return $"YEAR({sqlField}) > {m.Groups[1].Value}";
                case Operand.GE: return $"YEAR({sqlField}) >= {m.Groups[1].Value}";
            }
        }
        m = ComparisonDatesRegExYear2.Match(val1);
        if (m.Success)
        {
            switch (operand)
            {
                case Operand.EQ: return $"YEAR({sqlField}) = {"20" + m.Groups[1].Value}";
                case Operand.NE: return $"YEAR({sqlField}) != {"20" + m.Groups[1].Value}";
                case Operand.LT: return $"YEAR({sqlField}) < {"20" + m.Groups[1].Value}";
                case Operand.LE: return $"YEAR({sqlField}) <= {"20" + m.Groups[1].Value}";
                case Operand.GT: return $"YEAR({sqlField}) > {"20" + m.Groups[1].Value}";
                case Operand.GE: return $"YEAR({sqlField}) >= {"20" + m.Groups[1].Value}";
            }
        }
        m = ComparisonDatesRegExMonth.Match(val1);
        if (m.Success)
        {
            switch (operand)
            {
                case Operand.EQ: return $"MONTH({sqlField}) = {m.Groups[1].Value}";
                case Operand.NE: return $"MONTH({sqlField}) != {m.Groups[1].Value}";
                case Operand.LT: return $"MONTH({sqlField}) < {m.Groups[1].Value}";
                case Operand.LE: return $"MONTH({sqlField}) <= {m.Groups[1].Value}";
                case Operand.GT: return $"MONTH({sqlField}) > {m.Groups[1].Value}";
                case Operand.GE: return $"MONTH({sqlField}) >= {m.Groups[1].Value}";
            }
        }
        m = ComparisonDatesRegExDay.Match(val1);
        if (m.Success)
        {
            switch (operand)
            {
                case Operand.EQ: return $"DAY({sqlField}) = {m.Groups[1].Value}";
                case Operand.NE: return $"DAY({sqlField}) != {m.Groups[1].Value}";
                case Operand.LT: return $"DAY({sqlField}) < {m.Groups[1].Value}";
                case Operand.LE: return $"DAY({sqlField}) <= {m.Groups[1].Value}";
                case Operand.GT: return $"DAY({sqlField}) > {m.Groups[1].Value}";
                case Operand.GE: return $"DAY({sqlField}) >= {m.Groups[1].Value}";
            }
        }


        return result;
    }

    public static async Task<ObservableCollection<Client>> GetClientsFilteredByName(string firstNameOrLastName)
    {
        using MySqlConnection connection = new(MYSQL_CONNECTION_STRING);
        string sql = "SELECT * FROM client";
        if (!string.IsNullOrEmpty(firstNameOrLastName))
        {
            sql += " WHERE first_name LIKE @FirstNameOrLastName OR last_name LIKE @FirstNameOrLastName";
            firstNameOrLastName = "%" + firstNameOrLastName + "%";
        }
        IEnumerable<Client> clients = await connection.QueryAsync<Client>(sql, new { FirstNameOrLastName = firstNameOrLastName });
        ObservableCollection<Client> result = [.. clients];
        return result;
    }

    public static async Task<ObservableCollection<Instructor>> GetInstructorsFilteredByName(string firstNameOrLastName)
    {
        using MySqlConnection connection = new(MYSQL_CONNECTION_STRING);
        string sql = "SELECT * FROM instructor";
        if (!string.IsNullOrEmpty(firstNameOrLastName))
        {
            sql += " WHERE first_name LIKE @FirstNameOrLastName OR last_name LIKE @FirstNameOrLastName";
            firstNameOrLastName = "%" + firstNameOrLastName + "%";
        }
        IEnumerable<Instructor> instructors = await connection.QueryAsync<Instructor>(sql, new { FirstNameOrLastName = firstNameOrLastName });
        ObservableCollection<Instructor> result = [.. instructors];
        return result;
    }

    public static async Task<ObservableCollection<Pool>> GetPoolsFilteredByName(string name)
    {
        using MySqlConnection connection = new(MYSQL_CONNECTION_STRING);
        string sql = "SELECT * FROM pool";
        if (!string.IsNullOrEmpty(name))
        {
            sql += " WHERE name LIKE @Name";
            name = "%" + name + "%";
        }
        IEnumerable<Pool> pools = await connection.QueryAsync<Pool>(sql, new { Name = name });
        ObservableCollection<Pool> result = [.. pools];
        return result;
    }

    public static async Task<ObservableCollection<SubscriptionType>> GetSubscriptionTypeFilteredByName(string name)
    {
        using MySqlConnection connection = new(MYSQL_CONNECTION_STRING);
        string sql = "SELECT * FROM subscription_type";
        if (!string.IsNullOrEmpty(name))
        {
            sql += " WHERE name LIKE @Name";
            name = "%" + name + "%";
        }
        IEnumerable<SubscriptionType> subscriptionTypes = await connection.QueryAsync<SubscriptionType>(sql, new { Name = name });
        ObservableCollection<SubscriptionType> result = [.. subscriptionTypes];
        return result;
    }

    public static async Task<ObservableCollection<SpecializationType>> GetSpecializationFilteredByName(string name)
    {
        using MySqlConnection connection = new(MYSQL_CONNECTION_STRING);
        string sql = "SELECT * FROM instructor_specialization";
        if (!string.IsNullOrEmpty(name))
        {
            sql += " WHERE specialization LIKE @Name";
            name = "%" + name + "%";
        }
        IEnumerable<SpecializationType> specializationTypes = await connection.QueryAsync<SpecializationType>(sql, new { Name = name });
        ObservableCollection<SpecializationType> result = [.. specializationTypes];
        return result;
    }

    public static async Task<string> GetClientNameByIdAsync(int clientId)
    {
        using MySqlConnection connection = new(MYSQL_CONNECTION_STRING);
        string sql = "SELECT CONCAT(first_name, ' ', last_name) FROM client WHERE client_id = @ClientId;";
        string? clientName = await connection.ExecuteScalarAsync<string>(sql, new { ClientId = clientId });
        return clientName ?? string.Empty;
    }

    public static async Task<string> GetSubscriptionTypeNameByIdAsync(int subscriptionTypeId)
    {
        using MySqlConnection connection = new(MYSQL_CONNECTION_STRING);
        string sql = "SELECT name FROM subscription_type WHERE subscription_type_id = @SubscriptionTypeId;";
        string? name = await connection.ExecuteScalarAsync<string>(sql, new { SubscriptionTypeId = subscriptionTypeId });
        return name ?? string.Empty;
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

    public static async Task<ObservableCollection<Training>> GetTrainingFiltered(string date, string trainingType, string clientNames, string poolName, string instructorName)
    {
        using MySqlConnection connection = new(MYSQL_CONNECTION_STRING);

        string sql = @"
        SELECT 
            training.*, 
            instructor.first_name AS instructorName
        FROM 
            training 
        LEFT JOIN 
            instructor ON training.instructor_id = instructor.instructor_id
        LEFT JOIN 
            pool ON training.pool_id = pool.pool_id";

        List<string> conditions = [];
        if (!string.IsNullOrWhiteSpace(date)) conditions.Add(
            GenerateDatesComparison(ref date, "training.date")
            );
        if (!string.IsNullOrWhiteSpace(trainingType)) conditions.Add("training.training_type LIKE @TrainingType");
        if (!string.IsNullOrWhiteSpace(poolName)) conditions.Add("pool.name LIKE @PoolName");
        if (!string.IsNullOrWhiteSpace(instructorName)) conditions.Add("(instructor.first_name LIKE @InstructorName OR instructor.last_name LIKE @InstructorName)");

        if (conditions.Count > 0)
        {
            sql += " WHERE " + string.Join(" AND ", conditions);
        }

        sql += " GROUP BY training.training_id ORDER BY training.training_id;";

        IEnumerable<Training> trainings = await connection.QueryAsync<Training>(sql, new
        {
            TrainingType = $"%{trainingType}%",
            InstructorName = $"%{instructorName}%",
            PoolName = $"%{poolName}%"
        });

        List<Training> trainingResult = [];

        foreach (Training training in trainings)
        {
            sql = "SELECT CONCAT(c.first_name, ' ', c.last_name) AS clientName FROM client as c LEFT JOIN client_training_enrollment AS e ON c.client_id = e.client_id WHERE e.training_id = @TrainingId";
            var clientNamesResult = await connection.QueryAsync<string>(sql, new { training.TrainingId });
            string clientNamesConcatenated = string.Join(", ", clientNamesResult);
            bool clientNameMatch = string.IsNullOrEmpty(clientNames) || clientNamesConcatenated.Contains(clientNames, StringComparison.OrdinalIgnoreCase);
            if (clientNameMatch)
            {
                training.ClientNames = clientNamesConcatenated;

                sql = "SELECT name FROM pool WHERE pool_id = @PoolId";
                string? pool = await connection.QueryFirstAsync<string>(sql, new { training.PoolId });
                training.PoolName = pool;

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
        foreach (Instructor instructor in instructors)
        {
            await instructor.SetSpecializationTypeNameAsync();
        }
        ObservableCollection<Instructor> result = [.. instructors];
        return result;
    }

    public static async Task CreateInstructor(string firstName, string lastName, int age, string phoneNumber, string emailAddress, int specialization_id)
    {
        using MySqlConnection connection = new(MYSQL_CONNECTION_STRING);
        string sql = @"INSERT INTO instructor (first_name, last_name, age, phone_number, email_address, instructor_specialization_id)
                        VALUES (@FirstName, @LastName, @Age, @PhoneNumber, @EmailAddress, @SpecializationId);";
        await connection.ExecuteAsync(sql, new { FirstName = firstName, LastName = lastName, Age = age, PhoneNumber = phoneNumber, EmailAddress = emailAddress, SpecializationId = specialization_id });
    }

    public static async Task DeleteInstructor(int instructorId)
    {
        using MySqlConnection connection = new(MYSQL_CONNECTION_STRING);
        string sql = "DELETE FROM instructor WHERE instructor_id = @InstructorId;";
        await connection.ExecuteAsync(sql, new { InstructorId = instructorId });
    }

    public static async Task UpdateInstructor(int instructorId, string firstName, string lastName, int age, string phoneNumber, string emailAddress, int specialization_id)
    {
        using MySqlConnection connection = new(MYSQL_CONNECTION_STRING);
        string sql = @"UPDATE instructor 
                           SET first_name = @FirstName, last_name = @LastName, age = @Age, 
                               phone_number = @PhoneNumber, email_address = @EmailAddress, instructor_specialization_id = @SpecializationId
                           WHERE instructor_id = @InstructorId;";
        await connection.ExecuteAsync(sql, new { InstructorId = instructorId, FirstName = firstName, LastName = lastName, Age = age, PhoneNumber = phoneNumber, EmailAddress = emailAddress, SpecializationId = specialization_id });
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
        string age2 = "";
        using MySqlConnection connection = new(MYSQL_CONNECTION_STRING);
        string sql = "SELECT * FROM instructor LEFT JOIN instructor_specialization ON instructor.instructor_specialization_id = instructor_specialization.instructor_specialization_id";
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
                sql += " WHERE ";
            }
            else
            {
                sql += " AND ";
            }
            sql += GenerateNumbersComparison(ref age, ref age2, "age", "@Age", "@Age2");
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
                sql += " WHERE instructor_specialization.specialization LIKE @Specialization";
            }
            else
            {
                sql += " AND instructor_specialization.specialization LIKE @Specialization";
            }
            specialization = "%" + specialization + "%";
            first = false;
        }
        IEnumerable<Instructor> instructors = await connection.QueryAsync<Instructor>(sql, new { FirstName = firstName, LastName = lastName, Age = age, Age2 = age2, PhoneNumber = phoneNumber, EmailAddress = email, Specialization = specialization });
        foreach (Instructor instructor in instructors)
        {
            await instructor.SetSpecializationTypeNameAsync();
        }
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

        string laneCount2 = "";
        string poolLength2 = "";
        string poolDepth2 = "";

        if (!string.IsNullOrEmpty(poolName))
            sql += " AND name LIKE @PoolName";
        if (!string.IsNullOrEmpty(laneCount))
            sql += " AND " + GenerateNumbersComparison(ref laneCount, ref laneCount2, "lane_count", "@LaneCount", "@LaneCount2");
        if (!string.IsNullOrEmpty(poolLength))
            sql += " AND " + GenerateNumbersComparison(ref poolLength, ref poolLength2, "length", "@PoolLength", "@PoolLength2");
        if (!string.IsNullOrEmpty(poolDepth))
            sql += " AND " + GenerateNumbersComparison(ref poolDepth, ref poolDepth2, "depth", "@PoolDepth", "@PoolDepth2");
        if (!string.IsNullOrEmpty(poolAddress))
            sql += " AND address LIKE @PoolAddress";

        IEnumerable<Pool> pools = await connection.QueryAsync<Pool>(sql, new
        {
            PoolName = $"%{poolName}%",
            LaneCount = laneCount,
            PoolLength = poolLength,
            PoolDepth = poolDepth,
            LaneCount2 = laneCount2,
            PoolLength2 = poolLength2,
            PoolDepth2 = poolDepth2,
            PoolAddress = $"%{poolAddress}%"
        });

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

    public static async Task<List<InstructorEngagement>> GetInstructorEngagement()
    {
        using MySqlConnection connection = new(MYSQL_CONNECTION_STRING);
        string sql = "SELECT CONCAT(first_name, ' ', last_name) AS name, COUNT(training.training_id) AS count FROM" +
            " instructor RIGHT JOIN training ON training.instructor_id = instructor.instructor_id GROUP BY instructor.instructor_id";
        IEnumerable<InstructorEngagement>? tmp = await connection.QueryAsync<InstructorEngagement>(sql);
        List<InstructorEngagement> result = [.. tmp];
        return result;
    }

    public static async Task<List<InstructorEngagement>> GetInstructorClients()
    {
        using MySqlConnection connection = new(MYSQL_CONNECTION_STRING);
        string sql = "select concat(i.first_name, '\n' , i.last_name) as Name, Count(c.client_id) AS count from Instructor as i right join training as t on t.instructor_id = i.instructor_id right join client_training_enrollment as e ON\r\ne.training_id = t.training_id left join client as c on e.client_id = c.client_id group by i.instructor_id";
        IEnumerable<InstructorEngagement>? tmp = await connection.QueryAsync<InstructorEngagement>(sql);
        List<InstructorEngagement> result = [.. tmp];
        return result;
    }

    public static async Task<List<TrainingStatistics>> GetTrainingsStatistics()
    {
        using MySqlConnection connection = new(MYSQL_CONNECTION_STRING);
        string sql = "select t.date as Date, sum(s.price) as Price, Count(c.client_id) as Clients from training as t right join client_training_enrollment as e on e.training_id = t.training_id left join client as c on c.client_id = e.client_id right join subscription as s on c.client_id = s.client_id GROUP by t.training_id";
        IEnumerable<TrainingStatistics>? tmp = await connection.QueryAsync<TrainingStatistics>(sql);
        List<TrainingStatistics> result = [.. tmp];
        return result;
    }

    public static async Task<List<SubscriptionStatistics>> GetSubscriptionStatistics()
    {
        using MySqlConnection connection = new(MYSQL_CONNECTION_STRING);
        string sql = "select st.name As SubscriptionType, Avg(s.price) as Price, count(c.client_id) as Clients from subscription_type as st right join subscription as s on s.subscription_type_id=st.subscription_type_id left join client as c on s.client_id = c.client_id group by st.subscription_type_id";
        IEnumerable<SubscriptionStatistics>? tmp = await connection.QueryAsync<SubscriptionStatistics>(sql);
        List<SubscriptionStatistics> result = [.. tmp];
        return result;
    }    
    public static async Task<List<SpecializationStatistics>> GetAverageAgePerSpecializationStatistics()
    {
        using MySqlConnection connection = new(MYSQL_CONNECTION_STRING);
        string sql = "select Avg(c.age) as Age, s.specialization as Specialization, Count(c.client_id) as Clients from client as c right join client_training_enrollment as e on e.client_id = c.client_id left join training as t on t.training_id = e.training_id left join instructor as i on t.instructor_id = i.instructor_id left join instructor_specialization as s on i.instructor_specialization_id = s.instructor_specialization_id group by s.instructor_specialization_id;\r\n";
        IEnumerable<SpecializationStatistics>? tmp = await connection.QueryAsync<SpecializationStatistics>(sql);
        List<SpecializationStatistics> result = [.. tmp];
        return result;
    }

    public class SubscriptionStatistics
    {
        public string? SubscriptionType { get; set; }
        public float Price { get; set; }
        public int Clients { get; set; }
    }    
    public class SpecializationStatistics
    {
        public string? Specialization { get; set; }
        public float Age { get; set; }
        public int Clients { get; set; }
    }
    public class TrainingStatistics
    {
        public DateTime Date { get; set; }
        public float Price { get; set; }
        public int Clients { get; set; }
    }
    public class InstructorEngagement
    {
        public string? Name { get; set; }
        public int Count { get; set; }
    }

    #endregion

    #region Enrollments
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

    #endregion

    #region SpecializationType

    public static async Task CreateSpecializationType(string specialization)
    {
        using MySqlConnection connection = new(MYSQL_CONNECTION_STRING);
        string sql = @"INSERT INTO instructor_specialization (specialization) VALUES (@Specialization)";
        await connection.ExecuteAsync(sql, new { Specialization = specialization });
    }

    public static async Task<ObservableCollection<SpecializationType>> GetAllSpecializationTypes()
    {
        using MySqlConnection connection = new(MYSQL_CONNECTION_STRING);
        string sql = @"SELECT * FROM instructor_specialization";
        IEnumerable<SpecializationType> result = await connection.QueryAsync<SpecializationType>(sql);
        return new ObservableCollection<SpecializationType>(result);
    }

    public static async Task<SpecializationType?> GetSpecializationTypeById(int specializationTypeId)
    {
        using MySqlConnection connection = new(MYSQL_CONNECTION_STRING);
        string sql = @"SELECT * FROM instructor_specialization WHERE instructor_specialization_id = @SpecializationTypeId";
        return await connection.QueryFirstOrDefaultAsync<SpecializationType>(sql, new { SpecializationTypeId = specializationTypeId });
    }

    public static async Task UpdateSpecializationType(int specializationTypeId, string specialization)
    {
        using MySqlConnection connection = new(MYSQL_CONNECTION_STRING);
        string sql = @"UPDATE instructor_specialization SET specialization = @Specialization WHERE instructor_specialization_id = @SpecializationTypeId";
        await connection.ExecuteAsync(sql, new { SpecializationTypeId = specializationTypeId, Specialization = specialization });
    }

    public static async Task DeleteSpecializationType(int specializationTypeId)
    {
        using MySqlConnection connection = new(MYSQL_CONNECTION_STRING);
        string sql = @"DELETE FROM instructor_specialization WHERE instructor_specialization_id = @SpecializationTypeId";
        await connection.ExecuteAsync(sql, new { SpecializationTypeId = specializationTypeId });
    }

    public static async Task<string> GetSpecializationTypeNameByIdAsync(int specializationId)
    {
        using MySqlConnection connection = new(MYSQL_CONNECTION_STRING);
        string sql = "SELECT specialization FROM instructor_specialization WHERE instructor_specialization_id = @SpecializationId";
        string? specialization = await connection.ExecuteScalarAsync<string>(sql, new { SpecializationId = specializationId });
        return specialization ?? string.Empty;
    }

    #endregion
}

