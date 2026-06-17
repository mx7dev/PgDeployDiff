# PgDeployDiff

A desktop tool for comparing PostgreSQL databases and generating production deployment artifacts.

## The Problem

When deploying changes to a PostgreSQL production database, you typically need three things:
a **forward script** (to apply the changes), a **rollback script** (to undo them if something goes wrong),
and a **description document** separating DDL from DML changes.

Doing this manually is tedious, error-prone, and time-consuming — especially when the diff
between environments is large or involves both schema and data changes.

## What PgDeployDiff Does

PgDeployDiff connects to two PostgreSQL databases (e.g. staging and production),
compares their schemas and data, and generates the deployment package for you:

- **Forward script** — SQL to apply all detected changes
- **Rollback script** — SQL to safely revert them
- **Deployment document** — a human-readable summary separating DDL and DML changes

## Status

> This project is currently under active development.

- [x] Project structure and architecture
- [ ] Database connection UI
- [ ] Schema comparison (tables, columns, indexes, constraints)
- [ ] Data comparison
- [ ] Forward script generation
- [ ] Rollback script generation
- [ ] Deployment document export

## Tech Stack

- .NET 10 — WPF desktop application
- Npgsql — PostgreSQL driver
- Material Design 3 — UI theme
- MVVM pattern with CommunityToolkit.Mvvm

## License

MIT
