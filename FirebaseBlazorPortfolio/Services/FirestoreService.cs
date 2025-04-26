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
    }
}
