using Google.Cloud.Firestore;

namespace MyPortfolio.Models
{
    [FirestoreData]
    public class Skill
    {
        [FirestoreProperty]
        public string TechName { get; set; }

        [FirestoreProperty]
        public string Category { get; set; }

        [FirestoreProperty]
        public string ImageUrl { get; set; }
    }
}
