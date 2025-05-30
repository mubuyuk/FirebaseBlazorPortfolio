﻿using Google.Cloud.Firestore;

namespace FirebaseBlazorPortfolio.Models
{
    [FirestoreData]
    public class Skill
    {
        [FirestoreProperty]
        public string? TechName { get; set; }

        [FirestoreProperty]
        public string? Category { get; set; }

        [FirestoreProperty]
        public string? ImageUrl { get; set; }
    }

    [FirestoreData]
    public class Project
    {
        [FirestoreDocumentId]
        public string? Id { get; set; }

        [FirestoreProperty]
        public string? Title { get; set; }

        [FirestoreProperty]
        public string? Description { get; set; }

        [FirestoreProperty]
        public string? Link { get; set; }

        [FirestoreProperty]
        public string? ImageUrl { get; set; }

        [FirestoreProperty]
        public List<string> Tags { get; set; } = new();
    }
}
