# PgDeployDiff

A desktop tool for comparing PostgreSQL databases and generating production deployment artifacts.

## The Problem

Working with PostgreSQL in real-world projects involves two recurring pain points:

**1. Deploying to production without a safety net**
When applying changes to a production database, you need a forward script, a rollback script,
and a document describing what changed and why. Doing this manually is tedious and error-prone —
especially when the diff involves both schema and data changes across large environments.

**2. Understanding undocumented systems**
When you inherit a system with little or no documentation, it's often impossible to know
what a single button click actually does to the database. Which tables are involved?
What gets inserted, updated, or deleted?

The usual approach is to compare the database before and after the action —
but for PostgreSQL, the tools that do this well are either expensive or non-existent.
Visual Studio's Schema/Data Compare is great, but it only works with SQL Server.

PgDeployDiff was born from both of these problems.

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
