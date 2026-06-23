# VQuery

[![NuGet](https://img.shields.io/nuget/v/VQuery.svg)](https://www.nuget.org/packages/VQuery)

Lightweight ORM and Database Helper for .NET

VQuery is a lightweight, high-performance database library designed for .NET applications. It provides a simple API for working with MySQL, PostgreSQL, and SQL Server while maintaining excellent performance through ReflectionCache optimization and lightweight ORM mapping.

## Supported Frameworks

* .NET 8.0

## Features

✅ MySQL Support

✅ PostgreSQL Support

✅ SQL Server Support

✅ Generic ORM Mapping

✅ Async Support

✅ Transactions

✅ Parameterized Queries

✅ ExecuteScalar<T>()

✅ ExecuteNonQuery()

✅ Query<T>()

✅ QueryFirst<T>()

✅ QuerySingle<T>()

✅ QueryMultiple()

✅ Connection Pooling

✅ ReflectionCache Optimization

✅ VariableConverter Utilities

---

## Installation

### Package Manager

```powershell
Install-Package VQuery
```

### .NET CLI

```bash
dotnet add package VQuery
```

### Specific Version

```bash
dotnet add package VQuery --version 1.0.5
```

---

## Configuration

### appsettings.json

```json
{
  "ConnectionStrings": {

    "MSQLConnection": {
      "server": "127.0.0.1",
      "port": "3306",
      "database": "sampledb",
      "username": "root",
      "password": "123456",
      "pooling": "true",
      "minimumPoolSize": "5",
      "maximumPoolSize": "100",
      "connectionTimeout": "30",
      "commandTimeout": "60"
    },

    "POSTGREConnection": {
      "server": "127.0.0.1",
      "port": "5432",
      "database": "sampledb",
      "username": "postgres",
      "password": "123456",
      "pooling": "true",
      "minimumPoolSize": "5",
      "maximumPoolSize": "100",
      "connectionTimeout": "30",
      "commandTimeout": "60"
    },

    "SQLSERVERConnection": {
      "server": "127.0.0.1",
      "port": "1433",
      "database": "sampledb",
      "username": "sa",
      "password": "123456",
      "pooling": "true",
      "minimumPoolSize": "5",
      "maximumPoolSize": "100",
      "connectionTimeout": "30",
      "commandTimeout": "60",
      "trustServerCertificate": "true"
    }
  }
}
```

---

## Quick Start

### MySQL

```csharp
using VQuery;

using var db = new MySQLConnection();

db.ConnectionOpen("MYSQLConnection");

var users =
    db.Query<User>(
        "SELECT * FROM users");
```

### PostgreSQL

```csharp
using VQuery;

using var db = new PostgreSQLConnection();

db.ConnectionOpen("POSTGREConnection");

var users =
    db.Query<User>(
        "SELECT * FROM users");
```

### SQL Server

```csharp
using VQuery;

using var db = new SQLServerConnection();

db.ConnectionOpen("SQLSERVERConnection");

var users =
    db.Query<User>(
        "SELECT * FROM users");
```

---

## Transactions

```csharp
db.Begin();

try
{
    db.Insert(
        "users",
        new Dictionary<string,string>
        {
            { "name", "John" }
        });

    db.Update(
        "users",
        new Dictionary<string,string>
        {
            { "name", "David" }
        },
        "id=1");

    db.Commit();
}
catch
{
    db.RollBack();
}
```

---

## CRUD Operations

### SelectDataTable

```csharp
DataTable dt =
    db.SelectDataTable(
        "SELECT * FROM users");
```

### SelectDataRow

```csharp
DataRow row =
    db.SelectDataRow(
        "SELECT * FROM users WHERE id=1");
```

### SelectDataValue

```csharp
string value =
    db.SelectDataValue(
        "SELECT name FROM users WHERE id=1");
```

### SelectDataScalar

```csharp
int total =
    db.ExecuteScalar<int>(
        "SELECT COUNT(*) FROM users");
```

### Insert

```csharp
db.Insert(
    "users",
    new Dictionary<string,string>
    {
        { "name", "John" },
        { "email", "john@test.com" }
    });
```

### Update

```csharp
db.Update(
    "users",
    new Dictionary<string,string>
    {
        { "name", "David" }
    },
    "id=1");
```

### Delete

```csharp
db.Delete(
    "DELETE FROM users WHERE id=@id",
    new()
    {
        ["@id"] = 1
    });
```

---

## ORM Mapping

```csharp
public class User
{
    public int Id { get; set; }

    public string Name { get; set; }

    public string Email { get; set; }
}
```

```csharp
var users =
    db.Query<User>(
        "SELECT * FROM users");
```

---

## Async Support

```csharp
var users =
    await db.QueryAsync<User>(
        "SELECT * FROM users");
```

```csharp
var user =
    await db.QueryFirstAsync<User>(
        "SELECT * FROM users WHERE id=@id",
        new()
        {
            ["@id"] = 1
        });
```

```csharp
int total =
    await db.ExecuteScalarAsync<int>(
        "SELECT COUNT(*) FROM users");
```

---

## QueryMultiple

```csharp
string sql =
@"
SELECT * FROM users;
SELECT * FROM roles;
";

DataSet ds =
    db.QueryMultiple(sql);

DataTable users = ds.Tables[0];
DataTable roles = ds.Tables[1];
```

---

## VariableConverter

```csharp
db.ToInt("100");

db.ToDouble("123.45");

db.ToString(DateTime.Now);

db.ToDate("01-01-2026");

db.NumberToText(1000);

db.NumberToTextKH(1000);
```

---

## Performance

VQuery automatically uses ReflectionCache for:

* Query<T>()
* QueryAsync<T>()
* QueryFirst<T>()
* QuerySingle<T>()

Benefits:

* Faster object mapping
* Reduced reflection overhead
* Lower CPU usage
* Better scalability

---

## Links

GitHub

https://github.com/ravyouerm/VQuery

NuGet

https://www.nuget.org/packages/VQuery

---

## Version 1.0.5

### Added

* PostgreSQL Support
* SQL Server Support
* Async Methods
* QueryMultiple
* ReflectionCache
* Connection Pooling
* ExecuteScalar<T>()
* ExecuteNonQuery()

### Improved

* MySQL Performance
* PostgreSQL Performance
* SQL Server Performance
* Documentation

### Fixed

* Dictionary<string,string> compatibility overloads
* MySQL connection string handling
* ORM mapping improvements

---

## License

MIT License
