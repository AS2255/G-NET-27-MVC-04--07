using GymManagement.DAL.Data.DbContexts;
using GymManagement.DAL.Data.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace GymManagement.DAL.Data.DataSeeding
{
    public class GymDataSeeding
    {
        public async static Task SeedAsync(
            GymDbContext context,
            ILogger logger,
            string seedFilesPath,
            CancellationToken ct = default)
        {
            try
            {
                if (!context.Plans.Any())
                {
                    var plans = LoadDataFromJsonFile<Plan>(seedFilesPath, "plans.json");
                    // Add Plans To Database
                    if (plans.Any())
                    {
                        await context.Plans.AddRangeAsync(plans);
                        logger.LogInformation($"Seeding Plans With Counts {plans.Count}");
                    }
                }
                //
                if (context.ChangeTracker.HasChanges())
                    await context.SaveChangesAsync(ct);
                // 
                else
                    logger.LogInformation("Plans Table Already Seeded");
            }
            catch (Exception ex)
            {
                logger.LogError(ex.Message);
            }
        }


        private static List<T> LoadDataFromJsonFile<T>(string folderPath, string fileName)
        {
            var fullFilePath = Path.Combine(folderPath, fileName);

            if (!File.Exists(fullFilePath))
                throw new FileNotFoundException($"Data Seeding File Is Not Found At : {fullFilePath}");

            // Read Data From JSON File "plans.json" As String
            var data = File.ReadAllText(fullFilePath);
            var options = new JsonSerializerOptions()
            {
                PropertyNameCaseInsensitive = true,
            };
            // Convert JSON String To List Of Plan Objects
            return JsonSerializer.Deserialize<List<T>>(data, options) ?? [];
        }

    }
}
