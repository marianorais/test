using System;
using System.Diagnostics;
using System.Data;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using KWB.Web.Models;
using KWB.Web.Models.Response;

namespace KWB.Web
{
    public class AplicationDbContext : DbContext
    {
        public AplicationDbContext(DbContextOptions<AplicationDbContext> Options)
           : base(Options)
        {

        }
        public DbSet<Certification> Certification { get; set; }
        public DbSet<Place> Place { get; set; }
        public DbSet<Tipo> Tipo { get; set; }
        public DbSet<TipoTag> TipoTag { get; set; }
        public DbSet<Tokens> Tokens { get; set; }
        public DbSet<Eruv> Eruv { get; set; }
        public DbSet<Caracteristicas> Caracteristicas { get; set; }
        public DbSet<LocationTag> LocationTag { get; set; }
        public DbSet<PlaceLocationTag> PlaceLocationTag { get; set; }
        public DbSet<Favorite> Favorite { get; set; }
        public DbSet<Review> Review { get; set; }
        public DbSet<ThingsToDo> ThingsToDo { get; set; }
        public DbSet<Photo> Photo { get; set; }
        public DbSet<PrayerTime> PrayerTime { get; set; }
        public DbSet<Tag> Tag { get; set; }
        public DbSet<User> User { get; set; }
        public DbSet<Suggestion> Suggestion { get; set; }
        public DbSet<ContactMessage> ContactMessage { get; set; }
        public DbSet<UserReport> UserReport { get; set; }
        public DbSet<ChangesHistory> ChangesHistory { get; set; }
        public DbSet<ApiCallsRegister> ApiCallsRegister { get; set; }
        public DbSet<Faq> Faq { get; set; }
        public DbSet<UserChange> UserChange { get; set; }
        public DbSet<Sponsor> Sponsor { get; set; }

    }
}
