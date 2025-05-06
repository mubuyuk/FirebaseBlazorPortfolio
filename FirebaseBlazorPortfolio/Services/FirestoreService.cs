using Google.Cloud.Firestore;
using MyPortfolio.Models;

namespace MyPortfolio.Services
{
    public class FirestoreService
    {
        private readonly FirestoreDb _firestoreDb;

        public FirestoreService(IWebHostEnvironment env)
        {
            // Hämta sökvägen till din Firebase JSON-nyckel
            string path = Path.Combine(env.ContentRootPath, "Keys", "portfolio-17515-firebase-adminsdk-fbsvc-d9017fc234.json");
            Environment.SetEnvironmentVariable("GOOGLE_APPLICATION_CREDENTIALS", path);

            // Anslut till Firestore (byt till ditt riktiga Firebase-projekt-ID)
            _firestoreDb = FirestoreDb.Create("portfolio-17515");
        }

        public async Task<List<Skill>> GetSkillsAsync()
        {
            Query query = _firestoreDb.Collection("skills");
            QuerySnapshot snapshot = await query.GetSnapshotAsync();

            List<Skill> skills = new();
            foreach (DocumentSnapshot doc in snapshot.Documents)
            {
                if (doc.Exists)
                {
                    Skill skill = doc.ConvertTo<Skill>();
                    skills.Add(skill);
                }
            }

            return skills;
        }

        public async Task<List<Project>> GetProjectsAsync()
        {
            Query query = _firestoreDb.Collection("projects");
            QuerySnapshot snapshots = await query.GetSnapshotAsync();

            List<Project> projects = new();
            foreach (DocumentSnapshot doc in snapshots.Documents)
            {
                if (doc.Exists)
                {
                    Project project = doc.ConvertTo<Project>();
                    projects.Add(project);
                }
            }

            return projects;
        }

        public async Task SeedProjectsAsync()
        {
            CollectionReference projectsRef = _firestoreDb.Collection("projects");

            var projects = new List<Project>
    {
        new()
        {
            Title = "Portfolio Website",
            Description = "Responsiv Blazor Server-portfolio med Firebase och Bootstrap.",
            Link = "https://github.com/mubuyuk/portfolio",
            Tags = new List<string> { "Blazor", ".NET", "Firebase", "Bootstrap" }
        },
        new()
        {
            Title = "Azure DevOps Pipeline Demo",
            Description = "CI/CD-pipeline för .NET med slot-deployment och testautomatisering.",
            Link = "https://github.com/mubuyuk/devops-demo",
            Tags = new List<string> { "Azure DevOps", "CI/CD", ".NET", "Pipelines" }
        },
        new()
        {
            Title = "MongoDB Admin Tool",
            Description = "Admin-UI för MongoDB Atlas med Blazor och realtidsfunktioner.",
            Link = "https://github.com/mubuyuk/mongo-admin",
            Tags = new List<string> { "MongoDB", "Blazor", "Admin UI" }
        },
        new()
        {
            Title = "Blazor Chatbot",
            Description = "En lokal AI-chatbot byggd i Blazor Server och C#.",
            Link = "https://github.com/mubuyuk/blazor-chatbot",
            Tags = new List<string> { "Blazor", "Chatbot", "AI", ".NET" }
        }
    };

            foreach (var project in projects)
            {
                await projectsRef.AddAsync(project);
            }
        }

    }
}
