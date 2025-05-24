using Google.Cloud.Firestore;
using FirebaseBlazorPortfolio.Models;

namespace FirebaseBlazorPortfolio.Services
{
    public class FirestoreService
    {
        private readonly FirestoreDb _firestoreDb;

        public FirestoreService(IWebHostEnvironment env)
        {
            string path = Path.Combine(env.ContentRootPath, "Keys", "portfolio-17515-firebase-adminsdk-fbsvc-d9017fc234.json");
            Environment.SetEnvironmentVariable("GOOGLE_APPLICATION_CREDENTIALS", path);

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

        public async Task AddProjectAsync(Project project)
        {
            var projectsRef = _firestoreDb.Collection("projects");
            await projectsRef.AddAsync(project);
        }

        public async Task DeleteProjectAsync(Project project)
        {
            if (string.IsNullOrEmpty(project.Id)) return;

            var docRef = _firestoreDb.Collection("projects").Document(project.Id);
            await docRef.DeleteAsync();
        }

        public async Task UpdateProjectAsync(Project project)
        {
            if (string.IsNullOrEmpty(project.Id)) return;

            var docRef = _firestoreDb.Collection("projects").Document(project.Id);
            await docRef.SetAsync(project, SetOptions.Overwrite);
        }

    }
}
