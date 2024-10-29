# BookYourResource

Your Way of making reservations.
![obraz](https://github.com/user-attachments/assets/a74254ea-07be-42c6-81bc-6e7672d473d1)



## Most important API endpoints:
- `/reservations/active` - returns JSON of active and sorted Reservations
- `/reservations/grouped/<resource_name>` - returns JSON of active Reservations of Resource grouped by StartDay(Reservation start)

\* Name refers to Resource: Name and CodeName

## Features
- Seeded db for testing
- User Andrzej
- Can be used for various types of Resources
- Prepared for future features

![obraz](https://github.com/user-attachments/assets/dead737e-f78f-46ff-85b3-1c99a5563bf9)


## Conceptual ERD Diagram

![diagram-export-29 10 2024-22_19_44](https://github.com/user-attachments/assets/5b66e547-e5a2-4e6c-aabf-e0af6fdfce55)


\* Users and Roles are in use with entities from Identity


---
## How to participate?
1. Use commit convention
## Conventional Commits Messages

### Types

- API relevant changes
  - `feat` Commits, that adds or remove a new feature
  - `fix` Commits, that fixes a bug
- `refactor` Commits, that rewrite/restructure your code, however does not change any API behaviour
  - `perf` Commits are special `refactor` commits, that improve performance
- `style` Commits, that do not affect the meaning (white-space, formatting, missing semi-colons, etc)
- `test` Commits, that add missing tests or correcting existing tests
- `docs` Commits, that affect documentation only
- `build` Commits, that affect build components like build tool, ci pipeline, dependencies, project version, ...
- `ops` Commits, that affect operational components like infrastructure, deployment, backup, recovery, ...
- `chore` Miscellaneous commits e.g. modifying `.gitignore`
- - `ci` – continuous integration related
- - `revert` – reverts a previous commit

-

#### gitignore enhanted with:

https://www.toptal.com/developers/gitignore/

![obraz](https://github.com/user-attachments/assets/a56495e3-f5f7-4ae3-90d6-d52b55f47203)

#### Purge db (destructive edition)

```bash
rm -rf Migrations/*
rm reservations.db reservations.db-shm reservations.db-wal

```

Recreate

```bash
dotnet ef migrations add InitialCreate
dotnet ef database update

```
