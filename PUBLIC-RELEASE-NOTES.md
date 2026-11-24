# Public release notes

This repository is a history-free snapshot of the ARF source code (branch `master`, commit `54d823d`), published on 2026-11-24.

Removed before publishing:
- `Database/*.bak`, `Database/Old/*.bak` and the full database dumps (`arf_db_*.sql`, `db_init.sql`, `script*.sql`, `arf_schema_and_data.sql`, `db_v5.sql`). They held real user records (names, e-mail addresses, phone numbers, password hashes). The schema, migration and lookup-data scripts are kept.
- People's contact data in `Documents/`: `Lookups.xlsx`, the referees database template and the vice-deans/centre-directors list.

Values replaced with `__SET_ME__` (set your own before building or running):
- `NewSources/ARF.Core/Constants.cs`: `SendGridAPIKey`, `EncryptionSecurityKey`. A different `EncryptionSecurityKey` cannot read data encrypted with the original one.
- `Web.config` / `App.config` connection-string passwords, and `smtp:Password` in `NewSources/ARF.Web/Views/Web.config`.
- SMTP and SMS provider passwords in `NewSources/ARF.Services/Sms*.cs`, `NewSources/ARF.Core/EmailService.cs` and `NewSources/ARF.Core/Utils.cs`.
