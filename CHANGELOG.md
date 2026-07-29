# Changelog

All notable changes to VQuery are documented here.

## [1.0.6]

### Fixed
- **`ConnectionOpen()` default connection string key** — was `"MSQLConnection"` (typo), now correctly `"MYSQLConnection"`. Anyone relying on the default (no argument) was silently failing to connect.
- **`dbx()` default connection string key** — corrected to match the actual `appsettings.json` key name.
- **`DataUpdateMysql` connection side effect** — two internal overloads used to open the connection if it was closed and then close it again afterward. This could unexpectedly close a connection a caller was still using elsewhere. These now require an already-open connection, consistent with every other method in the class.
- **`Exists()` / `ExistsAsync()` nullable-cast crash** — `ExecuteScalar<int?>()` threw `Invalid cast from 'Int64' to 'Nullable<Int32>'` because `Convert.ChangeType` doesn't support nullable target types directly. Also affected any code that checked existence during a rollback flow.
- **`ToInt()` / `ToIntString()`** — threw and silently returned `0` for decimal-looking strings (e.g. `"88.90"`), since `Int32.Parse` rejects decimal points. Now parses as decimal first and truncates.
- **`ToDate()` / `ToDateTime()`** — returned today's date instead of the parsed value whenever the input didn't exactly match the given format string (e.g. ISO `yyyy-MM-dd` against the default `dd-MM-yyyy`). Now tries a short list of common fallback formats before giving up.

### Added
- **`[Column("...")]` attribute** — map a model property to a differently-named database column. Falls back to the property name (case-insensitive) when absent, matching prior behavior.
- **`IVQueryConnection` interface** — a shared contract implemented by `MySQLConnection`, `PostgreSQLConnection`, and `SQLServerConnection`, for writing database-agnostic code and registering VQuery with dependency injection.
- **`InsertMultiple<T>()`** (in `VQuery.Extensions`) — bulk-insert a `List<T>` in a single statement. Now validates table/column identifiers consistently with the rest of the library, and respects `[Column]` mappings.
- XML documentation comments on the full public API of `MySQLConnection`, `PostgreSQLConnection`, and `SQLServerConnection`, for IntelliSense support in Visual Studio.

### Notes
- PostgreSQL and SQL Server implementations already had the nullable-cast fix and correct connection-key defaults prior to this release — the fixes above were specific to the MySQL implementation.
- No breaking changes — all fixes are internal or additive.

## [1.0.5]

- Added PostgreSQL and SQL Server support
- Added async methods, `QueryMultiple`, `ReflectionCache`, connection pooling
- Added `ExecuteScalar<T>()`, `ExecuteNonQuery()`
- Improved MySQL/PostgreSQL/SQL Server performance and documentation
- Fixed `Dictionary<string,string>` compatibility overloads, MySQL connection string handling, ORM mapping
