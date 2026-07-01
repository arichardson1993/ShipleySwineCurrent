using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Hosting;

namespace ShipleySwine.Models
{
    public static class ContactBlockStore
    {
        private static readonly object SyncRoot = new object();
        private const string StoragePath = "~/App_Data/contact-blocks.json";

        public static IReadOnlyList<ContactBlockEntry> GetAll()
        {
            lock (SyncRoot)
            {
                return LoadCore()
                    .OrderByDescending(entry => entry.CreatedUtc)
                    .ToList();
            }
        }

        public static ContactBlockEntry Add(string email, string phone, string reason)
        {
            string normalizedEmail = NormalizeEmail(email);
            string normalizedPhone = NormalizePhone(phone);

            if (string.IsNullOrWhiteSpace(normalizedEmail) && string.IsNullOrWhiteSpace(normalizedPhone))
            {
                throw new InvalidOperationException("Provide an email address, a phone number, or both.");
            }

            lock (SyncRoot)
            {
                List<ContactBlockEntry> entries = LoadCore().ToList();

                ContactBlockEntry existing = entries.FirstOrDefault(entry =>
                    string.Equals(NormalizeEmail(entry.Email), normalizedEmail, StringComparison.OrdinalIgnoreCase) &&
                    string.Equals(NormalizePhone(entry.Phone), normalizedPhone, StringComparison.OrdinalIgnoreCase));

                if (existing != null)
                {
                    if (!string.IsNullOrWhiteSpace(reason))
                    {
                        existing.Reason = reason.Trim();
                    }

                    SaveCore(entries);
                    return existing;
                }

                ContactBlockEntry newEntry = new ContactBlockEntry
                {
                    Id = Guid.NewGuid(),
                    Email = string.IsNullOrWhiteSpace(normalizedEmail) ? null : normalizedEmail,
                    Phone = string.IsNullOrWhiteSpace(normalizedPhone) ? null : normalizedPhone,
                    Reason = string.IsNullOrWhiteSpace(reason) ? null : reason.Trim(),
                    CreatedUtc = DateTime.UtcNow
                };

                entries.Add(newEntry);
                SaveCore(entries);
                return newEntry;
            }
        }

        public static bool Remove(Guid id)
        {
            lock (SyncRoot)
            {
                List<ContactBlockEntry> entries = LoadCore().ToList();
                int removed = entries.RemoveAll(entry => entry.Id == id);

                if (removed > 0)
                {
                    SaveCore(entries);
                }

                return removed > 0;
            }
        }

        public static bool IsBlocked(string email, string phone, out ContactBlockEntry matchedEntry)
        {
            string normalizedEmail = NormalizeEmail(email);
            string normalizedPhone = NormalizePhone(phone);

            lock (SyncRoot)
            {
                matchedEntry = LoadCore().FirstOrDefault(entry =>
                {
                    bool emailMatches = string.IsNullOrWhiteSpace(entry.Email) ||
                        string.Equals(NormalizeEmail(entry.Email), normalizedEmail, StringComparison.OrdinalIgnoreCase);
                    bool phoneMatches = string.IsNullOrWhiteSpace(entry.Phone) ||
                        string.Equals(NormalizePhone(entry.Phone), normalizedPhone, StringComparison.OrdinalIgnoreCase);
                    bool hasCriteria = !string.IsNullOrWhiteSpace(entry.Email) || !string.IsNullOrWhiteSpace(entry.Phone);
                    return hasCriteria && emailMatches && phoneMatches;
                });

                return matchedEntry != null;
            }
        }

        public static string NormalizeEmail(string value)
        {
            return string.IsNullOrWhiteSpace(value)
                ? null
                : value.Trim().ToLowerInvariant();
        }

        public static string NormalizePhone(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                return null;
            }

            string digits = Regex.Replace(value, @"\D", string.Empty);
            return string.IsNullOrWhiteSpace(digits) ? null : digits;
        }

        private static IEnumerable<ContactBlockEntry> LoadCore()
        {
            string path = GetStorageFilePath();
            if (!File.Exists(path))
            {
                return new List<ContactBlockEntry>();
            }

            string json = File.ReadAllText(path);
            if (string.IsNullOrWhiteSpace(json))
            {
                return new List<ContactBlockEntry>();
            }

            try
            {
                return JsonConvert.DeserializeObject<List<ContactBlockEntry>>(json) ?? new List<ContactBlockEntry>();
            }
            catch
            {
                return new List<ContactBlockEntry>();
            }
        }

        private static void SaveCore(IEnumerable<ContactBlockEntry> entries)
        {
            string path = GetStorageFilePath();
            string directory = Path.GetDirectoryName(path);

            if (!Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }

            string json = JsonConvert.SerializeObject(
                entries.OrderByDescending(entry => entry.CreatedUtc),
                Formatting.Indented);

            File.WriteAllText(path, json);
        }

        private static string GetStorageFilePath()
        {
            string mappedPath = HostingEnvironment.MapPath(StoragePath);
            if (!string.IsNullOrWhiteSpace(mappedPath))
            {
                return mappedPath;
            }

            return Path.Combine(HttpRuntime.AppDomainAppPath, "App_Data", "contact-blocks.json");
        }
    }
}
