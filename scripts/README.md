# Comparing local files with Plesk

Download the live site's document root from Plesk into a separate folder. Do not
download it over the local project folder.

From the repository root, run:

```powershell
.\scripts\Compare-PleskSnapshot.ps1 -PleskSnapshot 'C:\path\to\plesk-download'
```

The report has three statuses:

- `Changed`: the relative path exists in both places but its contents differ.
- `Plesk only`: usually an image or other file created through the live admin site.
- `Local only`: a file exists locally but is absent from the Plesk snapshot.

To save the report:

```powershell
.\scripts\Compare-PleskSnapshot.ps1 `
    -PleskSnapshot 'C:\path\to\plesk-download' `
    -CsvPath '.\plesk-comparison.csv'
```

This script is read-only. It does not copy, delete, or overwrite files. It compares
files by size and SHA-256 content hash and ignores development/build directories.

Database records are not compared when production uses a database server. Export
the production database separately when a local database refresh is needed.
