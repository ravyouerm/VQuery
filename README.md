# VQuery

**A lightweight, high-performance database library for .NET** — one simple API for MySQL, PostgreSQL, and SQL Server.

[![NuGet](https://img.shields.io/nuget/v/VQuery.svg)](https://www.nuget.org/packages/VQuery)
[![Downloads](https://img.shields.io/nuget/dt/VQuery.svg)](https://www.nuget.org/packages/VQuery)
[![License: MIT](https://img.shields.io/badge/license-MIT-blue.svg)](https://licenses.nuget.org/MIT)
[![.NET](https://img.shields.io/badge/.NET-8.0-512BD4)](https://dotnet.microsoft.com/)

---

## Why VQuery?

Most .NET projects end up choosing between hand-written ADO.NET (verbose, repetitive) or a full ORM like EF Core (heavy, opinionated, slow to set up for simple apps). VQuery sits in between:

- 🔌 **One API, three databases** — switch between MySQL, PostgreSQL, and SQL Server without rewriting your data layer
- ⚡ **Fast** — reflection caching means object mapping doesn't slow you down as your app grows
- 🪶 **Lightweight** — no heavy configuration, no migrations engine, no fighting the framework
- 🧵 **Async-first** — every core operation has an async counterpart
- 🔒 **Safe by default** — parameterized queries throughout

If you want Dapper-like simplicity with built-in multi-database support out of the box, VQuery is built for that.

> **Status:** MySQL support is the most tested and battle-tested part of VQuery. PostgreSQL and SQL Server support exist and follow the same API, but have seen less real-world use so far — feedback and bug reports for those are especially welcome.

---

## Installation

```bash
dotnet add package VQuery
```

Or via the Package Manager Console:

```powershell
Install-Package VQuery
```

---

## Quick Start

**1. Add a connection string** to `appsettings.json`:

```json
{
  "ConnectionStrings": {
    "MYSQLConnection": {
      "server": "127.0.0.1",
      "port": "3306",
      "database": "sampledb",
      "username": "root",
      "password": "123456"
    }
  }
}
```

**2. Query in three lines:**

```csharp
using VQuery;

using var db = new MySQLConnection();
db.ConnectionOpen("MYSQLConnection");

var users = db.Query<User>("SELECT * FROM users");
```

Swap `MySQLConnection` for `PostgreSQLConnection` or `SQLServerConnection` and everything else stays the same.

---

## Core Features

| Feature | Description |
|---|---|
| `Query<T>()` / `QueryAsync<T>()` | Map query results directly to your model classes |
| `QueryFirst<T>()` / `QuerySingle<T>()` | Grab a single record with less boilerplate |
| `QueryFirstOrDefault<T>()` / `QuerySingleOrDefault<T>()` | Same as above, but return `default` instead of throwing when no rows are found |
| `QueryMultiple()` | Run multiple SELECT statements and get back a `DataSet` |
| `SelectDataTable()` / `SelectDataRow()` / `SelectDataValue()` | Lower-level access when you don't need object mapping |
| `ExecuteScalar<T>()` | Return a single value (counts, sums, etc.) |
| `ExecuteNonQuery()` | Run INSERT/UPDATE/DELETE and get back the affected row count |
| `Execute()` / `ExecuteRun()` | Run a statement and get back a `bool` instead of a row count |
| `Insert()` / `Update()` / `Delete()` | Simple CRUD helpers |
| `Exists()` / `ExistsAsync()` | Check whether a query returns any matching rows |
| `Begin()` / `Commit()` / `RollBack()` | Full transaction support |
| Connection pooling | Configurable min/max pool size out of the box |
| `VariableConverter` | Type-conversion utilities available directly on the connection object |

---

## Usage Examples

### CRUD

```csharp
// Insert
db.Insert("users", new Dictionary<string, string>
{
    { "name", "John" },
    { "email", "john@test.com" }
});

// Update
db.Update("users", new Dictionary<string, string>
{
    { "name", "David" }
}, "id=1");

// Delete (parameterized)
db.Delete("DELETE FROM users WHERE id=@id", new() { ["@id"] = 1 });
```

> **Note:** `Insert()` and `Update()` also have overloads that take `Dictionary<string, object>` instead of `Dictionary<string, string>`. Prefer the `object` overloads when your values include numbers, dates, or booleans — the `string` overloads pass every value through as `VARCHAR`.

### Check existence

```csharp
bool hasAdmins = db.Exists("SELECT 1 FROM users WHERE role=@role", new() { ["@role"] = "admin" });

bool hasAdminsAsync = await db.ExistsAsync("SELECT 1 FROM users WHERE role=@role", new() { ["@role"] = "admin" });
```

### Transactions

```csharp
db.Begin();
try
{
    db.Insert("users", new Dictionary<string, string> { { "name", "John" } });
    db.Update("users", new Dictionary<string, string> { { "name", "David" } }, "id=1");
    db.Commit();
}
catch
{
    db.RollBack();
}
```

### Async

```csharp
var users = await db.QueryAsync<User>("SELECT * FROM users");

var user = await db.QueryFirstAsync<User>(
    "SELECT * FROM users WHERE id=@id",
    new() { ["@id"] = 1 });
```

### Safe single-record lookups

```csharp
// Returns default(T) instead of throwing when no row is found
var user = db.QueryFirstOrDefault<User>(
    "SELECT * FROM users WHERE id=@id", new() { ["@id"] = 1 });

var userAsync = await db.QueryFirstOrDefaultAsync<User>(
    "SELECT * FROM users WHERE id=@id", new() { ["@id"] = 1 });
```

`QuerySingle<T>()` / `QuerySingleOrDefault<T>()` (and their async versions) work the same way, but throw if more than one row is returned.

### VariableConverter utilities

Available directly on the connection object for quick type conversions:

```csharp
db.ToInt("100");                     // -> 100
db.ToDouble("123.45");               // -> 123.45
db.ToIntString(100);                 // -> "100"
db.ToDoubleString(123.45);           // -> "123.45"
db.ToString(DateTime.Now);           // -> formatted string
db.ToStringDate(DateTime.Now);       // -> "dd-MM-yyyy" by default
db.ToStringDateTime(DateTime.Now);   // -> "dd-MM-yyyy HH:mm:ss" by default
db.ToDate("01-01-2026");             // -> DateTime
db.ToDateTime("01-01-2026 10:00:00");// -> DateTime
db.NumberToText(1000);               // -> number spelled out in words
db.NumberToTextKH(1000);             // -> number spelled out in Khmer
db.NumberToKhNumber(1000);           // -> number in Khmer numerals
db.IsEmpty(row);                     // -> true if a DataRow has no data
```

### Multiple result sets

```csharp
string sql = @"
    SELECT * FROM users;
    SELECT * FROM roles;
";

DataSet ds = db.QueryMultiple(sql);
DataTable users = ds.Tables[0];
DataTable roles = ds.Tables[1];
```

---

## Configuration Reference

Each connection block in `appsettings.json` supports:

| Key | Description | Default |
|---|---|---|
| `server` | Host address | — |
| `port` | Port number | DB-specific |
| `database` | Database name | — |
| `username` / `password` | Credentials | — |
| `pooling` | Enable connection pooling | `true` |
| `minimumPoolSize` / `maximumPoolSize` | Pool size range | `5` / `100` |
| `connectionTimeout` | Seconds before connection timeout | `30` |
| `commandTimeout` | Seconds before command timeout | `60` |
| `trustServerCertificate` | SQL Server only | `false` |

---

## Supported Frameworks

- .NET 8.0 (also compatible with .NET 9.0 / 10.0)

## Dependencies

- `Microsoft.Data.SqlClient` (>= 7.0.1)
- `Microsoft.Extensions.Configuration.Json` (>= 8.0.0)
- `MySql.Data` (>= 8.4.0)
- `Npgsql` (>= 8.0.3)

---

## Contributing

Issues and pull requests are welcome! If you find a bug or have a feature request, please [open an issue](https://github.com/ravyouerm/VQuery/issues).

## License

[MIT](https://licenses.nuget.org/MIT) — free to use in personal and commercial projects.

## Links

- 📦 [NuGet Package](https://www.nuget.org/packages/VQuery)
- 💻 [Source Code](https://github.com/ravyouerm/VQuery)